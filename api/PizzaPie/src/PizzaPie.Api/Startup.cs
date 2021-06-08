using System;
using PizzaPie.Handlers;
using PizzaPie.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Api;
using Platform.Common;
using Platform.CosmosDocumentProvider;
using Platform.Domain;

namespace PizzaPie.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddRepositories(Configuration);
            services.AddServiceLocator();
            services.AddPlatformApi(Configuration, Environment, "Comestibles");

            services.AddCosmosDocumentDb(Configuration);

            services.AddDomainMessaging(new[]
            {
                AppDomain.CurrentDomain.Load("Comestibles.Messaging"),
                AppDomain.CurrentDomain.Load("Comestibles.Abstractions")
            });
            
            services.AddPlatformAutoMapper(new []
            {
                AppDomain.CurrentDomain.Load("Comestibles.Abstractions")
            });
            
            services.AddHandlers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment environment, IServiceProvider serviceProvider)
        {
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            app.UsePlatformApi(environment);
            app.UseServiceLocator(serviceProvider);
        }
    }
}