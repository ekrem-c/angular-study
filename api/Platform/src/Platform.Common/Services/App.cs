using System;
using System.ComponentModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Platform.Common
{
    public static class App
    {
        public static IServiceProvider ServiceProvider { get; set; }

        public static IConfiguration Configuration { get; set; }

        public static string RootPath { get; set; }

        public static Deployment GetDeploymentInformation()
        {
            return (ServiceProvider.GetService(typeof(IOptions<Deployment>)) as IOptions<Deployment>)?.Value;
        }

        public static string GetDirectoryWithRoot(string partialPath)
        {
            return $"{RootPath}/{partialPath}";
        }

        public static void EnsureSetting()
        {
            if (ServiceProvider == null)
            {
                throw new Exception("The \'App\' class has not been configured appropriately. The service provider property is missing.");

            }

            EnsureSettingOnConfigure();
        }

        public static void EnsureSettingOnConfigure()
        {
            if (Configuration == null)
            {
                throw new Exception("The \'App\' class has not been configured appropriately. The configuration app property is missing.");
            }
        }

        public static string GetServiceBaseUrl(Service service, bool local = false)
        {
            var deploymentInformation = GetDeploymentInformation();

            var serviceName = service.GetDescription() ?? service.ToString();

            return local
                ? $"http://{serviceName}/"
                : $"https://{serviceName}.{deploymentInformation.Namespace}.{deploymentInformation.Environment ?? "sandbox"}.societal/";

            // todo: add Api Gateway - public url.
        }
    }

    public enum Service
    {
        [Description("pizza-pie")]
        PizzaPie
    }
}