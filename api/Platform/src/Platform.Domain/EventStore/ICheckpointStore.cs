using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace Platform.Domain
{
    public interface ICheckpointStore
    {
        Task<Position> GetCheckpoint();
        Task StoreCheckpoint(Position checkpoint);
    }
}