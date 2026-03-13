using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Common.Publishing
{
    public interface IMessagePublisher
    {
        void Publish<T>(T message, string exchangeName, string routingKey) where T : class;
    }
}
