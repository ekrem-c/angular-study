using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;

namespace Platform.CosmosDocumentProvider
{
    public interface ICosmosDocumentRepository<T>
    {
        Task<T> CreateAsync(T data);

        Task<T> ReadAsync(string id, string partitionKey = null);

        Task<bool> UpdateAsync(T data);

        Task<bool> DeleteAsync(string id, string partitionKey = null);

        Task<IEnumerable<T>> AllAsync();

        Task<IEnumerable<T>> QueryAsync(Expression<Func<T, bool>> predicate = null);

        Task<int> ReadCountAsync(Expression<Func<T, bool>> predicate = null);
    }
}