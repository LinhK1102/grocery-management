using BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IOrderDetailRepository
    {
        List<OrderDetail> GetAllOrderDetails();
        OrderDetail GetOrderDetailById(int id);
        List<OrderDetail> GetOrderDetailsByOrderId(int id);
        void CreateOrderDetail(OrderDetail od);
        void UpdateOrderDetail(OrderDetail od);
        void DeleteOrderDetail(int id);
    }
}
