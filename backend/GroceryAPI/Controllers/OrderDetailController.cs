using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;

namespace GroceryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderDetailController : ControllerBase
    {
        private readonly IOrderDetailRepository _orderDetailRepository;

        public OrderDetailController(IOrderDetailRepository repo) => _orderDetailRepository = repo;

        [HttpGet("GetOrderDetailsByOrderId/{orderId}")]
        public IActionResult GetOrderDetailsByOrderId(int orderId) => Ok(_orderDetailRepository.GetOrderDetailsByOrderId(orderId));

        [HttpPost("CreateOrderDetail")]
        public IActionResult CreateOrderDetail([FromBody] OrderDetail od)
        {
            _orderDetailRepository.CreateOrderDetail(od);
            return Ok();
        }

        [HttpPut("UpdateOrderDetail/{id}")]
        public IActionResult UpdateOrderDetail(int id, OrderDetail od)
        {
            if (id != od.OrderDetailId) return BadRequest();
            _orderDetailRepository.UpdateOrderDetail(od);
            return NoContent();
        }

        [HttpDelete("DeleteOrderDetail/{id}")]
        public IActionResult DeleteOrderDetail(int id)
        {
            _orderDetailRepository.DeleteOrderDetail(id);
            return NoContent();
        }
    }

}
