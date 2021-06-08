using System;
using System.Threading.Tasks;
using Platform.Common;

namespace PizzaPie.Domain
{
    public partial class Order
    {
        public Task<Guid> Create()
        {
            if (State.Value != PizzaState.New)
            {
                throw new DomainException("Pizza may only be created in the open state");
            }

            return ServiceBus.Send(new OrderCreatedCommand { Order = this});
        }
        
        public Task<bool> Cook()
        {
            if (State.Value != PizzaState.New)
            {
                throw new DomainException("Pizza must be in open state to start cooking");
            }

            State.Value = PizzaState.Cooking;

            return ServiceBus.Send(new OrderSentToKitchenCommand { Order = this });
        }
    }
}