using AutoMapper;

namespace Platform.Common
{
    public static class ObjectExtensions
    {
        /// <summary>
        ///     Determines whether [is not null] [the specified source].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>A value indicating if the object is null or not.</returns>
        // TODO: 1) This method tests whether an object is null - this could include a non-nullable object (decimal) which makes little sense
        // TODO: 2) ReSharper does not recognise this as defensive coding against nulls, so will throw false positive warnings in subsequent code for "this could be null".
        //          If the code tests for null (== null, or != null) then ReSharper does not warn.
        public static bool IsNotNull(this object source)
        {
            return source != null;
        }

        /// <summary>
        ///     Determines whether [is not null] [the specified source].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>A value indicating if the object is null or not.</returns>
        // TODO: 1) This method tests whether an object is null - this could include a non-nullable object (decimal) which makes little sense
        // TODO: 2) ReSharper does not recognise this as defensive coding against nulls, so will throw false positive warnings in subsequent code for "this could be null".
        //          If the code tests for null (== null, or != null) then ReSharper does not warn.
        public static bool IsNull(this object source)
        {
            return source == null;
        }

        public static T MapTo<T>(this object source)
        {
            var mapper = ServiceLocator.ServiceProvider.GetService<IMapper>();
            return mapper.Map<T>(source);
        }
        
        public static TDestination Map<TSource, TDestination>(this TSource source)
        {
            var mapper = ServiceLocator.ServiceProvider.GetService<IMapper>();
            return mapper.Map<TSource, TDestination>(source);
        }
    }
}