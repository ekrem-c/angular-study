using Inflector;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;
using Platform.Common;
using Platform.Data;
using Platform.Domain;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Platform.CosmosDocumentProvider
{
    public class CosmosDocumentRepository<T> : IPlatformRepository<T>
        where T : DomainAggregate
    {
        private readonly string databaseId;
        private readonly string collectionName;
        private readonly DocumentClient client;

        private readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new PrivateSetterCamelCasePropertyNamesContractResolver()
        };

        public CosmosDocumentRepository(CosmosDbSettings settings)
        {
            databaseId = settings.DatabaseId;
            Inflector.Inflector.SetDefaultCultureFunc = () => new CultureInfo("en");
            client = new DocumentClient(new Uri(settings.Endpoint), settings.Key);
            collectionName = typeof(T).ToString();
        }

        public async Task<T> ReadAsync(string id)
        {
            try
            {
                Document document = await client.ReadDocumentAsync(
                    UriFactory.CreateDocumentUri(databaseId, collectionName, id.ToString()),
                    new RequestOptions
                    {
                        // TODO: Improve this to read from a partition key provider.
                        PartitionKey = new PartitionKey(id.ToString())
                    }
                );
                return (T)(dynamic)document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return default;
                }

                throw;
            }
        }

        public async Task<IEnumerable<T>> QueryAsync(Expression<Func<T, bool>> predicate = null)
        {
            var orderedQueryable = client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(
                    databaseId,
                    collectionName),
                new FeedOptions
                {
                    JsonSerializerSettings = jsonSerializerSettings,
                    MaxItemCount = -1,
                    EnableCrossPartitionQuery = true
                });

            var documentQuery = predicate.IsNotNull()
                ? orderedQueryable.Where(predicate)
                : orderedQueryable;

            return await Task.Run(() => documentQuery.ToList());
        }

        public async Task<T> CreateAsync(T item)
        {
            var collection = UriFactory.CreateDocumentCollectionUri(databaseId, collectionName);
            var result = await client.CreateDocumentAsync(
                collection,
                item,
                new RequestOptions { JsonSerializerSettings = jsonSerializerSettings });
            return JsonConvert.DeserializeObject<T>(result.Resource.ToString(), jsonSerializerSettings);
        }

        public async Task<bool> UpdateAsync(T item)
        {
            var documentUri = UriFactory.CreateDocumentUri(databaseId, collectionName, item.Id.ToString());
            var result = await client.ReplaceDocumentAsync(documentUri, item);
            return result != null;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                await client.DeleteDocumentAsync(
                    UriFactory.CreateDocumentUri(databaseId, collectionName, id.ToString()),
                    // TODO: Improve this to read from a partition key provider.
                    new RequestOptions { PartitionKey = new PartitionKey(id.ToString()) });
                return true;
            }
            catch (Exception)
            {
                // TODO: Log exception
                return false;
            }
        }

        public async Task<IEnumerable<T>> AllAsync()
        {
            var orderedQueryable = client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(
                    databaseId,
                    collectionName),
                new FeedOptions
                {
                    JsonSerializerSettings = jsonSerializerSettings,
                    MaxItemCount = -1,
                    EnableCrossPartitionQuery = true
                });
            
            return await Task.Run(() => orderedQueryable.ToList());
        }

        public async Task<int> ReadCountAsync(Expression<Func<T, bool>> predicate = null)
        {
            var orderedQueryable = client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(
                    databaseId,
                    collectionName),
                new FeedOptions
                {
                    JsonSerializerSettings = jsonSerializerSettings,
                    MaxItemCount = -1,
                    EnableCrossPartitionQuery = true
                });

            var documentQuery = predicate.IsNotNull()
                ? orderedQueryable.Where(predicate)
                : orderedQueryable;

            return await documentQuery.CountAsync();
        }
    }
}