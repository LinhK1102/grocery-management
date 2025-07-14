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
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly OrderDetailDAO _dao;
        private readonly IOrderRepository _orderRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly IEmployeeRepository _employeeRepo;
        private readonly IProductRepository _productRepo;

        public OrderDetailRepository(OrderDetailDAO orderDetailDAO, IOrderRepository orderRepository,
            ICustomerRepository customerRepo, IEmployeeRepository employeeRepo, IProductRepository productRepo)
        {
            _dao = orderDetailDAO;
            _orderRepo = orderRepository;
            _customerRepo = customerRepo;
            _employeeRepo = employeeRepo;
            _productRepo = productRepo;
        }

        public List<OrderDetail> GetAllOrderDetails() => _dao.GetAllOrderDetails();
        public OrderDetail GetOrderDetailById(int id) => _dao.GetOrderDetailById(id);
        public List<OrderDetail> GetOrderDetailsByOrderId(int id) => _dao.GetOrderDetailsByOrderId(id);
        public OrderDetail CreateOrderDetail(OrderDetail od) => _dao.CreateOrderDetail(od);
        public OrderDetail UpdateOrderDetail(OrderDetail od) => _dao.UpdateOrderDetail(od);
        public bool DeleteOrderDetail(int id) => _dao.DeleteOrderDetail(id);

        public async Task<bool> EnsureSeedDataAsync()
        {
            if (!GetAllOrderDetails().Any())
            {

                // Thêm dữ liệu mẫu nếu chưa có
                if (_orderRepo.GetAllOrders().Count == 0)
                {
                    _orderRepo.EnsureSeedDataAsync().Wait(); // Đảm bảo có ít nhất một đơn hàng
                }
                var order = _orderRepo.GetAllOrders().FirstOrDefault();
                var products = await _productRepo.GetAllProduct(); // lấy ra List<Product>
                var firstProduct = products.FirstOrDefault(); // dùng như bình thường với List

                return CreateOrderDetail(new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = firstProduct?.ProductId ?? 0, // Giả sử có sản phẩm với ID 1
                    Quantity = 1,
                    UnitPriceAtTimeOfSale = 10.0m, // Giá đơn vị mẫu
                    DiscountApplied = 0.0m // Giảm giá mẫu
                }) != null;
            }
            return true;
        }
    }
}
