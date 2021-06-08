using System.Collections.Generic;
using Platform.Domain;

namespace PizzaPie.Domain
{
    public partial class PizzaStore : DomainAggregate
    {
        private readonly List<Order> orders = new List<Order>();

        public IReadOnlyList<Order> Orders => orders.AsReadOnly();
    }
}