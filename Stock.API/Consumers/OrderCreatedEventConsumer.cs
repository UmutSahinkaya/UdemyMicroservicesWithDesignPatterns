﻿using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Interfaces;
using Stock.API.Models;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<IOrderCreatedEvent>
    {
        private readonly AppDbContext _context;
        private ILogger<OrderCreatedEventConsumer> _logger;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderCreatedEventConsumer(ILogger<OrderCreatedEventConsumer> logger, AppDbContext context, IPublishEndpoint publishEndpoint, ISendEndpointProvider sendEndpointProvider)
        {
            _logger = logger;
            _context = context;
            _publishEndpoint = publishEndpoint;
            _sendEndpointProvider = sendEndpointProvider;
        }
        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var stockResult = new List<bool>();
            foreach (var item in context.Message.OrderItems)
            {
                stockResult.Add(await _context.Stocks.AnyAsync(x => x.ProductId == item.ProductId && x.Count > item.Count));
            }

            if (stockResult.All(x => x.Equals(true)))
            {
                foreach (var item in context.Message.OrderItems)
                {
                    var stock = await _context.Stocks.FirstOrDefaultAsync(x => x.ProductId == item.ProductId);
                    if (stock != null)
                        stock.Count -= item.Count;
                    await _context.SaveChangesAsync();
                }
                _logger.LogInformation($"Stock was reserved for correlation Id:{context.Message.CorrelationId}");

                StockReservedEvent stockReservedEvent = new StockReservedEvent(context.Message.CorrelationId)
                {
                    OrderItems = context.Message.OrderItems,
                };

                await _publishEndpoint.Publish(stockReservedEvent);
            }
            else
            {
                await _publishEndpoint.Publish(new StockNotReservedEvent
                {
                    Reason="Not enough stock"
                });

                _logger.LogInformation($"Not enough stock for correlation Id:{context.Message.CorrelationId}");

            }

        }
    }
}
