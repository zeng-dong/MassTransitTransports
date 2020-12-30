using MassTransit;
using Messages;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;

namespace AzureServiceBusOrderService
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

            var azureServiceBus = Bus.Factory.CreateUsingAzureServiceBus(busFactoryConfig =>
            {
                busFactoryConfig.Message<Order>(configTopology => { configTopology.SetEntityName(ordersTopic); });

                busFactoryConfig.Host(asbcs, hostConfig =>
                {
                    hostConfig.TransportType = Microsoft.Azure.ServiceBus.TransportType.AmqpWebSockets;
                });
            });
            services.AddMassTransit(config => config.AddBus(provider => azureServiceBus));

            services.AddSingleton<IPublishEndpoint>(azureServiceBus);
            services.AddSingleton<ISendEndpointProvider>(azureServiceBus);
            services.AddSingleton<IBus>(azureServiceBus);            // optional

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Multiple APIs",
                    Description = "api to publish/send message to azure service bus",
                    Contact = new OpenApiContact
                    {
                        Name = "Multiple APIs",
                        Email = "someone@somewhere.net",
                        Url = new Uri("https://www.someone.org")
                    }
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order API V1");
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
