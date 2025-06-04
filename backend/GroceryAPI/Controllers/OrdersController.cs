using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;

namespace GroceryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet("GetAllOrders")]
        public IActionResult GetAllOrders() => Ok(_orderRepository.GetAllOrders());

        [HttpGet("GetOrderById/{id}")]
        public IActionResult GetOrderById(int id)
        {
            var order = _orderRepository.GetOrderById(id);
            return order == null ? NotFound() : Ok(order);
        }

        [HttpPost("CreateOrder")]
        public IActionResult CreateOrder([FromBody] Order order)
        {
            _orderRepository.CreateOrder(order);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId }, order);
        }

        [HttpPut("UpdateOrder/{id}")]
        public IActionResult UpdateOrder(int id, Order order)
        {
            if (id != order.OrderId) return BadRequest();
            _orderRepository.UpdateOrder(order);
            return NoContent();
        }

        [HttpDelete("DeleteOrder/{id}")]
        public IActionResult DeleteOrder(int id)
        {
            _orderRepository.DeleteOrder(id);
            return NoContent();
        }

        [HttpGet("search")]
        public IActionResult SearchOrders([FromQuery] string keyword)
        {
            return Ok(_orderRepository.SearchOrders(keyword));
        }
    }

}
