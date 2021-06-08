using System;
using Platform.Domain;

namespace PizzaPie.Domain
{
    public class OrderByIdQuery : IDomainQuery<Order>
    {
        public Guid Id { get; set; }
    }
}