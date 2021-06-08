using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PizzaPie.Common;
using Platform.Domain;
using Domain = PizzaPie.Domain;
using Models = PizzaPie.Abstractions;

namespace Comestibles.Api
{
    [Route("[controller]")]
    public partial class OrdersController : AuthDomainController
    {
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var domainQuery = new Domain.OrdersQuery();
            var result = await ServiceBus.Send(domainQuery);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var domainQuery = new Domain.OrderByIdQuery
            {
                Id = id
            };
            
            var result = await ServiceBus.Send(domainQuery);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Models.Order entry)
        {
            var aggregate = entry.ToAggregate();
            return Ok(await aggregate.Create());
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody]Models.Order entry)
        {
            throw new NotImplementedException();
        }

        private async Task<Domain.Order> EntryAggregate(Guid entryId)
        {
            return await ServiceBus.Send(new Domain.OrderByIdQuery
            {
                Id = entryId
            });
        }    
    }
}