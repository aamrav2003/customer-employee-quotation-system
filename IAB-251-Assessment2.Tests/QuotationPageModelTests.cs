using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IAB_251_Assessment2.Pages;
using IAB_251_Assessment2.Application.Interfaces;
using IAB_251_Assessment2.BusinessLogic.Entities;
using IAB_251_Assessment2.Pages.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IAB_251_Assessment2.Tests
{
    public class QuotationPageModelTests
    {
        private readonly Mock<IQuoteAppService> _mockQuoteAppService;
        private readonly QuotationModel _pageModel;

        public QuotationPageModelTests()
        {
            _mockQuoteAppService = new Mock<IQuoteAppService>();
            _pageModel = new QuotationModel(_mockQuoteAppService.Object);
        }

        [Fact]
        public void OnGet_ShouldPopulateQuoteList()
        {
            // Arrange
            var quotations = new List<Quotation>
            {
                new Quotation 
                { 
                    Id = 1, 
                    clientName = "Test Client 1", 
                    dateIssued = DateTime.Now, 
                    status = "Pending" 
                },
                new Quotation 
                { 
                    Id = 2, 
                    clientName = "Test Client 2", 
                    dateIssued = DateTime.Now, 
                    status = "Accepted" 
                }
            };

            _mockQuoteAppService.Setup(service => service.GetAllQuotes())
                .Returns(quotations);

            // Act
            _pageModel.OnGet();

            // Assert
            Assert.Equal(2, _pageModel.QuoteList.Count);
            Assert.Equal(quotations[0].clientName, _pageModel.QuoteList[0].clientName);
            Assert.Equal(quotations[1].status, _pageModel.QuoteList[1].status);
            _mockQuoteAppService.Verify(service => service.GetAllQuotes(), Times.Once);
        }

        [Fact]
        public void OnPost_WithValidModelState_ShouldAddQuoteAndRedirect()
        {
            // Arrange
            _pageModel.Quote = new QuoteViewModel
            {
                clientName = "New Client",
                status = "Pending"
            };

            // Act
            var result = _pageModel.OnPost() as RedirectToPageResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("/Quotation", result.PageName);
            _mockQuoteAppService.Verify(service => service.AddQuote(
                It.Is<Quotation>(q =>
                    q.clientName == _pageModel.Quote.clientName &&
                    q.status == _pageModel.Quote.status)), 
                Times.Once);
        }

        [Fact]
        public void OnPost_WithInvalidModelState_ShouldReturnPage()
        {
            // Arrange
            _pageModel.ModelState.AddModelError("Error", "Test error");
            _pageModel.Quote = new QuoteViewModel
            {
                clientName = "New Client",
                status = "Pending"
            };

            // Act
            var result = _pageModel.OnPost() as PageResult;

            // Assert
            Assert.NotNull(result);
            _mockQuoteAppService.Verify(service => service.AddQuote(It.IsAny<Quotation>()), Times.Never);
        }

        [Fact]
        public void OnGet_WithEmptyQuotations_ShouldReturnEmptyList()
        {
            // Arrange
            var emptyQuotations = new List<Quotation>();
            _mockQuoteAppService.Setup(service => service.GetAllQuotes())
                .Returns(emptyQuotations);

            // Act
            _pageModel.OnGet();

            // Assert
            Assert.Empty(_pageModel.QuoteList);
            _mockQuoteAppService.Verify(service => service.GetAllQuotes(), Times.Once);
        }

        [Theory]
        [InlineData("Accepted")]
        [InlineData("Rejected")]
        public void OnPost_WithDifferentStatuses_ShouldAddQuoteWithCorrectStatus(string status)
        {
            // Arrange
            _pageModel.Quote = new QuoteViewModel
            {
                clientName = "Test Client",
                status = status
            };

            // Act
            var result = _pageModel.OnPost() as RedirectToPageResult;

            // Assert
            Assert.NotNull(result);
            _mockQuoteAppService.Verify(service => service.AddQuote(
                It.Is<Quotation>(q =>
                    q.status == status)), 
                Times.Once);
        }
    }
}
