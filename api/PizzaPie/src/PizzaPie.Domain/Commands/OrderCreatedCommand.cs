using System;
using Platform.Domain;

namespace PizzaPie.Domain
{
    public class OrderCreatedCommand : IDomainCommand<Guid>
    {
        public Order Order { get; init;  }
    }
}