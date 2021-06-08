using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Platform.Domain
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDomainMessaging(this IServiceCollection services, Assembly[] mediatorAssemblies)
        {
            services.AddMediatR(mediatorAssemblies);
            services.AddSingleton<IDomainServiceBus, DomainServiceBus>();
        }
    }
}