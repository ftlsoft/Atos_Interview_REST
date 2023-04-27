using CSharpFunctionalExtensions;
using Interview_Atos.Models;
using Interview_Atos.Services;
using Interview_Atos.Store;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace InterviewAtos.Tests
{
    public class CustomerServiceTests
    {
        private readonly ICustomerService _customerService;

        public CustomerServiceTests()
        {
            var svcLoggerMock = Mock.Of<ILogger<CustomerService>>();
            var repoLoggerMock = Mock.Of<ILogger<CustomerRepository>>();

            var repo = new CustomerRepository(repoLoggerMock);

            _customerService = new CustomerService(svcLoggerMock, repo);
        }

        [Fact]
        public void Create_ValidData_ReturnSavedObjectWithValidId()
        {
            var result = _customerService.Create("firstname", "lastname");

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value.Id);
            Assert.True(result.Value.Id > 0);
        }

        [Fact]
        public async Task CreateAsync_InvalidData_ReturnResultWithValidationErrors()
        {
            var result = await _customerService.CreateAsync(new Customer() { Firstname = null, Lastname = String.Empty });

            Assert.True(result.IsFailure);
            Assert.NotNull(result.Error);
            Assert.All(result.Error, (err) => err.Contains("can not be null or empty"));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task DeleteAsync_ValidId_ReturnTrue(int id)
        {
            var result = await _customerService.DeleteAsync(id);
            Assert.True(result);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(99999999)]
        public async Task DeleteAsync_InvalidId_ReturnFalse(int id)
        {
            var result = await _customerService.DeleteAsync(id);
            Assert.False(result);
        }

        [Fact]
        public async Task GetAllAsync_ReturnListOfMultipleObjects()
        {
            var customers = new List<Customer>();
            
            await foreach (var customer in _customerService.GetAllAsync())
            {
                customers.Add(customer);
            }

            Assert.NotEmpty(customers);
            Assert.True(customers.Count > 2);
        }
    }
}