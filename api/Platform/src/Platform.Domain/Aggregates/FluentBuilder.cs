using System.Collections.Generic;
using System.Text;
using Platform.Common;

namespace Platform.Domain
{
    public abstract class FluentBuilder<T> 
        where T : class, new()
    {
        protected readonly List<string> DomainExceptions = new List<string>();
        protected readonly T Instance;

        protected FluentBuilder()
        {
            Instance = new T();
        }

        protected void AddDomainException(string exceptionMessage)
        {
            DomainExceptions.Add(exceptionMessage);
        }

        public T Build()
        {
            Validate();
            return Instance;
        }

        private void Validate()
        {
            if (DomainExceptions.IsNullOrEmpty())
            {
                return;
            }

            StringBuilder excpetionMessage = new StringBuilder();
            DomainExceptions.ForEachItem(x =>
            {
                excpetionMessage.AppendLine(x);
            });

            throw new DomainException(excpetionMessage.ToString());
        }
    }
}