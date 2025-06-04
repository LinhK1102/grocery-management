using BusinessObjects;
using DataAccess.DAO;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly OrderDetailDAO _dao;
        public OrderDetailRepository(ApplicationDbContext ctx) => _dao = new OrderDetailDAO(ctx);

        public List<OrderDetail> GetAllOrderDetails() => _dao.GetAllOrderDetails();
        public OrderDetail GetOrderDetailById(int id) => _dao.GetOrderDetailById(id);
        public List<OrderDetail> GetOrderDetailsByOrderId(int id) => _dao.GetOrderDetailsByOrderId(id);
        public void CreateOrderDetail(OrderDetail od) => _dao.CreateOrderDetail(od);
        public void UpdateOrderDetail(OrderDetail od) => _dao.UpdateOrderDetail(od);
        public void DeleteOrderDetail(int id) => _dao.DeleteOrderDetail(id);
    }
}
