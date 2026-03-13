using Microsoft.AspNetCore.Mvc;
using ECommerce.Application.DTOs;
using ECommerce.Application.Services;
using Messaging.Common.Publishing;
using Messaging.Common.Events;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IMessagePublisher _messagePublisher;
        public OrdersController(IOrderService orderService, IMessagePublisher messagePublisher)
        {
            _orderService = orderService;
            _messagePublisher = messagePublisher;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDTO request)
        {
            try
            {
                int newOrderId = await _orderService.PlaceOrderAsync(request);
                var orderEvent = new OrderPlacedEvent
                {
                    OrderId = Guid.NewGuid(), // Hoặc ánh xạ từ newOrderId tùy logic của bạn
                    OrderNumber = $"ORD-{newOrderId}",
                    TotalAmount = 0
                    // Lưu ý: Nếu OrderPlacedEvent của bạn có trường Items, hãy map từ request.OrderItems sang
                };
                _messagePublisher.Publish(orderEvent, "order-exchange", "order.placed");

                return Ok(new
                {
                    Message = "Order placed successfully",
                    OrderId = newOrderId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }
    }
}