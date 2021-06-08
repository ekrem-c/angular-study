using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Platform.CosmosDocumentProvider
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCosmosDocumentDb(this IServiceCollection services, IConfiguration configuration)
        {
            var cosmosDbSettings = new CosmosDbSettings();
            configuration.GetSection("CosmosDbSettings").Bind(cosmosDbSettings);
            services.AddSingleton(cosmosDbSettings);
        }
    }
}