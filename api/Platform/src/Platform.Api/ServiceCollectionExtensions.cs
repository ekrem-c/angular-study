using System;
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Platform.Api.Extensions;
using Platform.Api.Filters;
using Platform.Common;

namespace Platform.Api
{
    public static class ServiceCollectionExtensions
    {
        public static void AddPlatformApi(
            this IServiceCollection services,
            IConfiguration configuration,
            IWebHostEnvironment environment,
            string? serviceTitle = null,
            string? version = null)
        {
            services.AddSwaggerGen(c =>
            {
                if (File.Exists($"{AppContext.BaseDirectory}/SwaggerApi.xml"))
                {
                    c.IncludeXmlComments($"{AppContext.BaseDirectory}/SwaggerApi.xml");
                }

                c.SwaggerDoc("v1", new OpenApiInfo {Title = serviceTitle ?? "Service API", Version = version ?? "v1"});
                c.DocumentFilter<LowercaseDocumentFilter>();
            });
            
            services.AddHttpClient("HttpClientName", client =>
            {
            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();
                if (environment.IsDevelopment())
                {
                    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
                }

                return handler;
            });

            services.AddMvc(x =>
            {
                x.Filters.Add<ApiExceptionFilter>();
            });
            
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            };
        }

        public static void UsePlatformApi(this IApplicationBuilder app, IWebHostEnvironment environment)
        {
            App.ServiceProvider = app.ApplicationServices;
            app.UseMiddleware<ResponseTimeFilter>();

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Service API"); });

            app.UseForwardedHeaders();

            app.UseAuthentication();

            app.UseCors(builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .Build();
            });

            if (environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<GlobalExceptionMiddleWare>();
        }
    }
}