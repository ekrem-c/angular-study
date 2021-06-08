using System;
using System.Linq;
using System.Reflection;

namespace Platform.Common
{
    /// <summary>
    ///     Extension methods for Generics.
    /// </summary>
    public static class GenericExtensions
    {
        /// <summary>
        ///     Iterates through all the properties and set the values back to their default values.
        /// </summary>
        /// <typeparam name="T">The object whose property values will be reset.</typeparam>
        /// <param name="source">The object whose properties are to be reset.</param>
        public static void ResetProperties<T>(this T source) where T : class
        {
            const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            var typeIn = source.GetType();

            typeIn.GetProperties(Flags)
                .Where(p => p.CanWrite)
                .ForEachItem(p => p.SetValue(source, default(T)));
        }

        /// <summary>
        ///     Copies properties with identical names from the source to the target.
        /// </summary>
        /// <typeparam name="TTarget">The object whose properties will be updated.</typeparam>
        /// <typeparam name="TSource">The object whose property values will be taken.</typeparam>
        /// <param name="target">Defines the generic type of the target.</param>
        /// <param name="source">Defines the generic type of the source.</param>
        public static void CopyPropertiesFrom<TTarget, TSource>(this TTarget target, TSource source)
        {
            CopyProps(source, target);
        }

        /// <summary>
        ///     Copies properties with identical names to the target from the source..
        /// </summary>
        /// <typeparam name="TSource">The object whose property values will be taken.</typeparam>
        /// <typeparam name="TTarget">The object whose properties will be updated.</typeparam>
        /// <param name="source">Defines the generic type of the source.</param>
        /// <param name="target">Defines the generic type of the target.</param>
        public static void CopyPropertiesTo<TSource, TTarget>(this TSource source, TTarget target)
        {
            CopyProps(source, target);
        }

        /// <summary>
        ///     Copies properties with identical names from source to output.
        /// </summary>
        /// <typeparam name="TOut">The output object.</typeparam>
        /// <param name="source">The object from whose properties are to be read.</param>
        /// <returns>A new instance of the object whose properties are to be written.</returns>
        public static TOut CopyPropertiesToNew<TOut>(this object source)
        {
            var target = (TOut) Activator.CreateInstance(typeof(TOut));
            CopyProps(source, target);
            return target;
        }

        /// <summary>
        ///     Copies properties with identical names to the target from the source..
        /// </summary>
        /// <typeparam name="TSource">The object whose property values will be taken.</typeparam>
        /// <typeparam name="TTarget">The object whose properties will be updated.</typeparam>
        /// <param name="source">Defines the generic type of the source.</param>
        /// <param name="target">Defines the generic type of the target.</param>
        private static void CopyProps<TSource, TTarget>(TSource source, TTarget target)
        {
            const BindingFlags BasicFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            const BindingFlags TargetFlags = BindingFlags.GetProperty | BasicFlags;
            const BindingFlags SourceFlags = BindingFlags.SetProperty | BasicFlags;

            var sourceType = source.GetType();
            var targetType = target.GetType();

            sourceType
                .GetProperties(SourceFlags)
                .Where(p => p.CanRead)
                .ForEachItem(p =>
                {
                    var info = targetType.GetProperty(p.Name, TargetFlags);

                    if (info != null && info.CanWrite) info.SetValue(target, p.GetValue(source, null), null);
                });
        }
    }
}