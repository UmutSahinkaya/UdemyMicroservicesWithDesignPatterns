using MassTransit;
using Shared.Interfaces;
using System.Threading.Tasks;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<IOrderCreatedEvent>
    {
        //private readonly
        public Task Consume(ConsumeContext<IOrderCreatedEvent> context)
        {
            throw new System.NotImplementedException();
        }
    }
}
