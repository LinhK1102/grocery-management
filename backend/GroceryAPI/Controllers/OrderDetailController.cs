using BusinessObjects.Entities;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
using Utility.Common;

namespace GroceryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderDetailController : ControllerBase
    {
        private readonly IOrderDetailRepository _orderDetailRepository;

        public OrderDetailController(IOrderDetailRepository repo)
        {
            _orderDetailRepository = repo;
        }

        [HttpGet("get-all-by-order-id/{orderId}")]
        public IActionResult GetAllByOrderId(int orderId)
        {
            try
            {
                var details = _orderDetailRepository.GetOrderDetailsByOrderId(orderId);
                if (details == null || !details.Any())
                    return NotFound(SystemStatus.Fail($"No order details found for Order ID {orderId}."));

                return Ok(SystemStatus.Success(details, $"Order details for Order ID {orderId} retrieved."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, SystemStatus.Fail($"An error occurred: {ex.Message}"));
            }
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] OrderDetail od)
        {
            try
            {
                _orderDetailRepository.CreateOrderDetail(od);
                return Ok(SystemStatus.Success(od, "Order detail created successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, SystemStatus.Fail($"Failed to create order detail: {ex.Message}"));
            }
        }

        [HttpPut("update/{id}")]
        public IActionResult Update(int id, [FromBody] OrderDetail od)
        {
            if (id != od.OrderDetailId)
                return BadRequest(SystemStatus.Fail("Mismatched OrderDetail ID."));

            try
            {
                _orderDetailRepository.UpdateOrderDetail(od);
                return Ok(SystemStatus.Success(od, "Order detail updated successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, SystemStatus.Fail($"Failed to update order detail: {ex.Message}"));
            }
        }

        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _orderDetailRepository.DeleteOrderDetail(id);
                return Ok(SystemStatus.Success($"Order detail with ID {id} deleted successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, SystemStatus.Fail($"Failed to delete order detail: {ex.Message}"));
            }
        }
    }


}
