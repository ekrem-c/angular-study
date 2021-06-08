using MediatR;

namespace Platform.Domain
{
    public interface IDomainCommand<TResponse> : IRequest<TResponse>
    {
    }
}