using MassTransit;
using Messages;
using System;
using System.Threading.Tasks;

namespace RabbitMqInventoryService
{
    internal class OrderConsumer : IConsumer<Order>
    {

        public async Task Consume(ConsumeContext<Order> context)
        {
            await Console.Out.WriteLineAsync($"{context.Message.Name}");
        }
    }
}
