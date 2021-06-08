using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Platform.Data
{
    public class IgnorableSerializerContractResolver : DefaultContractResolver
    {
        protected readonly Dictionary<Type, HashSet<string>> Ignores;

        public IgnorableSerializerContractResolver()
        {
            Ignores = new Dictionary<Type, HashSet<string>>();
        }

        public void Ignore(Type type, params string[] propertyName)
        {
            if (!Ignores.ContainsKey(type))
            {
                Ignores[type] = new HashSet<string>();
            }

            foreach (var prop in propertyName)
            {
                Ignores[type].Add(prop);
            }
        }

        public bool IsIgnored(Type type, string propertyName)
        {
            if (!Ignores.ContainsKey(type))
            {
                return false;
            }

            // If no properties provided, ignore the type entirely.
            if (Ignores[type].Count == 0)
            {
                return true;
            }

            return Ignores[type].Contains(propertyName);
        }

        public IgnorableSerializerContractResolver Ignore<TModel>(Expression<Func<TModel, object>> selector)
        {
            var body = selector.Body as MemberExpression;

            if (body == null)
            {
                var ubody = (UnaryExpression) selector.Body;
                body = ubody.Operand as MemberExpression;

                if (body == null)
                {
                    throw new ArgumentException("Could not get property name", "selector");
                }
            }

            var propertyName = body.Member.Name;
            Ignore(typeof(TModel), propertyName);
            return this;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (IsIgnored(property.DeclaringType, property.PropertyName)
                || IsIgnored(property.DeclaringType.BaseType, property.PropertyName))
            {
                property.ShouldSerialize = instance => false;
            }

            return property;
        }
    }
}