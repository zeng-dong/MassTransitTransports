using MassTransit;
using Messages;
using Serilog;
using System.Threading.Tasks;

namespace AzureServiceBusInventoryService
{
    public class OrderConsumer : IConsumer<Order>
    {
        public async Task Consume(ConsumeContext<Order> context)
        {
            Log.Debug($"OrderConsumer-One: consuming: {context.Message.Name}");
        }
    }

    public class OrderConsumerNumberTwo : IConsumer<Order>
    {
        public async Task Consume(ConsumeContext<Order> context)
        {
            Log.Debug($"OrderConsumer-Two: consuming: {context.Message.Name}");
        }
    }

    public class QueuedOrderConsumer : IConsumer<Order>
    {
        public async Task Consume(ConsumeContext<Order> context)
        {
            Log.Debug($"QueuedOrderConsumer: consuming from queue: {context.Message.Name}");
        }
    }
}
