using MassTransit;
using Messages;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace OrderServiceBareBones
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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

            var bus = Bus.Factory.CreateUsingRabbitMq(config =>
            {
                config.Host("amqp://guest:guest@localhost:5672");

                config.ReceiveEndpoint("zd-barebones-queue", c =>
                {
                    c.Handler<Order>(ctx =>
                    {
                        return Console.Out.WriteLineAsync(ctx.Message.Name);
                    });
                });
            });

            bus.Start();

            bus.Publish(new Order { Id = Guid.NewGuid(), Name = "first order", Timestamp = DateTime.UtcNow });
        }
    }
}
