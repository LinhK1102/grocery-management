using BusinessObjects.Entities;
using System.Collections.Generic;

namespace Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        IEnumerable<Customer> GetAllCustomers();
        Customer GetCustomerById(int id);
        Task<Customer?> GetCustomerByNameAsync(string name);
        void CreateCustomer(Customer customer);
        void UpdateCustomer(Customer customer);
        void DeleteCustomer(int id);

        IEnumerable<Customer> SearchCustomers(string searchTerm);
        IEnumerable<Customer> GetHighDiscountCustomers(decimal minDiscount);
        //IEnumerable<Customer> GetCustomersByPurchaseFrequency(int minFrequency);
        bool UpdateCustomerDiscountRateWithLimit(int customerId, decimal newDiscountRate, decimal maxLimit);
        Task EnsureDefaultCustomerAsync();
    }
}
