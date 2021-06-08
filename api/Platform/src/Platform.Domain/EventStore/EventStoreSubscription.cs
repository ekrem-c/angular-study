using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace Platform.Domain
{
    public class AllStreamSubscription
    {
        readonly string name;
        readonly AllStreamCheckpointStore checkpointStore;
        readonly List<HandleEvent> handlers = new List<HandleEvent>();
        
        public AllStreamSubscription(string name, AllStreamCheckpointStore checkpointStore)
        {
            this.name = name;
            this.checkpointStore = checkpointStore;
        }

        public AllStreamSubscription AddHandler(HandleEvent handleEvent)
        {
            handlers.Add(handleEvent);
            return this;
        }

        public async Task<EventStoreCatchUpSubscription> Subscribe(IEventStoreConnection connection)
        {
            var settings = new CatchUpSubscriptionSettings(10000, 500, false, true, name);
            var position = await checkpointStore.Load(name);
            return connection.SubscribeToAllFrom(position, settings, EventAppeared);
            
            async Task EventAppeared(EventStoreCatchUpSubscription _, ResolvedEvent resolvedEvent)
            {
                if (resolvedEvent.IsSystemEvent()) return;

                try
                {
                    var streamEvent = resolvedEvent.Deserialize();
                    var subscription = (EventStoreAllCatchUpSubscription) _;
                    
                    await Task.WhenAll(handlers.Select(x => x(streamEvent)));
                    
                    await checkpointStore.Store(name, subscription.LastProcessedPosition);
                }
                catch (Exception e)
                {
                    // Logger.Error(e, "Error occured while handling {@event}", resolvedEvent);
                }
            }
        }
    }
}
