using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Messaging.Common.Publishing
{
    public class RabbitMqMessagePublisher : IMessagePublisher
    {
        private readonly IModel _channel;

        public RabbitMqMessagePublisher(IModel channel)
        {
            _channel = channel;
        }

        public void Publish<T>(T message, string exchangeName, string routingKey) where T : class
        {
            // Chuyển Object thành chuỗi JSON và mảng byte
            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);

            // Gửi tin nhắn lên Exchange cụ thể
            _channel.BasicPublish(exchange: exchangeName,
                                 routingKey: routingKey,
                                 basicProperties: null,
                                 body: body);
        }
    }
}
