using MediatR;

namespace Platform.Domain
{
    public interface IHandleDomainCommand<TDomainEvent, TResponse> : IRequestHandler<TDomainEvent, TResponse>
        where TDomainEvent : IDomainCommand<TResponse>
    {
    }
}