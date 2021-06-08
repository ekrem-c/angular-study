// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Linq.Expressions;
// using System.Reflection;
// using Platform.Common.Enumerations;
// using Platform.Common.Models;
//
// namespace Platform.Common
// {
//     public static class SortingExtensions
//     {
//         /// <summary>
//         ///     Orders the by.
//         /// </summary>
//         /// <returns>The by.</returns>
//         /// <param name="results">Results.</param>
//         /// <param name="sorting">Sorting.</param>
//         /// <typeparam name="T">The 1st type parameter.</typeparam>
//         public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> results, Sorting sorting)
//             where T : class
//         {
//             var sortedResults = results.AsQueryable();
//             foreach (var sort in sorting.Properties)
//             {
//                 var sortingProperty = PropertyInfo<T>(sort.Property);
//
//                 if (sortingProperty != null)
//                 {
//                     var sortingPropertyType = sortingProperty.PropertyType;
//
//                     //Construct sorted query based on the sorting property type
//                     if (sortingPropertyType == typeof(string))
//                         sortedResults =
//                             SortQueryable<T, string>(sortedResults.AsQueryable(), sort.Property, sort.SortOrder);
//                     else if (sortingPropertyType == typeof(DateTime))
//                         sortedResults = SortQueryable<T, DateTime>(sortedResults, sort.Property, sort.SortOrder);
//                     else if (sortingPropertyType == typeof(bool))
//                         sortedResults = SortQueryable<T, bool>(sortedResults, sort.Property, sort.SortOrder);
//                     else if (sortingPropertyType == typeof(int) || sortingPropertyType == typeof(short) ||
//                              sortingPropertyType == typeof(long))
//                         sortedResults = SortQueryable<T, long>(sortedResults, sort.Property, sort.SortOrder);
//                     else if (sortingPropertyType == typeof(decimal) || sortingPropertyType == typeof(float) ||
//                              sortingPropertyType == typeof(double))
//                         sortedResults = SortQueryable<T, decimal>(sortedResults, sort.Property, sort.SortOrder);
//                     else if (sortingPropertyType == typeof(DateTime?))
//                         sortedResults = SortQueryable<T, DateTime?>(sortedResults, sort.Property, sort.SortOrder);
//                     else if (sortingPropertyType == typeof(int?) || sortingPropertyType == typeof(short?) ||
//                              sortingPropertyType == typeof(long?))
//                         sortedResults = SortQueryable<T, long?>(sortedResults, sort.Property, sort.SortOrder);
//                     else if (sortingPropertyType == typeof(decimal?) || sortingPropertyType == typeof(float?) ||
//                              sortingPropertyType == typeof(double?))
//                         sortedResults = SortQueryable<T, decimal?>(sortedResults, sort.Property, sort.SortOrder);
//                     else if (sortingPropertyType == typeof(bool?))
//                         sortedResults = SortQueryable<T, bool?>(sortedResults, sort.Property, sort.SortOrder);
//                     else
//                         sortedResults = SortQueryable<T, object>(sortedResults, sort.Property, sort.SortOrder);
//                 }
//             }
//
//             return sortedResults;
//         }
//
//         /// <summary>
//         ///     Properties the info.
//         /// </summary>
//         /// <returns>The info.</returns>
//         /// <param name="propertyName">Property name.</param>
//         /// <typeparam name="T">The 1st type parameter.</typeparam>
//         private static PropertyInfo PropertyInfo<T>(string propertyName)
//         {
//             var parentProps = typeof(T).GetProperties();
//
//             var props = propertyName.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
//
//             var index = 0;
//             foreach (var prop in props)
//             foreach (var parentProp in parentProps)
//             {
//                 if (parentProp.Name.ToLower() != prop.ToLower()) continue;
//
//                 //last property will/should be of basic datatype
//                 if (index == props.Length - 1)
//                 {
//                     var propInfo = typeof(T).GetProperty(propertyName,
//                         BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
//                     if (propInfo != null) return propInfo;
//                 }
//
//                 //make a recursive call to this same method and pass a different object (T)
//                 var parentPropObject = Activator.CreateInstance(parentProp.PropertyType);
//                 var method =
//                     typeof(SortingExtensions).GetMethod("PropertyInfo", BindingFlags.NonPublic | BindingFlags.Static);
//                 var generic = method.MakeGenericMethod(parentPropObject.GetType());
//                 //skip the parentProp and pass rest of the props to the new new method call
//                 var list = props.ToList().Skip(index + 1);
//
//                 index++;
//                 return (PropertyInfo) generic.Invoke(null, new object[] {string.Join(".", list)});
//             }
//
//             return null;
//         }
//     }
// }
// }
