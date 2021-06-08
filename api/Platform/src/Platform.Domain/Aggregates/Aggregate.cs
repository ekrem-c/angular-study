using System.Collections.Generic;

namespace Platform.Domain
{
    public abstract class Aggregate : DomainAggregate
    {
        private readonly List<object> changes = new List<object>();
        
        protected void Apply(object evt)
        {
            changes.Add(evt);
            When(evt);
        }
        
        public void Load(object[] events)
        {
            foreach (var @event in events)
            {
                When(@event);
                Version++;
            }
        }

        protected abstract void When(object evt);
        
        public IReadOnlyCollection<object> Changes => changes.AsReadOnly();

        public void ClearChanges() => changes.Clear();
        
        public abstract string GetId();
        
        public int Version { get; set; } = -1;
    }
}