using Platform.Domain;

namespace PizzaPie.Domain
{
    public class OrderSentToKitchenCommand : IDomainCommand<bool>
    {
        public Order Order { get; init;  }
    }
}