using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Platform.Domain
{
    public class DomainServiceBus : IDomainServiceBus
    {
        private readonly IMediator mediator;

        public DomainServiceBus(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            return mediator.Send(request, cancellationToken);
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
        {
            return mediator.Publish(notification, cancellationToken);
        }
    }
}