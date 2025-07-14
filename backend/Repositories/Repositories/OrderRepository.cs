using BusinessObjects.Entities;
using DataAccess.DAO;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Common;

namespace Repositories.Repositories
{
    public class OrderRepository : IOrderRepository, ISeedableRepository
    {
        private readonly OrderDAO _dao;
        private readonly ICustomerRepository _cusRepo;
        private readonly IEmployeeRepository _employeeRepository;

        public OrderRepository(OrderDAO orderDAO, ICustomerRepository customerRepository, IEmployeeRepository employeeRepository)
        {
            _dao = orderDAO;
            _cusRepo = customerRepository;
            _employeeRepository = employeeRepository;
        }

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

        public async Task<bool> EnsureSeedDataAsync()
        {
            if (!GetAllOrders().Any())
            {
                // Create default orders if none exist

                return CreateOrder(new Order
                {
                    OrderDate = DateTime.Now,
                    CustomerId = (await _cusRepo.GetCustomerByNameAsync(UtitlityConstant.Customer_Default_Name)).CustomerId,
                    EmployeeId = (await _employeeRepository.GetEmployeeByEmployeeName(UtitlityConstant.Employee_Default_Name)).EmployeeId
                }) != null;

            }
            return true; // đã có dữ liệu, không cần thêm nữa
        }
    }
}
