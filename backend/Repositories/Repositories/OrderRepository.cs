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
        public Order GetOrderById(int id) => _dao.GetOrderById(id);
        public void CreateOrder(Order o) => _dao.CreateOrder(o);
        public void UpdateOrder(Order o) => _dao.UpdateOrder(o);
        public void DeleteOrder(int id) => _dao.DeleteOrder(id);
        public List<Order> SearchOrders(string term) => _dao.SearchOrders(term);
    }
}
