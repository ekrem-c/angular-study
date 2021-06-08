using System;
using System.Threading.Tasks;

namespace PizzaPie.Domain
{
    /// <summary>
    /// In Domain-driven Design, all operations that occur on any of the composed
    /// aggregates must go through the root aggregate, which is PizzaStore.
    /// </summary>
    public partial class PizzaStore
    {
        public async Task<Guid> CreateOrder(Order order)
        {
            return await ServiceBus.Send(new OrderCreatedCommand { Order = order });
        }

        public async Task<bool> CookPizza(Order order)
        {
            // HINT: This should call one of the methods of the Order aggregate
            throw new NotImplementedException("Add logic for saving the state of the pizza is in the oven.");
        }

        public async Task<bool> DispatchPizza(Order order)
        {
            throw new NotImplementedException("Add logic and saving of the driver for the order.");
        }

        public async Task<bool> CompleteOrder(Order order)
        {
            throw new NotImplementedException("Add logic to save the state of the order as complete.");
        }
    }
}