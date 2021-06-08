using System.Collections.Generic;
using Platform.Common;
using Platform.Domain;

namespace PizzaPie.Domain
{
    public partial class Order
    {
        public class With : FluentBuilder<Order>
        {
            private readonly Order instance = new Order();

            public With(string customerName, PizzaSize size)
            {
                if (customerName.IsNullOrEmpty())
                {
                    AddDomainException("Customer name empty");
                }

                instance.CustomerName = customerName;
                instance.Size.Value = size;
            }

            public With Toppings(List<Topping> toppings)
            {
                instance.toppings.AddRange(toppings);
                return this;
            }
        }
    }
}