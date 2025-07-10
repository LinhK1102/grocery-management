using BusinessObjects.Entities;
using DataAccess.DAO;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDAO _dao;
        public OrderRepository(ApplicationDbContext ctx) => _dao = new OrderDAO(ctx);

        public List<Order> GetAllOrders() => _dao.GetAllOrders();
        public Order? GetOrderById(int id)
        {
            return _dao.GetOrderById(id);
        }

        public Order CreateOrder(Order order)
        {
            return _dao.CreateOrder(order);
        }

        public Order UpdateOrder(Order order)
        {
            return _dao.UpdateOrder(order);
        }

        public bool DeleteOrder(int id)
        {
            return _dao.DeleteOrder(id);
        }
        public List<Order> SearchOrders(string term) => _dao.SearchOrders(term);
    }
}
