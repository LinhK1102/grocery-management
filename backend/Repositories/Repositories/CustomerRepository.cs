using BusinessObjects.Entities;
using DataAccess.DAO;
using Repositories.Interfaces;
using System.Collections.Generic;
using Utility.Common;

namespace Repositories.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerDAO _dao;

        public CustomerRepository(CustomerDAO customerDAO)
        {
            _dao = customerDAO;
        }

        public IEnumerable<Customer> GetAllCustomers() => _dao.GetAllCustomers();

        public Customer GetCustomerById(int id) => _dao.GetCustomerById(id);
        public async Task<Customer?> GetCustomerByNameAsync(string name)
        {
            return await _dao.GetCustomerByNameAsync(name);
        }

        public Customer CreateCustomer(Customer customer) => _dao.AddCustomer(customer);

        public Customer UpdateCustomer(Customer customer) => _dao.UpdateCustomer(customer);

        public bool DeleteCustomer(int id) => _dao.DeleteCustomer(id);

        public IEnumerable<Customer> SearchCustomers(string searchTerm) => _dao.SearchCustomers(searchTerm);

        public IEnumerable<Customer> GetHighDiscountCustomers(decimal minDiscount) => _dao.GetHighDiscountCustomers(minDiscount);

        //public IEnumerable<Customer> GetCustomersByPurchaseFrequency(int minFrequency) => _dao.GetCustomersByPurchaseFrequency(minFrequency);

        public bool UpdateCustomerDiscountRateWithLimit(int customerId, decimal newDiscountRate, decimal maxLimit)
            => _dao.UpdateCustomerDiscountRateWithLimit(customerId, newDiscountRate, maxLimit);

        public async Task<bool> EnsureSeedDataAsync()
        {
            var existing = await GetCustomerByNameAsync(UtitlityConstant.Category_Default_Name);

            if (existing == null)
            {
                var defaultCustomer = new Customer
                {
                    CustomerName = UtitlityConstant.Customer_Default_Name,
                    CustomerType = UtitlityConstant.Customer_Type_Unknown
                };

              var createdCustomer = CreateCustomer(defaultCustomer) ;
                return  createdCustomer != null; // đã thêm dữ liệu mặc định, trả về true
            }

            return true; // đã có dữ liệu, không cần thêm nữa
        }


    }
}
