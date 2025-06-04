using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class OrderDAO
    {
        private readonly ApplicationDbContext _context;

        public OrderDAO(ApplicationDbContext context) => _context = context;

        public List<Order> GetAllOrders() => _context.Orders.ToList();
        public Order GetOrderById(int id) => _context.Orders.Find(id);
        public void CreateOrder(Order o) { _context.Orders.Add(o); _context.SaveChanges(); }
        public void UpdateOrder(Order o) { _context.Orders.Update(o); _context.SaveChanges(); }
        public void DeleteOrder(int id)
        {
            var o = _context.Orders.Find(id);
            if (o != null) { _context.Orders.Remove(o); _context.SaveChanges(); }
        }

        public List<Order> SearchOrders(string term)
        {
            return _context.Orders
                .Where(o => 
                    o.Employee.EmployeeName.Contains(term)
                    || o.Customer.CustomerName.Contains(term)
                    )
                .ToList();
        }
    }
}
