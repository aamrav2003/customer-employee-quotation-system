using Xunit;
using Moq;
using IAB_251_Assessment2.BusinessLogic.Services;
using IAB_251_Assessment2.DataAccess.Interfaces;
using IAB_251_Assessment2.BusinessLogic.Entities;
using System;
using System.Collections.Generic;

namespace IAB_251_Assessment2.Tests
{
    public class QuotationServiceTests
    {
        private readonly Mock<IQuoteRepository> _mockQuoteRepository;
        private readonly QuotationService _quotationService;

        public QuotationServiceTests()
        {
            _mockQuoteRepository = new Mock<IQuoteRepository>();
            _quotationService = new QuotationService(_mockQuoteRepository.Object);
        }

        [Fact]
        public void GetAll_ShouldReturnAllQuotations()
        {
            // Arrange
            var expectedQuotes = new List<Quotation>
            {
                new Quotation { Id = 1, clientName = "Test Client 1", dateIssued = DateTime.Now, status = "Pending" },
                new Quotation { Id = 2, clientName = "Test Client 2", dateIssued = DateTime.Now, status = "Accepted" }
            };

            _mockQuoteRepository.Setup(repo => repo.GetAll())
                .Returns(expectedQuotes);

            // Act
            var result = _quotationService.GetAll();

            // Assert
            Assert.Equal(expectedQuotes.Count, result.Count);
            Assert.Equal(expectedQuotes[0].clientName, result[0].clientName);
            Assert.Equal(expectedQuotes[1].status, result[1].status);
            _mockQuoteRepository.Verify(repo => repo.GetAll(), Times.Once);
        }

        [Fact]
        public void GetById_ShouldReturnQuotation()
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

            _mockQuoteRepository.Setup(repo => repo.GetById(testId))
                .Returns(expectedQuote);

            // Act
            var result = _quotationService.GetById(testId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedQuote.Id, result.Id);
            Assert.Equal(expectedQuote.clientName, result.clientName);
            Assert.Equal(expectedQuote.status, result.status);
            _mockQuoteRepository.Verify(repo => repo.GetById(testId), Times.Once);
        }

        [Fact]
        public void Add_ShouldCallRepositoryAdd()
        {
            // Arrange
            var newQuote = new Quotation 
            { 
                clientName = "New Client", 
                dateIssued = DateTime.Now, 
                status = "Pending" 
            };

            // Act
            _quotationService.Add(newQuote);

            // Assert
            _mockQuoteRepository.Verify(repo => repo.Add(It.Is<Quotation>(q => 
                q.clientName == newQuote.clientName && 
                q.status == newQuote.status)), Times.Once);
        }

        [Fact]
        public void Update_ShouldCallRepositoryUpdate()
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
            _quotationService.Update(updateQuote);

            // Assert
            _mockQuoteRepository.Verify(repo => repo.Update(It.Is<Quotation>(q => 
                q.Id == updateQuote.Id && 
                q.clientName == updateQuote.clientName && 
                q.status == updateQuote.status)), Times.Once);
        }

        [Fact]
        public void Delete_ShouldCallRepositoryDelete()
        {
            // Arrange
            int testId = 1;

            // Act
            _quotationService.Delete(testId);

            // Assert
            _mockQuoteRepository.Verify(repo => repo.Delete(testId), Times.Once);
        }

        [Fact]
        public void GetById_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            int invalidId = -1;
            _mockQuoteRepository.Setup(repo => repo.GetById(invalidId))
                .Returns((Quotation)null);

            // Act
            var result = _quotationService.GetById(invalidId);

            // Assert
            Assert.Null(result);
            _mockQuoteRepository.Verify(repo => repo.GetById(invalidId), Times.Once);
        }
    }
}
