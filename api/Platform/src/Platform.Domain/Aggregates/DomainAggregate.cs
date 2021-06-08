using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Platform.Common;

namespace Platform.Domain
{
    [Serializable]
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public abstract class DomainAggregate
    {
        protected IDomainServiceBus ServiceBus;

        protected DomainAggregate()
        {
            ServiceBus = ServiceLocator.ServiceProvider.GetService<IDomainServiceBus>();
        }
        
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public virtual string PartitionKey => Id;
    }
}