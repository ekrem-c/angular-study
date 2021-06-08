using MediatR;

namespace Platform.Domain
{
    public interface IDomainQuery<TResponse> : IRequest<TResponse>
    {
    }
}