
using Microsoft.AspNetCore.Mvc;
using Platform.Common;

namespace Platform.Domain
{
    public abstract class DomainController : ControllerBase
    {
        protected readonly IDomainServiceBus ServiceBus;
        
        protected DomainController()
        {
            ServiceBus = ServiceLocator.ServiceProvider.GetService<IDomainServiceBus>();;
        }
    }
}