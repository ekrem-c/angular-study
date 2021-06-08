using System.Collections.Generic;
using Platform.Domain;

namespace PizzaPie.Domain
{
    public partial class PizzaStore
    {
        public class With : FluentBuilder<PizzaStore>
        {
            public With Orders(List<Order> orders)
            {
                orders.AddRange(orders);
                return this;
            }
        }
    }
}