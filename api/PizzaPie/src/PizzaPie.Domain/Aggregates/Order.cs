using System.Collections.Generic;
using Platform.Domain;

namespace PizzaPie.Domain
{
    public partial class Order : DomainAggregate
    {
        private readonly List<Topping> toppings = new List<Topping>();
        
        public string CustomerName { get; protected internal set; }
        
        public Size Size { get; protected internal set; }

        public State State { get; protected internal set; }
        
        public Driver Driver { get; protected internal set; }

        public IReadOnlyList<Topping> Toppings => toppings.AsReadOnly();
    }
}