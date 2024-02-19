using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SagaStateMachineWorkerService.Models;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SagaStateMachineWorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMassTransit(cfg =>
                    {
                        cfg.AddSagaStateMachine<OrderStateMachine, OrderStateInstance>().EntityFrameworkRepository(opt =>
                        {
                            opt.AddDbContext<OrderStateDbContext, OrderStateDbContext>((provider, builder) =>
                            {
                                builder.UseSqlServer(hostContext.Configuration.GetConnectionString("SqlCon"), m =>
                                {
                                    m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                                });
                            });
                        });

                        cfg.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(configure =>
                        {
                            configure.Host(hostContext.Configuration.GetConnectionString("RabbitMQ"));

                            //OrderState imi state machine da yeni bir instance olu�turacak k�s�m
                            configure.ReceiveEndpoint(RabbitMQSettingsConst.OrderSaga, e =>
                            {
                                e.ConfigureSaga<OrderStateInstance>(provider);
                            });

                        }));
                    });



                    services.AddHostedService<Worker>();
                });
    }
}
