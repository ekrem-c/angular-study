using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace Platform.Domain
{
    public delegate Task HandleEvent(StreamEvent evt);

    public delegate Task<EventStoreCatchUpSubscription> Subscribe(IEventStoreConnection connection);
}