using PizzaPie.Domain;
using Platform.CosmosDocumentProvider;

namespace PizzaPie.Repository
{
    public class OrderRepository : CosmosDocumentRepository<Order>, IOrderRepository
    {
        public OrderRepository(CosmosDbSettings settings)
            : base(settings)
        {
        }
    }
}