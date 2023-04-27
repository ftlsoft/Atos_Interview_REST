using Interview_Atos.Models;
using Interview_Atos.Store;
using CSharpFunctionalExtensions;

namespace Interview_Atos.Services
{
    public interface ICustomerService
    {
        Result<Customer, List<string>> Create(string firstname, string lastname);
        Task<Result<Customer, List<string>>> CreateAsync(Customer customerToCreate);
        Task<Customer> AddAsync(Customer customer);
        IAsyncEnumerable<Customer> GetAllAsync();
        Task<bool> DeleteAsync(int customerId);
        Task<Customer?> FindAsync(int id);
    }

    public class CustomerService : ICustomerService
    {
        private readonly ILogger<CustomerService> _logger;
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ILogger<CustomerService> logger, ICustomerRepository customerRepository)
        {
            _logger = logger;
            _customerRepository = customerRepository;
        }

        public async Task<Customer?> FindAsync(int id)
        {
            try
            {
                return await _customerRepository.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Find customer in repository exception");
                return null;
            }
        }

        public IAsyncEnumerable<Customer> GetAllAsync()
        {
            try
            {
                return _customerRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Get all customers from repository exception");
                throw;
            }
        }

        protected bool ValidateNewCustomer(Customer customerToValidate, out List<string> validationErrors)
        {
            validationErrors = new List<string>();

            if (customerToValidate == null)
            {
                validationErrors.Add("Customer object can not be null");
                return false;
            }

            if (string.IsNullOrEmpty(customerToValidate.Firstname))
                validationErrors.Add("Customer firstname can not be null or empty");

            if (string.IsNullOrEmpty(customerToValidate.Lastname))
                validationErrors.Add("Customer lastname can not be null or empty");

            return !validationErrors.Any();
        }

        //ToDo: Add to readme
        /// <summary>
        /// In case of failure Error object contains list of validation/error strings
        /// </summary>
        /// <param name="firstname"></param>
        /// <param name="lastname"></param>
        /// <returns>Created customer</returns>
        public Result<Customer, List<string>> Create(string firstname, string lastname)
        {
            return CreateAsync(new Customer() { Firstname = firstname, Lastname = lastname }).Result;
        }

        //ToDo: Add to readme
        public async Task<Result<Customer, List<string>>> CreateAsync(Customer customerToCreate)
        {
            List<string> validationErrors;

            if (!ValidateNewCustomer(customerToCreate, out validationErrors))
            {
                _logger.LogDebug("{0} {1} return false, validation errors: {2}", nameof(CustomerService), nameof(ValidateNewCustomer), String.Join(", ", validationErrors));
                return Result.Failure<Customer, List<string>>(validationErrors);
            }

            try
            {
                var created = await _customerRepository.AddAsync(customerToCreate);

                return customerToCreate;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Add customer to repository exception");
                return Result.Failure<Customer, List<string>>(new List<string>() { "Add customer to repository error: {ex.Message}" });
            }
        }

        public async Task<Customer> AddAsync(Customer customer)
        {
            return await _customerRepository.AddAsync(customer);
        }

        public async Task<bool> DeleteAsync(int customerId)
        {
            try
            {
                return await _customerRepository.DeleteAsync(customerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Delete customer from repository exception, customerId: {customerId}");
                return false;
            }
        }
    }
}
