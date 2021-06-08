using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace Platform.Common
{
    /// <summary>
    ///     Defines how to compare GUID properties.
    /// </summary>
    public enum GuidComparison
    {
        /// <summary>
        ///     All Guid values will be compared.
        /// </summary>
        CompareAll,

        /// <summary>
        ///     All Guid values will be assumed equal.
        /// </summary>
        IgnoreAll,

        /// <summary>
        ///     <para>If both values are empty guids, they will be considered equal.</para>
        ///     <para>If both values are non-empty guids, they will be considered equal.</para>
        ///     <para>If one value is empty whilst the other is non-empty, they will be considered unequal.</para>
        /// </summary>
        IgnoreIfNotEmpty
    }

    /// <summary>
    ///     Used to compare two objects by traversing the whole object tree.
    /// </summary>
    public static class DeepObjectComparer
    {
        /// <summary>
        ///     Gets a string that will be used for formatting the results.
        /// </summary>
        private static string Template => "{0}:\r\n\tA = '{1}'\r\n\tB = '{2}'\r\n";

        /// <summary>
        ///     Uses Reflection to traverse the object tree and identifies any differences found.
        /// </summary>
        /// <param name="objectA">
        ///     The first object to be compared - this will be referred to as object "A" in the list of
        ///     differences.
        /// </param>
        /// <param name="objectB">
        ///     The second object to be compared - this will be referred to as object "B" in the list of
        ///     differences.
        /// </param>
        /// <returns>A <c>DeepCompareOperationalResult</c> object that details the test results.</returns>
        public static DeepCompareOperationalResult Compare(object objectA, object objectB)
        {
            return Compare(objectA, objectB, GuidComparison.CompareAll);
        }

        /// <summary>
        ///     Uses Reflection to traverse the object tree and identifies any differences found.
        /// </summary>
        /// <param name="objectA">
        ///     The first object to be compared - this will be referred to as object "A" in the list of
        ///     differences.
        /// </param>
        /// <param name="objectB">
        ///     The second object to be compared - this will be referred to as object "B" in the list of
        ///     differences.
        /// </param>
        /// <param name="guidComparison">Indicates whether GUID values should be ignored when comparing.</param>
        /// <returns>A <c>DeepCompareOperationalResult</c> object that details the test results.</returns>
        public static DeepCompareOperationalResult Compare(
            object objectA,
            object objectB,
            GuidComparison guidComparison)
        {
            var operationResult = new DeepCompareOperationalResult();

            const int recursionDepth = 0;

            // Create locals
            var sb = new StringBuilder();

            // Perform test
            operationResult.ObjectsAreIdentical =
                DeepTest(objectA, objectB, sb, null, guidComparison, recursionDepth, false);

            // If objects are different, return the details
            if (operationResult.ObjectsAreIdentical == false)
            {
                operationResult.Differences = sb.ToString();
            }

            // Return results
            return operationResult;
        }

        /// <summary>
        ///     Uses Reflection to traverse the object tree and identifies any differences found.
        /// </summary>
        /// <param name="objectA">
        ///     The first object to be compared - this will be referred to as object "A" in the list of
        ///     differences.
        /// </param>
        /// <param name="objectB">
        ///     The second object to be compared - this will be referred to as object "B" in the list of
        ///     differences.
        /// </param>
        /// <param name="guidComparison">Indicates whether GUID values should be ignored when comparing.</param>
        /// <param name="jsonSerializedStringsExist">Indicates that some string properties may be serialized.</param>
        /// <returns>A <c>DeepCompareOperationalResult</c> object that details the test results.</returns>
        public static DeepCompareOperationalResult Compare(
            object objectA,
            object objectB,
            GuidComparison guidComparison,
            bool jsonSerializedStringsExist)
        {
            var operationResult = new DeepCompareOperationalResult();

            var recursionDepth = 0;

            // Create locals
            var sb = new StringBuilder();

            // Perform test
            operationResult.ObjectsAreIdentical = DeepTest(objectA, objectB, sb, null, guidComparison, recursionDepth,
                jsonSerializedStringsExist);

            // If objects are different, return the details
            if (operationResult.ObjectsAreIdentical == false)
            {
                operationResult.Differences = sb.ToString();
            }

            // Return results
            return operationResult;
        }

        /// <summary>
        ///     <para>Performs a deep-test comparison.</para>
        /// </summary>
        /// <param name="a">The first object to be compared - this will be referred to as object "A".</param>
        /// <param name="b">The second object to be compared - this will be referred to as object "B".</param>
        /// <param name="reasonStringBuilder">A <c>StringBuilder</c> object that will list any differences found.</param>
        /// <param name="propertyName">
        ///     A property label - pass in <c>null</c> (this method self-references and will use this
        ///     property internally).
        /// </param>
        /// <param name="guidComparison">Indicates whether GUID values should be ignored when comparing.</param>
        /// <param name="recursionDepth">Integer value to measure recursion depth to prevent stack overflow errors.</param>
        /// <param name="jsonSerializedStringsExist">Indicates that some string properties may be serialized.</param>
        /// <returns>A <c>DeepCompareOperationalResult</c> object that details the test results.</returns>
        /// <returns>True if the two items really are identical, otherwise false (two nulls will be considered false).</returns>
        private static bool DeepTest(object a, object b, StringBuilder reasonStringBuilder, string propertyName,
            GuidComparison guidComparison, int recursionDepth, bool jsonSerializedStringsExist)
        {
            if (++recursionDepth == 200) return true;

            // Assume that the objects passed in are equal
            var objectsAreEqual = true;

            // Only continue of both types not null
            if (a == null || b == null)
            {
                objectsAreEqual =
                    ProcessObjectsWhereOneOrBothAreNull(a, b, reasonStringBuilder, propertyName, objectsAreEqual);

            }
            else if (a.ToString().Equals("Null") || b.ToString().Equals("Null"))
            {
                objectsAreEqual =
                    ProcessObjectsWhereOneOrBothAreNull(a, b, reasonStringBuilder, propertyName, objectsAreEqual);
            }
            else
            {
                objectsAreEqual = ProcessObjectsWhereBothNotNull(a, b, reasonStringBuilder, propertyName,
                    objectsAreEqual, guidComparison, recursionDepth, jsonSerializedStringsExist);

            }

            // Were they equal?
            return objectsAreEqual;
        }

        /// <summary>
        ///     Process two objects where both are NOT null.
        /// </summary>
        /// <param name="a">The first object to be compared.</param>
        /// <param name="b">The second object to be compared.</param>
        /// <param name="reasonStringBuilder">The <c>StringBuilder</c> to hold any differences found.</param>
        /// <param name="propertyName">The current property name.</param>
        /// <param name="objectsAreEqual">A value indicating whether the current objects are equal.</param>
        /// <param name="guidComparison">Indicates whether GUID values should be ignored when comparing.</param>
        /// <param name="recursionDepth">Integer value to measure recursion depth to prevent stack overflow errors.</param>
        /// <param name="jsonSerializedStringsExist">Indicates that some string properties may be serialized.</param>
        /// <returns>True if identical, otherwise false.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Not used in PROD.")]
        private static bool ProcessObjectsWhereBothNotNull(object a, object b, StringBuilder reasonStringBuilder,
            string propertyName, bool objectsAreEqual, GuidComparison guidComparison, int recursionDepth,
            bool jsonSerializedStringsExist)
        {
            // Load the types for these objects
            var typeA = a.GetType();
            var typeB = b.GetType();

            // What is the name of the property that we are testing?
            propertyName = GeneratePropertyName(typeA, propertyName);

            // Are the types the same?
            if (typeA.Name.Equals(typeB.Name) == false)
            {
                // Different types
                objectsAreEqual =
                    ProcessObjectsOfDifferentType(a, b, reasonStringBuilder, objectsAreEqual, typeA, typeB);
            }
            else
            {
                // Are we dealing with a dictionary entry?
                if (typeA.Name.Equals("DictionaryEntry"))
                {
                    // Dealing with a dictionary entry
                    objectsAreEqual = CompareDictionaryEntryObjects(a, b, reasonStringBuilder, propertyName,
                        objectsAreEqual, guidComparison);
                }
                else if (typeA.IsValueType || new List<string> {"String"}.Contains(typeA.Name))
                {
                    // Can we simply compare the property values?
                    if (typeA.Namespace.Equals("System", StringComparison.Ordinal))
                        objectsAreEqual = CompareValueTypeAndStringObjects(a, b, reasonStringBuilder, propertyName,
                            objectsAreEqual, guidComparison, jsonSerializedStringsExist, recursionDepth);
                    else
                        objectsAreEqual = CompareStructs(a, b, reasonStringBuilder, propertyName, objectsAreEqual,
                            guidComparison, recursionDepth, jsonSerializedStringsExist);
                }
                else
                {
                    if (a is IList)
                    {
                        objectsAreEqual = ProcessIListObjects(a, b, reasonStringBuilder, propertyName, objectsAreEqual,
                            guidComparison, recursionDepth, jsonSerializedStringsExist);
                    }
                    else if (a is NameValueCollection)
                    {
                        objectsAreEqual = ProcessNameValueCollectionObjects(a, b, reasonStringBuilder, propertyName,
                            objectsAreEqual, guidComparison, recursionDepth, jsonSerializedStringsExist);
                    }
                    else if (a is IDictionary)
                    {
                        objectsAreEqual = ProcessIDictionaryObjects(a, b, reasonStringBuilder, propertyName,
                            objectsAreEqual, guidComparison, recursionDepth, jsonSerializedStringsExist);
                    }
                    else if (a is CultureInfo)
                    {
                        objectsAreEqual =
                            CompareCultureInfoObjects(a, b);
                    }
                    else if (a is XElement)
                    {
                        objectsAreEqual = CompareXElements((XElement) a, (XElement) b, reasonStringBuilder,
                            propertyName, guidComparison);
                    }
                    else
                    {
                        objectsAreEqual = IterateOverAllObjectProperties(a, b, reasonStringBuilder, propertyName,
                            objectsAreEqual, typeA, guidComparison, recursionDepth, jsonSerializedStringsExist);
                    }
                }
            }

            // Return our value
            return objectsAreEqual;
        }

        /// <summary>
        ///     Compares CultureInfo objects.
        /// </summary>
        /// <param name="a">The first object to be compared.</param>
        /// <param name="b">The second object to be compared.</param>
        private static bool CompareCultureInfoObjects(object a, object b)
        {
            // Only need to compare the LCID values
            return ((CultureInfo) a).LCID.Equals(((CultureInfo) b).LCID);
        }

        /// <summary>
        ///     Compares structures.
        /// </summary>
        /// <param name="a">The first object to be compared.</param>
        /// <param name="b">The second object to be compared.</param>
        /// <param name="reasonStringBuilder">The <c>StringBuilder</c> to hold any differences found.</param>
        /// <param name="propertyName">The current property name.</param>
        /// <param name="objectsAreEqual">A value indicating whether the current objects are equal.</param>
        /// <param name="guidComparison">Indicates whether GUID values should be ignored when comparing.</param>
        /// <param name="recursionDepth">Integer value to measure recursion depth to prevent stack overflow errors.</param>
        /// <param name="jsonSerializedStringsExist">Indicates that some string properties may be serialized.</param>
        /// <returns>True of identical, otherwise false.</returns>
        private static bool CompareStructs(object a, object b, StringBuilder reasonStringBuilder, string propertyName,
            bool objectsAreEqual, GuidComparison guidComparison, int recursionDepth, bool jsonSerializedStringsExist)
        {
            return IterateOverAllObjectProperties(a, b, reasonStringBuilder, propertyName, objectsAreEqual, a.GetType(),
                guidComparison, recursionDepth, jsonSerializedStringsExist);
        }

        /// <summary>
        ///     Compares objects where one or both objects are null.
        /// </summary>
        /// <param name="a">The first object to be compared.</param>
        /// <param name="b">The second object to be compared.</param>
        /// <param name="reasonStringBuilder">The <c>StringBuilder</c> to hold any differences found.</param>
        /// <param name="propertyName">The current property name.</param>
        /// <param name="objectsAreEqual">A value indicating whether the current objects are equal.</param>
        /// <returns>True if both null, otherwise false.</returns>
        private static bool ProcessObjectsWhereOneOrBothAreNull(object a, object b, StringBuilder reasonStringBuilder,
            string propertyName, bool objectsAreEqual)
        {
            // If one is null but not the other, then they're not equal.  If two objects are both null then they will be considered "equal".
            if ((a == null).Equals(b == null) == false)
            {
                objectsAreEqual = false;
                reasonStringBuilder.AppendLine(string.Format(Template, propertyName, NullTestResult(a),
                    NullTestResult(b)));
            }

            return objectsAreEqual;
        }

        /// <summary>
        ///     Iterates over all properties of an object.
        /// </summary>
        /// <param name="a">The first object to be compared.</param>
        /// <param name="b">The second object to be compared.</param>
        /// <param name="reasonStringBuilder">The <c>StringBuilder</c> to hold any differences found.</param>
        /// <param name="propertyName">The current property name.</param>
        /// <param name="objectsAreEqual">A value indicating whether the current objects are equal.</param>
        /// <param name="typeA">The type of the object A.</param>
        /// <param name="guidComparison">Indicates whether GUID values should be ignored when comparing.</param>
        /// <param name="recursionDepth">Integer value to measure recursion depth to prevent stack overflow errors.</param>
        /// <param name="jsonSerializedStringsExist">Indicates that some string properties may be serialized.</param>
        /// <returns>True if identical, otherwise false.</returns>
        private static bool IterateOverAllObjectProperties(object a, object b, StringBuilder reasonStringBuilder,
            string propertyName, bool objectsAreEqual, Type typeA, GuidComparison guidComparison, int recursionDepth,
            bool jsonSerializedStringsExist)
        {
            if (ShouldProcessProperty(propertyName) == false)
            {
                return  true;
            }

            // Iterate through all the properties - need to be careful of "SyncRoot" which gives infinite loops of self-referencing
            if (typeA.GetProperties().AnyItems())
            {
                foreach (var prop in typeA.GetProperties()
                    .Where(p =>
                        p.Name.Equals("Empty", StringComparison.Ordinal) == false &&
                        p.Name.Equals("SyncRoot", StringComparison.Ordinal) == false))
                {
                    // We don't care about the index item property
                    if ((prop.Name.Equals("Item", StringComparison.Ordinal) && prop.GetIndexParameters().Length > 0) ==
                        false)
                    {
                        if ((typeA.Name.Equals("SqlConnection", StringComparison.OrdinalIgnoreCase) &&
                             prop.Name.Equals("ServerVersion", StringComparison.OrdinalIgnoreCase)) == false)
                        {
                            try
                            {
                                var result = DeepTest(prop.GetValue(a, null), prop.GetValue(b, null),
                                    reasonStringBuilder, string.Format("{0}.{1}", propertyName, prop.Name),
                                    guidComparison, recursionDepth, jsonSerializedStringsExist);
                                if (objectsAreEqual) objectsAreEqual = result;
                            }
                            catch (Exception ex)
                            {
                                if (ex.GetType() == typeof(ArgumentException)
                                    && ex.Message.IsEqualTo("Property Get method was not found."))
                                {
                                    // If can't get at a property, then pass it by.
                                }
                                else if (ex.InnerException != null &&
                                         ex.InnerException.GetType().Equals(typeof(NotImplementedException)))
                                {
                                    // If the method is not implemented, then it doesn't matter.
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }
                        else
                        {
                            objectsAreEqual = CompareValueTypeAndStringObjects(a, b, reasonStringBuilder, propertyName,
                                objectsAreEqual, guidComparison, jsonSerializedStringsExist, recursionDepth);
                        }
                    }
                }
            }

            return objectsAreEqual;
        }

        /// <summary>
        ///     Determines whether the property value should be processed based upon the property name.
        /// </summary>
        /// <param name="propertyName">The name of the property to be processed.</param>
        /// <returns>True if should process.</returns>
        private static bool ShouldProcessProperty(string propertyName)
        {
            var shouldProcess = !propertyName.EndsWith("Value.Null", StringComparison.Ordinal);

            var cultureTypes = new List<string>
            {
                ".CurrentCulture",
                ".CurrentInfo",
                ".CurrentUICulture",
                ".InstalledUICulture",
                ".InvariantCulture",
                ".InvariantInfo",
                ".Parent"
            };

            var recursivePropertyNames = new List<string>();
            var max = cultureTypes.Count;

            for (var p1 = 0; p1 < max; p1++)
            {
                for (var p2 = 0; p2 < max; p2++)
                {
                    recursivePropertyNames.Add(string.Format("{0}{1}", cultureTypes[p1], cultureTypes[p2]));
                }
            }

            recursivePropertyNames.ForEachItem(pn =>
            {
                if (propertyName.EndsWith(pn, StringComparison.Ordinal))
                {
                    shouldProcess = false;
                }
            });

            return shouldProcess;
        }

        /// <summary>
        ///     Compares objects that implement the IDictionary interface.
        /// </summary>
        /// <param name="a">The first object to be compared.</param>
        /// <param name="b">The second object to be compared.</param>
        /// <param name="reasonStringBuilder">The <c>StringBuilder</c> to hold any differences found.</param>
        /// <param name="propertyName">The current property name.</param>
        /// <param name="objectsAreEqual">A value indicating whether the current objects are equal.</param>
        /// <param name="guidComparison">Indicates whether GUID values should be ignored when comparing.</param>
        /// <param name="recursionDepth">Integer value to measure recursion depth to prevent stack overflow errors.</param>
        /// <param name="jsonSerializedStringsExist">Indicates that some string properties may be serialized.</param>
        /// <returns>True if identical, otherwise false.</returns>
        private static bool ProcessIDictionaryObjects(object a, object b, StringBuilder reasonStringBuilder,
            string propertyName, bool objectsAreEqual, GuidComparison guidComparison, int recursionDepth,
            bool jsonSerializedStringsExist)
        {
            var dictA = (IDictionary) a;
            var dictB = (IDictionary) b;

            // Test the number of items in the dictionary - are they the same
            if (IDictionariesContainDifferentNumberOfItems(dictA, dictB))
            {
                objectsAreEqual = false;
                reasonStringBuilder.AppendLine(string.Format(Template, propertyName, NullTestResult(a),
                    NullTestResult(b)));
            }
            else
            {
                // Are there any items in the list?
                if (dictA.IsNullOrEmpty() == false)
                {
                    var iteratorA = dictA.GetEnumerator();
                    var iteratorB = dictB.GetEnumerator();
                    var i = 0;

                    while (iteratorA.MoveNext())
                    {
                        iteratorB.MoveNext();

                        var result = DeepTest(iteratorA.Current, iteratorB.Current, reasonStringBuilder,
                            string.Format("{0}[{1}]", propertyName, i), guidComparison, recursionDepth,
                            jsonSerializedStringsExist);

                        if (objectsAreEqual) objectsAreEqual = result;

                        i++;
                    }
                }
            }

            return objectsAreEqual;
        }

        /// <summary>
        ///     Compares objects that are NameValueCollection objects.
        /// </summary>
        /// <param name="a">The first object to be compared.</param>
        /// <param name="b">The second object to be compared.</param>
        /// <param name="reasonStringBuilder">The <c>StringBuilder</c> to hold any differences found.</param>
        /// <param name="propertyName">The current property name.</param>
        /// <param name="objectsAreEqual">A value indicating whether the current objects are equal.</param>
        /// <param name="guidComparison">Indicates whether GUID values should be ignored when comparing.</param>
        /// <param name="recursionDepth">Integer value to measure recursion depth to prevent stack overflow errors.</param>
        /// <param name="jsonSerializedStringsExist">Indicates that some string properties may be serialized.</param>
        /// <returns>True if identical, otherwise false.</returns>
        private static bool ProcessNameValueCollectionObjects(object a, object b, StringBuilder reasonStringBuilder,
            string propertyName, bool objectsAreEqual, GuidComparison guidComparison, int recursionDepth,
            bool jsonSerializedStringsExist)
        {
            var nvcA = (NameValueCollection) a;
            var nvcB = (NameValueCollection) b;

            // Test the number of items in the name value collection - are they the same
            if (NameValueCollectionContainDifferenNumberOfItems(nvcA, nvcB))
            {
                objectsAreEqual = false;
                reasonStringBuilder.AppendLine(string.Format(Template, propertyName, NullTestResult(a),
                    NullTestResult(b)));
            }
            else
            {
                // Are there any items in the list?
                if (nvcA.IsNullOrEmpty() == false)
                {
                    var iteratorA = nvcA.GetEnumerator();
                    var iteratorB = nvcB.GetEnumerator();

                    const string NvcItemTemplate = "[{0}] - {1}";

                    var i = 0;
                    while (iteratorA.MoveNext())
                    {
                        iteratorB.MoveNext();
                        var result = DeepTest(
                            string.Format(NvcItemTemplate, iteratorA.Current, nvcA[iteratorA.Current.ToString()]),
                            string.Format(NvcItemTemplate, iteratorB.Current, nvcB[iteratorB.Current.ToString()]),
                            reasonStringBuilder,
                            string.Format("{0}[{1}]", propertyName, i),
                            guidComparison,
                            recursionDepth, jsonSerializedStringsExist);

                        if (objectsAreEqual)
                        {
                            objectsAreEqual = result;
                        }

                        i++;
                    }
                }
            }

            return objectsAreEqual;
        }

        /// <summary>
        ///     Compares objects that implement the IList interface.
        /// </summary>
        /// <param name="a">The first object to be compared.</param>
        /// <param name="b">The second object to be compared.</param>
        /// <param name="reasonStringBuilder">The <c>StringBuilder</c> to hold any differences found.</param>
        /// <param name="propertyName">The current property name.</param>
        /// <param name="objectsAreEqual">A value indicating whether the current objects are equal.</param>
        /// <param name="guidComparison">Indicates whether GUID values should be ignored when comparing.</param>
        /// <param name="recursionDepth">Integer value to measure recursion depth to prevent stack overflow errors.</param>
        /// <param name="jsonSerializedStringsExist">Indicates that some string properties may be serialized.</param>
        /// <returns>True if identical, otherwise false.</returns>
        private static bool ProcessIListObjects(object a, object b, StringBuilder reasonStringBuilder,
            string propertyName, bool objectsAreEqual, GuidComparison guidComparison, int recursionDepth,
            bool jsonSerializedStringsExist)
        {
            // Cast to an IList object
            var listA = (IList) a;
            var listB = (IList) b;

            // Test the number of items in the list - are they the same
            if (IListsContainDifferentNumberOfItems(listA, listB))
            {
                objectsAreEqual = false;
                reasonStringBuilder.AppendLine(string.Format(Template, propertyName, NullTestResult(a),
                    NullTestResult(b)));
            }
            else
            {
                // Are there any items in the list?
                if (listA.IsNullOrEmpty() == false)
                {
                    var iteratorA = listA.GetEnumerator();
                    var iteratorB = listB.GetEnumerator();
                    var i = 0;
                    while (iteratorA.MoveNext())
                    {
                        iteratorB.MoveNext();

                        var result = DeepTest(iteratorA.Current, iteratorB.Current, reasonStringBuilder,
                            string.Format("{0}[{1}]", propertyName, i), guidComparison, recursionDepth,
                            jsonSerializedStringsExist);

                        if (objectsAreEqual) objectsAreEqual = result;

                        i++;
                    }
                }
            }

            return objectsAreEqual;
        }

        /// <summary>
        ///     Process objects of different types.
        /// </summary>
        /// <param name="a">The first object to be compared.</param>
        /// <param name="b">The second object to be compared.</param>
        /// <param name="reasonStringBuilder">The <c>StringBuilder</c> to hold any differences found.</param>
        /// <param name="objectsAreEqual">A value indicating whether the current objects are equal.</param>
        /// <param name="typeA">The type of object A.</param>
        /// <param name="typeB">The type of object B.</param>
        /// <returns>False as different types.</returns>
        private static bool ProcessObjectsOfDifferentType(object a, object b, StringBuilder reasonStringBuilder,
            bool objectsAreEqual, Type typeA, Type typeB)
        {
            objectsAreEqual = false;
            reasonStringBuilder.AppendLine(
                string.Format("Objects not of same type: {0} != {1}", typeA.Name, typeB.Name));
            reasonStringBuilder.AppendLine();

            ProcessCommonInheritancePattern(a, b, reasonStringBuilder);

            return objectsAreEqual;
        }

        /// <summary>
        ///     Compares Value types (and Strings).
        /// </summary>
        /// <param name="a">The first object to be compared.</param>
        /// <param name="b">The second object to be compared.</param>
        /// <param name="reasonStringBuilder">The <c>StringBuilder</c> to hold any differences found.</param>
        /// <param name="propertyName">The property name under test.</param>
        /// <param name="objectsAreEqual">A value indicating whether the current objects are equal.</param>
        /// <param name="guidComparison">Indicates whether GUID values should be ignored when comparing.</param>
        /// <param name="jsonSerializedStringsExist">Indicates that some string properties may be serialized.</param>
        /// <param name="recursionDepth">Integer value to measure recursion depth to prevent stack overflow errors.</param>
        /// <returns>True if identical, otherwise false.</returns>
        private static bool CompareValueTypeAndStringObjects(object a, object b, StringBuilder reasonStringBuilder,
            string propertyName, bool objectsAreEqual, GuidComparison guidComparison, bool jsonSerializedStringsExist,
            int recursionDepth)
        {
            // Guids are special cases
            var t = a.GetType();

            Guid aGuid;
            Guid bGuid;

            if (t.Name.Equals("Guid"))
            {
                // The client can determine how to compare guids via the "guidComparison" argument
                objectsAreEqual =
                    CompareGuids(a, b, reasonStringBuilder, propertyName, objectsAreEqual, guidComparison);
            }
            else if (Guid.TryParse(a.ToSafeString(), out aGuid) && Guid.TryParse(b.ToSafeString(), out bGuid))
            {
                objectsAreEqual = CompareGuids(aGuid, bGuid, reasonStringBuilder, propertyName, objectsAreEqual,
                    guidComparison);
            }
            else
            {
                if (t.IsValueType)
                {
                    objectsAreEqual = CompareValueTypes(a, b, reasonStringBuilder, propertyName, objectsAreEqual);
                }
                else if (t.Name.Equals("String"))
                {
                    XElement xeA = null;
                    XElement xeB = null;

                    var aString = a.ToSafeString();
                    var bString = b.ToSafeString();

                    if (aString.StartsWith("<") && aString.EndsWith(">") && bString.StartsWith("<") &&
                        bString.EndsWith(">"))
                        try
                        {
                            xeA = XElement.Parse(a.ToString());
                            xeB = XElement.Parse(b.ToString());
                        }
                        catch
                        {
                        }

                    if (xeA != null && xeB != null)
                    {
                        objectsAreEqual = CompareXElements(xeA, xeB, reasonStringBuilder, propertyName, guidComparison);
                    }
                    else
                    {
                        var tempObjectsAreEqual =
                            CompareValueTypes(a, b, reasonStringBuilder, propertyName, objectsAreEqual);

                        if (tempObjectsAreEqual == false)
                        {
                            if (jsonSerializedStringsExist && aString.IsJsonSerialized() && bString.IsJsonSerialized())
                            {
                                var aJson = JsonConvert.DeserializeObject(aString);
                                var bJson = JsonConvert.DeserializeObject(bString);

                                objectsAreEqual = DeepTest(aJson, bJson, reasonStringBuilder, propertyName,
                                    guidComparison, recursionDepth, jsonSerializedStringsExist);
                            }
                            else
                            {
                                objectsAreEqual = tempObjectsAreEqual;
                            }
                        }
                        else
                        {
                            objectsAreEqual = tempObjectsAreEqual;
                        }
                    }
                }
            }

            return objectsAreEqual;
        }

        /// <summary>
        ///     Compares <c>XElements</c>.
        /// </summary>
        /// <param name="a">The first object to be compared.</param>
        /// <param name="b">The second object to be compared.</param>
        /// <param name="reasonStringBuilder">The <c>StringBuilder</c> to hold any differences found.</param>
        /// <param name="propertyName">The property name under test.</param>
        /// <param name="guidComparison">Indicates whether GUID values should be ignored when comparing.</param>
        /// <returns>True if identical, otherwise false.</returns>
        private static bool CompareXElements(XElement a, XElement b, StringBuilder reasonStringBuilder,
            string propertyName, GuidComparison guidComparison)
        {
            var response = true;

            if (XNode.DeepEquals(a, b)) return true;

            // Figure out what's different...

            // Check the attributes
            var aAttributes = a.Attributes().ToList();
            var bAttributes = b.Attributes().ToList();

            var aHasAttributes = aAttributes.IsNullOrEmpty() == false;
            var bHasAttributes = bAttributes.IsNullOrEmpty() == false;

            // If one has attributes, but not the other then report differences
            if (aHasAttributes != bHasAttributes)
            {
                reasonStringBuilder.AppendLine(string.Format(Template, propertyName + " XElement " + a.Name,
                    string.Format("{0} has attributes = {1}", a.Name, aHasAttributes),
                    string.Format("{0} has attributes = {1}", b.Name, bHasAttributes)));
                return false;
            }

            // Do we have the same number of attributes
            if (aAttributes.Count.Equals(bAttributes.Count) == false)
            {
                reasonStringBuilder.AppendLine(string.Format(Template, propertyName + " XElement " + a.Name,
                    string.Format("{0} has {1} attribute(s)", a.Name, aAttributes.Count),
                    string.Format("{0} has {1} attribute(s)", b.Name, bAttributes.Count)));
                return false;
            }

            // Check the attribute names and values match
            var attributeCounter = 0;
            foreach (var attA in aAttributes)
            {
                var attB = bAttributes[attributeCounter];

                if (attA.Name.Equals(attB.Name) == false)
                {
                    response = false;
                    reasonStringBuilder.AppendLine(string.Format(Template, propertyName + " XElement " + a.Name,
                        attA.Name, attB.Name));
                }

                if (attA.Value.Equals(attB.Value) == false)
                {
                    response = false;
                    reasonStringBuilder.AppendLine(string.Format(Template, propertyName + " XElement " + a.Name,
                        attA.Value, attB.Value));
                }

                attributeCounter++;
            }

            // Check the elements
            var aElements = a.Elements().ToList();
            var bElements = b.Elements().ToList();

            var aHasElements = aElements.IsNullOrEmpty() == false;
            var bHasElements = bElements.IsNullOrEmpty() == false;

            // If one has elements, but not the other then report differences
            if (aHasElements != bHasElements)
            {
                reasonStringBuilder.AppendLine(string.Format(Template, propertyName + " XElement " + a.Name,
                    string.Format("{0} has elements = {1}", a.Name, aHasElements),
                    string.Format("{0} has elements = {1}", b.Name, bHasElements)));
                response = false;
            }

            // Do we have the same number of elements
            if (aElements.Count.Equals(bElements.Count) == false)
            {
                reasonStringBuilder.AppendLine(string.Format(Template, propertyName + " XElement " + a.Name,
                    string.Format("{0} has {1} element(s)", a.Name, aElements.Count),
                    string.Format("{0} has {1} element(s)", b.Name, bElements.Count)));
                response = false;
            }

            // Check that the element names match
            if (a.Name.Equals(b.Name) == false)
            {
                reasonStringBuilder.AppendLine(string.Format(Template, propertyName + " XElement " + a.Name, a.Name,
                    b.Name));
                response = false;
            }

            // Check that the element values match (this may require Guid intervention)
            var aValue = a.ValueWithoutChildValues();
            var bValue = b.ValueWithoutChildValues();
            Guid aAsGuid;
            Guid bAsGuid;
            if (Guid.TryParse(aValue, out aAsGuid) && Guid.TryParse(bValue, out bAsGuid))
            {
                if (CompareGuids(aAsGuid, bAsGuid, reasonStringBuilder, propertyName + " XElement " + a.Name, true,
                        guidComparison) == false) response = false;
            }
            else if (a.ValueWithoutChildValues().Trim().Equals(b.ValueWithoutChildValues().Trim()) == false)
            {
                reasonStringBuilder.AppendLine(string.Format(Template, propertyName + " XElement " + a.Name,
                    a.ValueWithoutChildValues(), b.ValueWithoutChildValues()));
                response = false;
            }

            // Check child elements
            if (aHasElements && aElements.Count.Equals(bElements.Count))
            {
                var elementCounter = 0;
                foreach (var ae in aElements)
                {
                    var be = bElements[elementCounter];

                    var result = CompareXElements(ae, be, reasonStringBuilder, propertyName + " XElement " + a.Name,
                        guidComparison);
                    if (response) response = result;

                    elementCounter++;
                }
            }

            return response;
        }

        /// <summary>
        ///     Compares the values of two Guids.
        /// </summary>
        /// <param name="a">The first Guid to compare.</param>
        /// <param name="b">The second Guid to compare.</param>
        /// <param name="reasonStringBuilder">The <c>StringBuilder</c> to hold any differences found.</param>
        /// <param name="propertyName">The property name under test.</param>
        /// <param name="objectsAreEqual">A value indicating whether the current objects are equal.</param>
        /// <param name="guidComparison">A value indication how Guids should be compared.</param>
        /// <returns>Returns true if deemed identical.</returns>
        private static bool CompareGuids(object a, object b, StringBuilder reasonStringBuilder, string propertyName,
            bool objectsAreEqual, GuidComparison guidComparison)
        {
            switch (guidComparison)
            {
                case GuidComparison.CompareAll:
                {
                    // Treat as any other Value type
                    objectsAreEqual = CompareValueTypes(a, b, reasonStringBuilder, propertyName, objectsAreEqual);
                    break;
                }

                case GuidComparison.IgnoreAll:
                {
                    // Do nothing, so assume equal
                    break;
                }

                case GuidComparison.IgnoreIfNotEmpty:
                {
                    if (a.Equals(Guid.Empty).Equals(b.Equals(Guid.Empty)) == false)
                    {
                        // One value is empty, the other not
                        objectsAreEqual = false;
                        reasonStringBuilder.AppendLine(string.Format(Template, propertyName, a, b));
                    }

                    break;
                }
            }

            return objectsAreEqual;
        }

        /// <summary>
        ///     Compares values.
        /// </summary>
        /// <param name="a">The first object to be compared.</param>
        /// <param name="b">The second object to be compared.</param>
        /// <param name="reasonStringBuilder">The <c>StringBuilder</c> to hold any differences found.</param>
        /// <param name="propertyName">The property name under test.</param>
        /// <param name="objectsAreEqual">A value indicating whether the current objects are equal.</param>
        /// <returns>True if deemed equal.</returns>
        private static bool CompareValueTypes(object a, object b, StringBuilder reasonStringBuilder,
            string propertyName, bool objectsAreEqual)
        {
            if (a.Equals(b) == false)
            {
                objectsAreEqual = false;
                reasonStringBuilder.AppendLine(string.Format(Template, propertyName, a, b));
            }

            return objectsAreEqual;
        }

        /// <summary>
        ///     Compares two objects that are Dictionary Entry items.
        /// </summary>
        /// <param name="a">The first object to be compared.</param>
        /// <param name="b">The second object to be compared.</param>
        /// <param name="reasonStringBuilder">The <c>StringBuilder</c> to hold any differences found.</param>
        /// <param name="propertyName">The property name under test.</param>
        /// <param name="objectsAreEqual">A value indicating whether the current objects are equal.</param>
        /// <param name="guidComparison">A value indication how Guids should be compared.</param>
        /// <returns>True if identical, otherwise false.</returns>
        private static bool CompareDictionaryEntryObjects(object a, object b, StringBuilder reasonStringBuilder,
            string propertyName, bool objectsAreEqual, GuidComparison guidComparison)
        {
            var dictEntriesAreIdentical = true;

            var dictEntryA = (DictionaryEntry) a;
            var dictEntryB = (DictionaryEntry) b;

            // Are key's GUIDs?
            var keysAreGuids = false;
            var keyAGuid = Guid.Empty;
            var keyBGuid = Guid.Empty;

            if (Guid.TryParse(dictEntryA.Key.ToString(), out keyAGuid) &&
                Guid.TryParse(dictEntryB.Key.ToString(), out keyBGuid)) keysAreGuids = true;

            if (keysAreGuids)
                switch (guidComparison)
                {
                    case GuidComparison.CompareAll:
                    {
                        dictEntriesAreIdentical =
                            keyAGuid.Equals(keyBGuid) && dictEntryA.Value.Equals(dictEntryB.Value);
                        break;
                    }

                    case GuidComparison.IgnoreAll:
                    {
                        // Just compare the values
                        dictEntriesAreIdentical = dictEntryA.Value.Equals(dictEntryB.Value);

                        break;
                    }

                    case GuidComparison.IgnoreIfNotEmpty:
                    {
                        if ((keyAGuid != Guid.Empty && keyBGuid != Guid.Empty) == false)
                            dictEntriesAreIdentical =
                                keyAGuid.Equals(keyBGuid) && dictEntryA.Value.Equals(dictEntryB.Value);
                        else
                            dictEntriesAreIdentical = dictEntryA.Value.Equals(dictEntryB.Value);

                        break;
                    }
                }
            else
                dictEntriesAreIdentical = Compare(dictEntryA.Key, dictEntryB.Key, guidComparison).ObjectsAreIdentical
                                          && Compare(dictEntryA.Value, dictEntryB.Value, guidComparison)
                                              .ObjectsAreIdentical;

            if (dictEntriesAreIdentical == false)
            {
                objectsAreEqual = false;
                const string DictionaryTemplate = "[{0}] - {1}";

                reasonStringBuilder.AppendLine(
                    string.Format(Template,
                        propertyName,
                        string.Format(DictionaryTemplate, dictEntryA.Key, dictEntryA.Value),
                        string.Format(DictionaryTemplate, dictEntryB.Key, dictEntryB.Value)));
            }

            return objectsAreEqual;
        }

        /// <summary>
        ///     This method with iterate through the inheritance trees for both objects and will attempt to find a common base
        ///     type.
        /// </summary>
        /// <param name="a">The first object to be compared.</param>
        /// <param name="b">The second object to be compared.</param>
        /// <param name="reasonStringBuilder">A <c>StringBuilder</c> object that will list any differences found.</param>
        private static void ProcessCommonInheritancePattern(object a, object b, StringBuilder reasonStringBuilder)
        {
            var inheritanceTreeA = a.GetType().GetInheritanceHierarchy().Select(t => t.Name).ToList();
            var inheritanceTreeB = b.GetType().GetInheritanceHierarchy().Select(t => t.Name).ToList();

            // Write these out
            reasonStringBuilder.AppendLine(string.Format("A's inheritance tree: {0}",
                string.Join(" >> ", inheritanceTreeA.ToArray<string>())));
            reasonStringBuilder.AppendLine(string.Format("B's inheritance tree: {0}",
                string.Join(" >> ", inheritanceTreeB.ToArray<string>())));

            // Find a common base
            var commonType =
                (from tA in inheritanceTreeA
                    from tB in inheritanceTreeB
                    where string.Equals(tA, "object", StringComparison.OrdinalIgnoreCase) == false
                    where string.Equals(tB, "object", StringComparison.OrdinalIgnoreCase) == false
                    where string.Equals(tA, tB, StringComparison.OrdinalIgnoreCase)
                    select tA).FirstOrDefault();

            if (commonType != null)
            {
                reasonStringBuilder.AppendLine();
                reasonStringBuilder.AppendLine(string.Format("Common base type is '{0}'.", commonType));
            }
        }

        /// <summary>
        ///     Iteratively traverses the inheritance tree for an object.
        /// </summary>
        /// <param name="type">The type to be analyzed.</param>
        /// <returns>An enumerable inheritance tree.</returns>
        private static IEnumerable<Type> GetInheritanceHierarchy(this Type type)
        {
            for (var current = type; current != null; current = current.BaseType) yield return current;
        }

        /// <summary>
        ///     This will generate the property name.
        /// </summary>
        /// <param name="type">The type of the object being tested.</param>
        /// <param name="propertyName">The existing property name.</param>
        /// <returns>The property name to use.</returns>
        private static string GeneratePropertyName(Type type, string propertyName)
        {
            // Do we need to initiate the property name?
            if (propertyName.IsNullOrEmpty())
            {
                // Are we dealing with a list? - we require the "{0}" prefix to put either "A" or "B" to define the object being investigated.
                if (type.Name == "List`1")
                    propertyName = string.Format("List<{0}>", NameOfListObject(type));
                else
                    propertyName = type.Name;
            }

            // Return the value
            return propertyName;
        }

        /// <summary>
        ///     Identifies T in a List of T.
        /// </summary>
        /// <param name="type">The list type.</param>
        /// <returns>The name of T.</returns>
        private static string NameOfListObject(Type type)
        {
            return type.AssemblyQualifiedName
                .Split("[[".ToArray(), StringSplitOptions.RemoveEmptyEntries)
                .Last()
                .Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries)
                .First()
                .Split(".".ToArray(), StringSplitOptions.RemoveEmptyEntries)
                .Last();
        }

        /// <summary>
        ///     Provides a description for a null test of an object.
        /// </summary>
        /// <param name="o">The object to be tested.</param>
        /// <returns>The description of the null-test result.</returns>
        private static string NullTestResult(object o)
        {
            // The description
            var description = string.Empty;

            if (o == null)
            {
                description = "NULL";
            }
            else
            {
                // Are we dealing with a list?
                var listO = o as IList;
                if (listO != null)
                {
                    description = string.Format("IList<{0}>.Count = '{1}'", NameOfListObject(o.GetType()),
                        listO.Count.ToString());
                }
                else
                {
                    var dictO = o as IDictionary;
                    if (dictO != null)
                    {
                        description = string.Format("IDictionary<{0}>.Count = '{1}'", NameOfListObject(o.GetType()),
                            dictO.Count.ToString());
                    }
                    else
                    {
                        var nvcO = o as NameValueCollection;
                        if (nvcO != null)
                            description = string.Format("NameValueCollection.Count = '{0}'", nvcO.Count.ToString());
                        else
                            description = "Not null";
                    }
                }
            }

            return description;
        }

        /// <summary>
        ///     Processes two ILists to see if they have the same number of items.
        /// </summary>
        /// <param name="a">The first item to compare.</param>
        /// <param name="b">The second item to compare.</param>
        /// <returns>True if same number of items, otherwise false.</returns>
        private static bool IListsContainDifferentNumberOfItems(IList a, IList b)
        {
            // Assume that the number of items will be the same
            var itemCountDiffers = false;

            // load the InNullOrEmpty values just once
            var listAIsNullOrEmpty = a.IsNullOrEmpty();
            var listBIsNullOrEmpty = b.IsNullOrEmpty();

            // Check the number of items
            if (listAIsNullOrEmpty.Equals(listBIsNullOrEmpty) == false)
                itemCountDiffers = true;
            else if (listAIsNullOrEmpty == false && listBIsNullOrEmpty == false && a.Count.Equals(b.Count) == false)
                itemCountDiffers = true;

            return itemCountDiffers;
        }

        /// <summary>
        ///     Processes two NameValueCollection objects to see if they have the same number of items.
        /// </summary>
        /// <param name="a">The first item to compare.</param>
        /// <param name="b">The second item to compare.</param>
        /// <returns>True if same number of items, otherwise false.</returns>
        private static bool NameValueCollectionContainDifferenNumberOfItems(NameValueCollection a,
            NameValueCollection b)
        {
            // Assume that the number of items will be the same
            var itemCountDiffers = false;

            var aIsNullOrEmpty = a.IsNullOrEmpty();
            var bIsNullOrEmpty = b.IsNullOrEmpty();

            // Check the number of items
            if (aIsNullOrEmpty.Equals(bIsNullOrEmpty) == false)
                itemCountDiffers = true;
            else if (aIsNullOrEmpty == false && bIsNullOrEmpty == false && a.Count.Equals(b.Count) == false)
                itemCountDiffers = true;

            return itemCountDiffers;
        }

        /// <summary>
        ///     Processes two IDictionary objects to see if they have the same number of items.
        /// </summary>
        /// <param name="a">The first item to compare.</param>
        /// <param name="b">The second item to compare.</param>
        /// <returns>True if same number of items, otherwise false.</returns>
        private static bool IDictionariesContainDifferentNumberOfItems(IDictionary a, IDictionary b)
        {
            // Assume that the number of items will be the same
            var itemCountDiffers = false;

            var dictionaryAIsNullOrEmpty = a.IsNullOrEmpty();
            var dictionaryBIsNullOrEmpty = b.IsNullOrEmpty();

            // Check the number of items
            if (dictionaryAIsNullOrEmpty.Equals(dictionaryBIsNullOrEmpty) == false)
                itemCountDiffers = true;
            else if (dictionaryAIsNullOrEmpty == false && dictionaryBIsNullOrEmpty == false &&
                     a.Count.Equals(b.Count) == false) itemCountDiffers = true;

            return itemCountDiffers;
        }
    }
}