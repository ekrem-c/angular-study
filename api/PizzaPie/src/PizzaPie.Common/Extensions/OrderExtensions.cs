using System;
using System.Linq;
using Aggregates = PizzaPie.Domain;
using Models = PizzaPie.Abstractions;

namespace PizzaPie.Common
{
    public static class OrderExtensions
    {
        public static Models.Order ToModel(this Aggregates.Order order)
        {
            return new Models.Order
            {
                Id = Guid.Parse(order.Id),
                CustomerName = order.CustomerName,
                Toppings = order.Toppings.Select(x => x.ToModel()).ToList(),
                Size = order.Size.Value,
                State = order.State.Value
            };
        }

        public static Aggregates.Order ToAggregate(this Models.Order order)
        {
            return new Aggregates.Order.With(order.CustomerName, order.Size)
                .Toppings(order.Toppings.Select(x => x.ToValueObject()).ToList())
                .Build();
        }
    }
}
