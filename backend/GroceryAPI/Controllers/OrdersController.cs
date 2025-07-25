using BusinessObjects.Entities;
using GroceryWebApp.Models.Dto;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
using Utility.Common; // nếu bạn dùng SystemStatus để return ApiResponse

namespace GroceryAPI.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet("get-all")]
        public IActionResult GetAllOrders()
        {
            var orders = _orderRepository.GetAllOrders();
            return Ok(SystemStatus.Success(orders, "All orders retrieved."));
        }

        [HttpGet("get-by-id/{id}")]
        public IActionResult GetOrderById(int id)
        {
            var order = _orderRepository.GetOrderById(id);
            return order == null
                ? NotFound(SystemStatus.Fail($"Order with ID {id} not found."))
                : Ok(SystemStatus.Success(order, "Order retrieved successfully."));
        }

        [HttpPost("create")]
        public IActionResult CreateOrder([FromBody] OrderUpdateDto dto)
        {
            var order = new Order
            {
                OrderId = dto.OrderId,
                OrderDate = dto.OrderDate,
                CustomerId = dto.CustomerId,
                EmployeeId = dto.EmployeeId,
                OutletId = dto.OutletId,
                WarehouseId = dto.WarehouseId,
                OrderDetails = dto.Items.Select(item => new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPriceAtTimeOfSale = item.UnitPrice,
                }).ToList()
            };
            _orderRepository.CreateOrder(order);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId },
                SystemStatus.Success(order, "Order created successfully."));
        }

        [HttpPut("update/{id}")]
        public IActionResult UpdateOrder(int id, [FromBody] Order order)
        {
            if (id != order.OrderId)
                return BadRequest(SystemStatus.Fail("Mismatched Order ID."));

            _orderRepository.UpdateOrder(order);
            return Ok(SystemStatus.Success(order, "Order updated successfully."));
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteOrder(int id)
        {
            _orderRepository.DeleteOrder(id);
            return Ok(SystemStatus.Success($"Order with ID {id} deleted successfully."));
        }

        [HttpGet("search")]
        public IActionResult SearchOrders([FromQuery] string keyword)
        {
            var results = _orderRepository.SearchOrders(keyword);
            return Ok(SystemStatus.Success(results, $"Orders matching '{keyword}' retrieved."));
        }
    }
}
