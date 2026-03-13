using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting; // PHẢI CÓ THƯ VIỆN NÀY

namespace Messaging.Common.Consuming
{
    // 1. Kế thừa BackgroundService để .NET có thể tự khởi chạy
    public abstract class BaseConsumer<T> : BackgroundService
    {
        protected readonly IModel _channel; // Đổi sang protected để lớp con thấy được
        protected readonly string _queue;

        protected BaseConsumer(IModel channel, string queue)
        {
            _channel = channel;
            _queue = queue;
        }

        // 2. BackgroundService yêu cầu thực thi hàm ExecuteAsync
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var message = JsonSerializer.Deserialize<T>(body);

                    // Gọi logic xử lý ở lớp con
                    await HandleMessage(message!, ea.BasicProperties?.CorrelationId ?? string.Empty);

                    _channel.BasicAck(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Error] Failed to process message: {ex.Message}");
                    _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);
                }
            };

            _channel.BasicConsume(queue: _queue, autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }

        protected abstract Task HandleMessage(T message, string correlationId);
    }
}