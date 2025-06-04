using BusinessObjects;
using DataAccess.DAO;
using Repositories.Interfaces;
using System.Collections.Generic;

namespace Repositories.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerDAO _dao;

        public CustomerRepository(ApplicationDbContext ctx)
        {
            _dao = new CustomerDAO(ctx);
        }

        public IEnumerable<Customer> GetAllCustomers() => _dao.GetAllCustomers();

        public Customer GetCustomerById(int id) => _dao.GetCustomerById(id);

        public void CreateCustomer(Customer customer) => _dao.AddCustomer(customer);

        public void UpdateCustomer(Customer customer) => _dao.UpdateCustomer(customer);

        public void DeleteCustomer(int id) => _dao.DeleteCustomer(id);

        public IEnumerable<Customer> SearchCustomers(string searchTerm) => _dao.SearchCustomers(searchTerm);

        public IEnumerable<Customer> GetHighDiscountCustomers(decimal minDiscount) => _dao.GetHighDiscountCustomers(minDiscount);

        //public IEnumerable<Customer> GetCustomersByPurchaseFrequency(int minFrequency) => _dao.GetCustomersByPurchaseFrequency(minFrequency);

        public bool UpdateCustomerDiscountRateWithLimit(int customerId, decimal newDiscountRate, decimal maxLimit)
            => _dao.UpdateCustomerDiscountRateWithLimit(customerId, newDiscountRate, maxLimit);
    }
}
