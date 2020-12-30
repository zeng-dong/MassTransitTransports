using MassTransit;
using Messages;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Threading.Tasks;

namespace AzureServiceBusOrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public OrdersController(IPublishEndpoint publishEndpoint, ISendEndpointProvider sendEndpointProvider)
        {
            _publishEndpoint = publishEndpoint;
            _sendEndpointProvider = sendEndpointProvider;
        }

        [HttpPost("publish-order-to-topic")]
        public async Task<ActionResult> Create([FromQuery] string name)
        {
            var order = new Order { Id = Guid.NewGuid(), Name = name };
            Log.Debug("Publishing Order to Topic, Order Id is " + order.Id);
            await _publishEndpoint.Publish<Order>(order);

            return Ok(order);
        }

        [HttpPost("send-order-to-queue")]
        public async Task<ActionResult> Cancel([FromQuery] string name)
        {
            var queueName = "sb://globoticketz.servicebus.windows.net/ticket-cancellation";
            //var queueName = "ticket-cancellation";
            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri(queueName));

            var order = new Order { Id = Guid.NewGuid(), Name = "Queueing Order=" + name };
            Log.Debug("Sending Order to Queue, Order Id is " + order.Id);
            await sendEndpoint.Send<Order>(order);

            return Ok(order);
        }
    }
}
