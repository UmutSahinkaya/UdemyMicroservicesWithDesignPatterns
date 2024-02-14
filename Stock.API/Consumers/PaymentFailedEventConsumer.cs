using MassTransit;
using Microsoft.Extensions.Logging;
using Shared;
using Stock.API.Models;
using System.Threading.Tasks;

namespace Stock.API.Consumers
{
    public class PaymentFailedEventConsumer /*: IConsumer<PaymentFailedEvent>*/
    {
        private readonly AppDbContext _context;
        private ILogger<PaymentFailedEventConsumer> _logger;

        public PaymentFailedEventConsumer(AppDbContext context, ILogger<PaymentFailedEventConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        //public Task Consume(ConsumeContext<PaymentFailedEvent> context)
        //{
            
        //}
    }
}
