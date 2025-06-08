using Xunit;
using Moq;
using IAB_251_Assessment2.Application.Services;
using IAB_251_Assessment2.BusinessLogic.Interfaces;
using IAB_251_Assessment2.BusinessLogic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IAB_251_Assessment2.Tests
{
    public class QuoteAppServiceTests
    {
        private readonly Mock<IQuotationService> _mockQuoteService;
        private readonly QuoteAppService _quoteAppService;

        public QuoteAppServiceTests()
        {
            _mockQuoteService = new Mock<IQuotationService>();
            _quoteAppService = new QuoteAppService(_mockQuoteService.Object);
        }

        [Fact]
        public void GetAllQuotes_ShouldReturnListOfQuotations()
        {
            // Arrange
            var expectedQuotes = new List<Quotation>
            {
                new Quotation { Id = 1, clientName = "Test Client 1", dateIssued = DateTime.Now, status = "Pending" },
                new Quotation { Id = 2, clientName = "Test Client 2", dateIssued = DateTime.Now, status = "Accepted" }
            };

            _mockQuoteService.Setup(service => service.GetAll())
                .Returns(expectedQuotes);

            // Act
            var result = _quoteAppService.GetAllQuotes();

            // Assert
            Assert.Equal(expectedQuotes.Count, result.Count);
            Assert.Equal(expectedQuotes[0].clientName, result[0].clientName);
            Assert.Equal(expectedQuotes[1].status, result[1].status);
            _mockQuoteService.Verify(service => service.GetAll(), Times.Once);
        }

        [Fact]
        public void GetQuote_ShouldReturnQuotationById()
        {
            // Arrange
            int testId = 1;
            var expectedQuote = new Quotation 
            { 
                Id = testId, 
                clientName = "Test Client", 
                dateIssued = DateTime.Now, 
                status = "Pending" 
            };

            _mockQuoteService.Setup(service => service.GetById(testId))
                .Returns(expectedQuote);

            // Act
            var result = _quoteAppService.GetQuote(testId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedQuote.Id, result.Id);
            Assert.Equal(expectedQuote.clientName, result.clientName);
            Assert.Equal(expectedQuote.status, result.status);
            _mockQuoteService.Verify(service => service.GetById(testId), Times.Once);
        }

        [Fact]
        public void AddQuote_ShouldCallQuotationServiceAdd()
        {
            // Arrange
            var newQuote = new Quotation 
            { 
                clientName = "New Client", 
                dateIssued = DateTime.Now, 
                status = "Pending" 
            };

            // Act
            _quoteAppService.AddQuote(newQuote);

            // Assert
            _mockQuoteService.Verify(service => service.Add(It.Is<Quotation>(q => 
                q.clientName == newQuote.clientName && 
                q.status == newQuote.status)), Times.Once);
        }

        [Fact]
        public void UpdateQuote_ShouldCallQuotationServiceUpdate()
        {
            // Arrange
            var updateQuote = new Quotation 
            { 
                Id = 1, 
                clientName = "Updated Client", 
                dateIssued = DateTime.Now, 
                status = "Accepted" 
            };

            // Act
            _quoteAppService.UpdateQuote(updateQuote);

            // Assert
            _mockQuoteService.Verify(service => service.Update(It.Is<Quotation>(q => 
                q.Id == updateQuote.Id && 
                q.clientName == updateQuote.clientName && 
                q.status == updateQuote.status)), Times.Once);
        }

        [Fact]
        public void DeleteQuote_ShouldCallQuotationServiceDelete()
        {
            // Arrange
            int testId = 1;

            // Act
            _quoteAppService.DeleteQuote(testId);

            // Assert
            _mockQuoteService.Verify(service => service.Delete(testId), Times.Once);
        }
    }
}
