using MassTransit;
using Messages;
using Serilog;
using System.Threading.Tasks;

namespace RabbitMqInventoryService
{
    internal class OrderConsumer : IConsumer<Order>
    {

        public async Task Consume(ConsumeContext<Order> context)
        {
            Log.Debug($"{context.Message.Name}");
        }
    }
}
