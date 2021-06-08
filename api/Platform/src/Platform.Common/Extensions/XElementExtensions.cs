using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Platform.Common
{
    public static class XElementExtensions
    {
        /// <summary>
        ///     Returns the value of an XElement, without including the values of its child elements.
        /// </summary>
        /// <param name="xe">The XElement for which we want the value.</param>
        /// <returns>Will return the XElement's value as a string, or an empty string.</returns>
        public static string ValueWithoutChildValues(this XElement xe)
        {
            string result;

            if (xe == null) return string.Empty;

            if (xe.HasElements == false)
            {
                result = xe.Value;
            }
            else
            {
                /* We may have:
                 *          <item>some values<first>one.</first> are <second>two.</second>in here.</item
                 *   This will return:
                 *           some values are in here
                 */
                var sb = new StringBuilder();
                xe.Nodes().ToList().ForEachItem(n =>
                {
                    if (n is XText) sb.Append(n);
                });

                result = sb.ToString();
            }

            return result;
        }

        /// <summary>
        ///     Returns at least an empty string even when the element is not found.
        /// </summary>
        /// <param name="x">The XElement to pull the string value from.</param>
        /// <param name="elementName">The XName of the element to search for and pull the string value from.</param>
        /// <param name="defaultValue">The default value to set the returning value to if it is null.</param>
        /// <returns>Returns the string value coalesced to empty string when null.</returns>
        public static string GetString(this XElement x, string elementName = "", string defaultValue = "")
        {
            if (x == null) return defaultValue;

            if (elementName.HasNoValue()) return (string) x ?? defaultValue;

            return (string) x.Element(elementName) ?? defaultValue;
        }

        /// <summary>
        ///     Returns at least a boolean's default value even when the element is not found.
        /// </summary>
        /// <param name="x">The XElement to pull the boolean value from.</param>
        /// <param name="elementName">The XName of the element to search for and pull the boolean value from.</param>
        /// <param name="defaultValue">The default value to set the returning value to if it is null.</param>
        /// <returns>Returns the boolean value coalesced to false when null.</returns>
        public static bool GetBool(this XElement x, string elementName = "", bool defaultValue = false)
        {
            if (x == null) return defaultValue;

            if (elementName.HasNoValue()) return (bool?) x ?? defaultValue;

            return (bool?) x.Element(elementName) ?? defaultValue;
        }

        /// <summary>
        ///     Returns at least a int 32's default value even when the element is not found.
        /// </summary>
        /// <param name="x">The XElement to pull the int value from.</param>
        /// <param name="elementName">The XName of the element to search for and pull the int value from.</param>
        /// <param name="defaultValue">The default value to set the returning value to if it is null.</param>
        /// <returns>Returns the int value coalesced to zero when null.</returns>
        public static int GetInt(this XElement x, string elementName = "", int defaultValue = 0)
        {
            if (x == null) return defaultValue;

            if (elementName.HasNoValue()) return (int?) x ?? defaultValue;

            return (int?) x.Element(elementName) ?? defaultValue;
        }

        /// <summary>
        ///     Returns at least an empty Guid value even when the element is not found.
        /// </summary>
        /// <param name="x">The XElement to pull the guid value from.</param>
        /// <param name="elementName">The XName of the element to search for and pull the guid value from.</param>
        /// <returns>Returns the Guid value coalesced to empty Guid when null.</returns>
        public static Guid GetGuid(this XElement x, string elementName = "")
        {
            return GetGuid(x, elementName, Guid.Empty);
        }

        /// <summary>
        ///     Returns at least an empty Guid value even when the element is not found.
        /// </summary>
        /// <param name="x">The XElement to pull the guid value from.</param>
        /// <param name="elementName">The XName of the element to search for and pull the guid value from.</param>
        /// <param name="defaultValue">The default value to set the returning value to if it is null.</param>
        /// <returns>Returns the Guid value coalesced to empty Guid when null.</returns>
        public static Guid GetGuid(this XElement x, string elementName, Guid defaultValue)
        {
            if (x == null) return defaultValue;

            if (elementName.HasNoValue()) return (Guid?) x ?? defaultValue;

            return (Guid?) x.Element(elementName) ?? defaultValue;
        }

        /// <summary>
        ///     Returns at least a Min Value DateTime value even when the element is not found.
        /// </summary>
        /// <param name="x">The XElement to pull the DateTime value from.</param>
        /// <param name="elementName">The XName of the element to search for and pull the DateTime value from.</param>
        /// <returns>Returns the DateTime value coalesced to the DateTime MinValue when null.</returns>
        public static DateTime GetDateTime(this XElement x, string elementName = "")
        {
            return GetDateTime(x, elementName, DateTime.MinValue);
        }

        /// <summary>
        ///     Returns at least a Min Value DateTime value even when the element is not found.
        /// </summary>
        /// <param name="x">The XElement to pull the DateTime value from.</param>
        /// <param name="elementName">The XName of the element to search for and pull the DateTime value from.</param>
        /// <param name="defaultValue">The default value to set the returning value to if it is null.</param>
        /// <returns>Returns the DateTime value coalesced to the DateTime MinValue when null.</returns>
        public static DateTime GetDateTime(this XElement x, string elementName, DateTime defaultValue)
        {
            if (x == null) return defaultValue;

            if (elementName.HasNoValue()) return (DateTime?) x ?? defaultValue;

            return (DateTime?) x.Element(elementName) ?? defaultValue;
        }

        /// <summary>
        ///     Returns at least a decimal's default value even when the element is not found.
        /// </summary>
        /// <param name="x">The XElement to pull the decimal value from.</param>
        /// <param name="elementName">The XName of the element to search for and pull the decimal value from.</param>
        /// <param name="defaultValue">The default value to set the returning value to if it is null.</param>
        /// <returns>Returns the decimal value coalesced to zero when null.</returns>
        public static decimal GetDecimal(this XElement x, string elementName = "", decimal defaultValue = 0)
        {
            if (x == null) return defaultValue;

            if (elementName.HasNoValue()) return (decimal?) x ?? defaultValue;

            return (decimal?) x.Element(elementName) ?? defaultValue;
        }

        /// <summary>
        ///     Returns an XmlReader (**MUST BE DISPOSED OF**) based upon the content of the XElement.
        /// </summary>
        /// <param name="x">The XElement to be converted into an XmlReader.</param>
        /// <param name="options">The <c>ToString</c> options for the <c>XElement</c>.</param>
        /// <returns>An XmlReader (**MUST BE DISPOSED OF**), or null if the XElement is null.</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification =
            "Calling methods must dispose of this.")]
        public static XmlReader ToXmlReader(this XElement x, SaveOptions options = SaveOptions.None)
        {
            if (x == null) return null;

            return XmlReader.Create(new StringReader(x.ToString(options)));
        }

        /// <summary>
        ///     Evaluates an XPath expression against an XContainer (which is an XElement and an XDocument)
        ///     and returns an sequence of strings the expression resulted in.
        ///     It is recommended to pass an XDocument, but if an XElement is submitted, it will be parsed to
        ///     an XDocument so the XPath expression evaluates as expected.
        /// </summary>
        /// <param name="xContainer">
        ///     The XElement or XDocument the XPath will be applied to.
        ///     NOTE: if an XElement is submitted, it will be parsed into an XDocument before the XPath expression is evaluated.
        /// </param>
        /// <param name="xPath">A string containing the XPath expression to evaluate with.</param>
        /// <param name="exceptionMessage">The exception message string.</param>
        /// <returns>
        ///     Returns an IEnumerable of strings containing all resulting element and attribute values evaluated with the XPath
        ///     expression.
        ///     If nothing was found, then an empty enumerable is returned.
        /// </returns>
        public static IEnumerable<string> SelectXPathValues(this XContainer xContainer, string xPath,
            out string exceptionMessage)
        {
            exceptionMessage = string.Empty;

            if (xContainer == null || xPath.HasNoValue()) return Enumerable.Empty<string>();

            var errorSb = new Lazy<StringBuilder>();

            var results = Enumerable.Empty<string>();

            if (xPath.HasNoValue()) return results;

            // if the container is an element, parse it to a document so the xpath evaluation works as expected.
            if (xContainer is XElement) xContainer = XDocument.Parse(xContainer.ToString());

            var manager = new XmlNamespaceManager(xContainer.CreateNavigator().NameTable);

            IEnumerable<object> xObjects;
            object xDocXPathEvaluate;

            try
            {
                xDocXPathEvaluate = xContainer.XPathEvaluate(xPath, manager);
            }
            catch (Exception ex)
            {
                var xmlMessage = new XElement("exception", new XElement("xPath", xPath),
                    new XElement("message", ex.Message));
                errorSb.Value.AppendFormat("Error Evaluating XPath: {0}.  ", xmlMessage);
                return results;
            }

            var xDocXPathEvaluateAsIEnumerable = xDocXPathEvaluate as IEnumerable<object>;
            if (xDocXPathEvaluateAsIEnumerable != null)
                xObjects = xDocXPathEvaluateAsIEnumerable;
            else
                return new List<string> {xDocXPathEvaluate.ToString()};

            if (xObjects.NoItems()) errorSb.Value.AppendFormat("Xpath did not find results: {0}", xPath);

            // return the evaluated strings
            results = (from e in xObjects.OfType<XElement>()
                    select (string) e)
                .Concat(
                    from e in xObjects.OfType<XAttribute>()
                    select (string) e);

            if (errorSb.IsValueCreated) exceptionMessage = errorSb.Value.ToString();

            return results;
        }


        /// <summary>
        ///     Creates a new XElement as a child element to another element within this XElement.
        /// </summary>
        /// <param name="x">The current XElement.</param>
        /// <param name="newElementParentName">
        ///     The name of the parent for the new child element that already exists in this
        ///     XElement.
        /// </param>
        /// <param name="newElementName">
        ///     The name of the new element to create; if it already exists, the value will simply be
        ///     updated.
        /// </param>
        /// <param name="newElementValue">The value of the new element to create.</param>
        /// <returns>False if the parent element could not be found and the child could not be created, otherwise true.</returns>
        public static bool CreateXElement(this XElement x, string newElementParentName, string newElementName,
            string newElementValue)
        {
            var success = true;

            if (x == null || newElementName.HasNoValue() || newElementParentName.HasNoValue()) return false;

            var parent = x.DescendantsAndSelf(newElementParentName).FirstOrDefault();
            if (parent != null)
            {
                var child = parent.Element(newElementName);
                if (child == null)
                {
                    child = new XElement(newElementName);
                    parent.Add(child);
                }

                child.SetValue(newElementValue.ToSafeString());
            }
            else
            {
                success = false;
            }

            return success;
        }
    }
}