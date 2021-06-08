using Platform.Domain;

namespace PizzaPie.Domain
{
    public class Driver: DomainValueObject<Driver>
    {
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;

        protected override bool EqualsCore(Driver other)
        {
            return FirstName == other.FirstName && LastName == other.LastName;
        }

        protected override int GetHashCodeCore()
        {
            return FirstName.GetHashCode() ^ LastName.GetHashCode();
        }
    }
}