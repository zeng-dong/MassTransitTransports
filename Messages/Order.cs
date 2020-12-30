using System;

namespace Messages
{
    public class Order
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
