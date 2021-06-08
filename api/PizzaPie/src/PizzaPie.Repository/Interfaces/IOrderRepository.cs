using PizzaPie.Domain;
using Platform.CosmosDocumentProvider;

namespace PizzaPie.Repository
{
    public interface IOrderRepository : ICosmosDocumentRepository<Order>
    {
        
    }
}