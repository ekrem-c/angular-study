using Platform.Domain;

namespace PizzaPie.Domain
{
    public class Size: DomainValueObject<Size>
    {
        public PizzaSize Value { get; protected internal set; }

        protected override bool EqualsCore(Size other)
        {
            return Value == other.Value;
        }

        protected override int GetHashCodeCore()
        {
            return GetHashCode();
        }
    }
}