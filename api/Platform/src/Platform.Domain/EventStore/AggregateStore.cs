using System;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace Platform.Domain
{
    public class AggregateStore : IAggregateStore
    {
        readonly IEventStoreConnection connection;
        
        public AggregateStore(IEventStoreConnection connection) => this.connection = connection;

        public async Task Store<T>(T entity) where T : Aggregate {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var streamName = GetStreamName<T>(entity.GetId());
            var changes = entity.Changes
                .Select((x, i) => new StreamEvent(x, new Metadata()))
                .ToArray();

            await connection.AppendEvents(streamName, entity.Version, changes);

            entity.ClearChanges();
        }

        public async Task<T> Load<T>(string id) where T : Aggregate {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            var stream = GetStreamName<T>(id);
            var entity = (T) Activator.CreateInstance(typeof(T), true);

            var page = await connection.ReadStreamEventsForwardAsync(
                stream, 0, 1024, false);

            entity.Load(page.Events.Select(resolvedEvent => resolvedEvent.Deserialize().Event).ToArray());

            return entity;
        }
        
        static string GetStreamName<T>(string entityId)
            => $"{typeof(T).Name}-{entityId}";
    }
}
