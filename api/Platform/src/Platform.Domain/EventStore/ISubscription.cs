using EventStore.ClientAPI;

namespace Platform.Domain
{
    public interface ISubscription
    {
        void Subscribe(IEventStoreConnection connection);
    }
}
