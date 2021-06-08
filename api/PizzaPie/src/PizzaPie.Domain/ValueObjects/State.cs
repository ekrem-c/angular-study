using System;
using Platform.Domain;

namespace PizzaPie.Domain
{
    public class State : DomainValueObject<State>
    {
        public PizzaState Value { get; protected internal set; } = PizzaState.New;

        protected override bool EqualsCore(State other)
        {
            return Value == other.Value;
        }

        protected override int GetHashCodeCore()
        {
            return GetHashCode();
        }
    }
}