using MassTransit;
using Messages;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AzureServiceBusInventoryService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var asbcs = Configuration["AzureServiceBusConnectionString"].ToString();
            string ordersTopic = "ticket-orders";
            string subscriptionName = "ticket-subscriber-01";
            string subscriptionNameNumberTwo = "ticket-subscriber-02";
            string queueName = "ticket-cancellation";

            services.AddControllers();
            services.AddMassTransit(config =>
            {
                config.AddConsumer<OrderConsumer>();
                config.AddConsumer<QueuedOrderConsumer>();
                config.AddConsumer<OrderConsumerNumberTwo>();

                config.AddBus(registrationContext => Bus.Factory.CreateUsingAzureServiceBus
                    (configurator =>
                    {
                        configurator.Host(asbcs);

                        configurator.Message<Order>(m => { m.SetEntityName(ordersTopic); });

                        configurator.SubscriptionEndpoint<Order>(subscriptionName, epc =>
                        {
                            epc.Consumer<OrderConsumer>(registrationContext);
                        });

                        configurator.ReceiveEndpoint(queueName, endpointConfigurator =>
                        {
                            endpointConfigurator.ConfigureConsumer<QueuedOrderConsumer>(registrationContext);
                            // as this is a queue, no need to subscribe to topics, so set this to false.  // not able to set this, queue consumer receives topic, what?                            
                            //endpointConfigurator.SubscribeMessageTopics = false;
                        });

                        configurator.SubscriptionEndpoint<Order>(subscriptionNameNumberTwo, epc =>
                        {
                            epc.Consumer<OrderConsumerNumberTwo>(registrationContext);
                        });
                    }));
            });

            //services.AddSingleton<IHostedService, BusHostedService>();
            services.AddMassTransitHostedService();                  // this works with CreateUsingRabbitMQ but not here CreateUsingAzureServiceBus
            // so I started the hosted service by the line above.
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
