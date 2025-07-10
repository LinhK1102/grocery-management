using BusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
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

        public List<Order?> GetAllOrders()
        {
            var list = _context.Orders
                .ToList();
            return list ?? new List<Order>();
        }
        public Order GetOrderById(int id) => _context.Orders.Find(id);
        public Order CreateOrder(Order o) { _context.Orders.Add(o); _context.SaveChanges(); return o; }
        public Order UpdateOrder(Order o) { _context.Orders.Update(o); _context.SaveChanges(); return o; }
        public bool DeleteOrder(int id)
        {
            var o = _context.Orders.Find(id);
            if (o != null) { _context.Orders.Remove(o); _context.SaveChanges(); return true; }
            return false;
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
