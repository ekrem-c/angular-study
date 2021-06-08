using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Platform.Data
{
    public interface IPlatformRepository<T>
        where T : class
    {
        Task<T> CreateAsync(T data);

        Task<T> ReadAsync(string id);

        Task<bool> UpdateAsync(T data);

        Task<bool> DeleteAsync(string id);

        Task<IEnumerable<T>> QueryAsync(Expression<Func<T, bool>> predicate = null);
    }
}
