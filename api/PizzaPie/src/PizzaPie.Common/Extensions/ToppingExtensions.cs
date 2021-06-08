using Models = PizzaPie.Abstractions;
using ValueObjects = PizzaPie.Domain;

namespace PizzaPie.Common
{
    public static class ToppingExtensions
    {
        public static Models.Topping ToModel(this ValueObjects.Topping topping)
        {
            return new Models.Topping
            {
                Name = topping.Name
            };
        }
        
        public static ValueObjects.Topping ToValueObject(this Models.Topping topping)
        {
            return new ValueObjects.Topping(topping.Name);
        }
    }
}