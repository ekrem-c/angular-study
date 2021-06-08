using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Platform.Common
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection Remove<T>(this IServiceCollection services)
        {
            var serviceDescriptor = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(T));
            if (serviceDescriptor != null)
            {
                services.Remove(serviceDescriptor);
            }

            return services;
        }

        public static void AddServiceLocator(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<IServiceProviderProxy, HttpContextServiceProviderProxy>();
        }

        public static void UseServiceLocator(this IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            ServiceLocator.Initialize(serviceProvider.GetService<IServiceProviderProxy>());
        }

        public static void AddPlatformAutoMapper(this IServiceCollection services, Assembly[] assemblies)
        {
            var mappingProfile = new MappingProfile();
            
            assemblies.ForEachItem(assembly =>
            {
                var types = assembly.GetExportedTypes()
                    .Where(t => t.GetInterfaces().Any(i => 
                        i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
                    .ToList();

                foreach (var type in types)
                {
                    var instance = Activator.CreateInstance(type);

                    var methodInfo = type.GetMethod("Mapping") 
                                     ?? type.GetInterface("IMapFrom`1").GetMethod("Mapping");
                
                    methodInfo?.Invoke(instance, new object[] { mappingProfile });
                }
            });

            services.AddAutoMapper(assemblies);
        }
    }
}