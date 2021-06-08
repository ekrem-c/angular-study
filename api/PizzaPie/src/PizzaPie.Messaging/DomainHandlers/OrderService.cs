
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PizzaPie.Domain;
using PizzaPie.Repository;
using Platform.Domain;

namespace PizzaPie.Handlers
{
    public class OrderService :
        IHandleDomainQuery<OrdersQuery, IEnumerable<Order>>,
        IHandleDomainCommand<OrderCreatedCommand, Guid>
    {
        private readonly IOrderRepository orderRepository;

        public OrderService(IOrderRepository repository)
        {
            orderRepository = repository;
        }

        public async Task<IEnumerable<Order>> Handle(OrdersQuery query, CancellationToken cancellationToken)
        {
            var result = await orderRepository.QueryAsync();
            return result;
        }
        
        public async Task<Guid> Handle(OrderCreatedCommand command, CancellationToken cancellationToken)
        {
            var result = await orderRepository.CreateAsync(command.Order);
            return Guid.Parse(result.Id);
        }
    }
}