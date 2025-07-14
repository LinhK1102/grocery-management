using BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class OrderDetailDAO
    {
        private readonly ApplicationDbContext _context;
        public OrderDetailDAO(ApplicationDbContext context) => _context = context;

        public List<OrderDetail> GetAllOrderDetails() => _context.OrderDetails.ToList();
        public List<OrderDetail> GetOrderDetailsByOrderId(int id)
            => _context.OrderDetails.Where(od => od.OrderId == id).ToList();
        public OrderDetail GetOrderDetailById(int id) => _context.OrderDetails.Find(id);
        public OrderDetail CreateOrderDetail(OrderDetail od)
        {
            try
            {
                _context.OrderDetails.Add(od);
                _context.SaveChanges();
                return od;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu cần
                throw new Exception("Error creating OrderDetail: " + ex.Message);
            }
        }
        public OrderDetail UpdateOrderDetail(OrderDetail od)
        {
            try
            {
                _context.OrderDetails.Update(od);
                _context.SaveChanges();
                return od;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu cần
                throw new Exception("Error updating OrderDetail: " + ex.Message);
            }
        }
        public bool DeleteOrderDetail(int id)
        {
            try
            {
                var od = _context.OrderDetails.Find(id);
                if (od != null) { _context.OrderDetails.Remove(od); _context.SaveChanges(); return true; }
                return false;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu cần
                throw new Exception("Error deleting OrderDetail: " + ex.Message);
            }
        }
    }
}
