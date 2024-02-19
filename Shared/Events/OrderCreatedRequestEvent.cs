﻿using Shared.Interfaces;
using System.Collections.Generic;

namespace Shared.Events
{
    public class OrderCreatedRequestEvent : IOrderCreatedRequestEvent
    {
        public int OrderId { get; set; }
        public string BuyerId { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; }=new List<OrderItemMessage>();
        public PaymentMessage Payment { get; set; }
    }
}
