using CSharpFunctionalExtensions;
using Interview_Atos.Models;

namespace Interview_Atos.Store
{
    public interface ICustomerRepository
    {
        IAsyncEnumerable<Customer> GetAllAsync();

        Task<Customer> AddAsync(Customer customer);

        Task<bool> DeleteAsync(int id);
        Task<Customer> FindAsync(int id);
    }

    /// <summary>
    /// Simple Customers list
    /// </summary>
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ILogger<CustomerRepository> _logger;

        private readonly List<Customer> _allCustomers = new List<Customer>()
        {
            new Customer() { Id = 0, Firstname = "Customer0_Firstname", Lastname = "Customer0_Lastname" },
            new Customer() { Id = 1, Firstname = "Customer1_Firstname", Lastname = "Customer1_Lastname" },
            new Customer() { Id = 2, Firstname = "Customer2_Firstname", Lastname = "Customer2_Lastname" },
            new Customer() { Id = 3, Firstname = "Customer3_Firstname", Lastname = "Customer3_Lastname" },
            new Customer() { Id = 4, Firstname = "Customer4_Firstname", Lastname = "Customer4_Lastname" },
            new Customer() { Id = 5, Firstname = "Customer5_Firstname", Lastname = "Customer5_Lastname" },
            new Customer() { Id = 6, Firstname = "Customer6_Firstname", Lastname = "Customer6_Lastname" }
        };

        public CustomerRepository(ILogger<CustomerRepository> logger)
        {
            _logger = logger;
        }

        //Todo: add to readme
        public async IAsyncEnumerable<Customer> GetAllAsync()
        {
            foreach (var customer in _allCustomers)
            {
                await Task.Delay(500); //Simulate long data retrieval time
                yield return customer;
            }
        }

        public async Task<Customer> AddAsync(Customer customer)
        {
            return await Task.Run(() =>
            {
                if (customer.Id != null)
                {
                    _logger.LogDebug($"{nameof(CustomerRepository)} {nameof(AddAsync)} Customer Id can not be specified -> override");
                }
                customer.Id = _allCustomers.Max(c => c.Id) + 1;

                _allCustomers.Add(customer);

                _logger.LogDebug($"{nameof(CustomerRepository)} {nameof(AddAsync)} return created customer, customerId: {customer.Id}");

                return customer;
            });
        }

        public async Task<Customer> FindAsync(int customerId)
        {
            return await Task.Run(() =>
            {
                var result = _allCustomers.First(c => c.Id == customerId);

                _logger.LogDebug($"{nameof(CustomerRepository)} {nameof(FindAsync)} return {result}, customerId: {customerId}");

                return result;
            });
        }

        public async Task<bool> DeleteAsync(int customerId)
        {
            return await Task.Run(() =>
            {
                var result = _allCustomers.Remove(_allCustomers.First(c => c.Id == customerId));

                _logger.LogDebug($"{nameof(CustomerRepository)} {nameof(DeleteAsync)} return {result}, customerId: {customerId}");

                return result;
            });
        }

    }
}
