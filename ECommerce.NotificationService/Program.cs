using ECommerce.NotificationService;
using Messaging.Common.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        // 1. Kết nối tới RabbitMQ Server (đã viết ở Messaging.Common)
        services.AddRabbitMq("localhost", "guest", "guest", "/");

        // 2. Đăng ký Consumer của bạn
        services.AddHostedService<OrderPlacedConsumer>();
    })
    .Build();

await host.RunAsync();