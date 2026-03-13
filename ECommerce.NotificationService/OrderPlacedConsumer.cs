using Messaging.Common.Consuming;
using Messaging.Common.Events;
using RabbitMQ.Client;

namespace ECommerce.NotificationService
{
    // Chỉ định rõ loại tin nhắn là <OrderPlacedEvent>
    public class OrderPlacedConsumer : BaseConsumer<OrderPlacedEvent>
    {
        // Truyền channel và tên queue vào constructor của lớp cha
        public OrderPlacedConsumer(IModel channel)
    : base(channel, "notification-queue")
        {
            // 1. Khai báo Exchange trước (Nếu chưa có nó sẽ tạo mới)
            _channel.ExchangeDeclare(exchange: "order-exchange", type: ExchangeType.Direct, durable: true);

            // 2. Khai báo Queue
            _channel.QueueDeclare(queue: "notification-queue", durable: true, exclusive: false, autoDelete: false);

            // 3. Bây giờ mới thực hiện Bind (Sẽ không còn lỗi NOT_FOUND)
            _channel.QueueBind(queue: "notification-queue", exchange: "order-exchange", routingKey: "order.placed");
        }

        // Ghi đè hàm HandleMessage đúng theo cấu trúc Generic
        protected override Task HandleMessage(OrderPlacedEvent message, string correlationId)
        {
            Console.WriteLine($"[NOTIFIED] Nhận đơn hàng mới: {message.OrderNumber}");
            Console.WriteLine($"[PROCESS] Đang xử lý cho khách hàng ID: {message.UserId}");

            return Task.CompletedTask;
        }
    }
}