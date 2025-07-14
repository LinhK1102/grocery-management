using BusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IOrderDetailRepository : ISeedableRepository
    {
        List<OrderDetail> GetAllOrderDetails();
        OrderDetail GetOrderDetailById(int id);
        List<OrderDetail> GetOrderDetailsByOrderId(int id);
        OrderDetail CreateOrderDetail(OrderDetail od);
        OrderDetail UpdateOrderDetail(OrderDetail od);
        bool DeleteOrderDetail(int id);
    }
}
