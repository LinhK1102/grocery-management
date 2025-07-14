using BusinessObjects.Entities;
using System.Collections.Generic;

namespace Repositories.Interfaces
{
    public interface ICustomerRepository : ISeedableRepository
    {
        IEnumerable<Customer> GetAllCustomers();
        Customer GetCustomerById(int id);
        Task<Customer?> GetCustomerByNameAsync(string name);
        Customer CreateCustomer(Customer customer);
        Customer UpdateCustomer(Customer customer);
        bool DeleteCustomer(int id);

        IEnumerable<Customer> SearchCustomers(string searchTerm);
        IEnumerable<Customer> GetHighDiscountCustomers(decimal minDiscount);
        //IEnumerable<Customer> GetCustomersByPurchaseFrequency(int minFrequency);
        bool UpdateCustomerDiscountRateWithLimit(int customerId, decimal newDiscountRate, decimal maxLimit);
    }
}
