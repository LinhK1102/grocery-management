using BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.DAO
{
    public class CustomerDAO
    {
        private readonly ApplicationDbContext _context;

        public CustomerDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Customer> GetAllCustomers() => _context.Customers.ToList();

        public Customer GetCustomerById(int id) => _context.Customers.Find(id);
        public async Task<Customer?> GetCustomerByNameAsync(string name)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.CustomerName == name);
        }

        public void AddCustomer(Customer c)
        {
            _context.Customers.Add(c);
            _context.SaveChanges();
        }

        public void UpdateCustomer(Customer c)
        {
            _context.Customers.Update(c);
            _context.SaveChanges();
        }

        public void DeleteCustomer(int id)
        {
            var c = _context.Customers.Find(id);
            if (c != null)
            {
                _context.Customers.Remove(c);
                _context.SaveChanges();
            }
        }

        public IEnumerable<Customer> SearchCustomers(string searchTerm)
        {
            return _context.Customers
                .Where(c => c.CustomerName.Contains(searchTerm))
                .ToList();
        }

        public IEnumerable<Customer> GetHighDiscountCustomers(decimal minDiscount)
        {
            return _context.Customers
                .Where(c => c.DiscountRate >= minDiscount)
                .ToList();
        }

        //public IEnumerable<Customer> GetCustomersByPurchaseFrequency(int minFrequency)
        //{
        //    return _context.Customers
        //        .Where(c => c.PurchaseCount >= minFrequency)
        //        .ToList();
        //}

        public bool UpdateCustomerDiscountRateWithLimit(int customerId, decimal newDiscountRate, decimal maxLimit)
        {
            var customer = _context.Customers.Find(customerId);
            if (customer == null || newDiscountRate > maxLimit) return false;

            customer.DiscountRate = newDiscountRate;
            _context.SaveChanges();
            return true;
        }
    }
}
