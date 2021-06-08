using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Platform.Data
{
    public abstract class PlatformRepository<T> : IPlatformRepository<T>
        where T : class
    {
        public abstract Task<T> CreateAsync(T data);

        public abstract Task<bool> DeleteAsync(Guid id);

        public abstract Task<T> ReadAsync(Guid id);

        public abstract Task<IEnumerable<T>> AllAsync();

        public abstract Task<IEnumerable<T>> QueryAsync(Expression<Func<T, bool>> predicate = null);

        public abstract Task<bool> UpdateAsync(T data);
    }
}
