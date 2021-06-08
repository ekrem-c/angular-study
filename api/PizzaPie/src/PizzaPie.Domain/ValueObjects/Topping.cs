using Platform.Domain;

namespace PizzaPie.Domain
{
    public class Topping: DomainValueObject<Topping>
    {
        public string Name { get; protected internal set; }

        public int CookTime { get; protected internal set; } = 1;

        public Topping(string name)
        {
            Name = name;
        }

        protected override bool EqualsCore(Topping other)
        {
            return Name == other.Name;
        }

        protected override int GetHashCodeCore()
        {
            return GetHashCode();
        }
    }
}