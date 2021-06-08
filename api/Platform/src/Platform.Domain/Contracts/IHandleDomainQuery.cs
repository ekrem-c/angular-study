using MediatR;

namespace Platform.Domain
{
    public interface IHandleDomainQuery<TDomainQuery, TResponse> : IRequestHandler<TDomainQuery, TResponse>
        where TDomainQuery : IDomainQuery<TResponse>
    {
    }
}