using System;
using System.Xml;
using System.Xml.Linq;

namespace Platform.Common
{
    /// <summary>
    ///     Extension methods for System.Guid
    /// </summary>
    public static class GuidExtensions
    {
        /// <summary>
        ///     Returns a non-empty Guid if empty.
        /// </summary>
        /// <param name="g">The extension method Guid.</param>
        /// <param name="defaultValue">Default GUID to use.</param>
        /// <returns>A non-empty Guid.</returns>
        public static Guid IfEmptyDefaultTo(this Guid g, Guid defaultValue)
        {
            return g == Guid.Empty
                ? defaultValue
                : g;
        }

        /// <summary>
        ///     Indicates whether a Guid is Empty.
        /// </summary>
        /// <param name="g">The extension method guid</param>
        /// <returns>Returns true if the guid is empty</returns>
        public static bool IsEmpty(this Guid g)
        {
            return g == Guid.Empty;
        }

        /// <summary>
        ///     Indicates whether a Guid is not an empty Guid.
        /// </summary>
        /// <param name="g">The extension method guid.</param>
        /// <returns>Returns true if the guid is empty.</returns>
        public static bool IsNotEmpty(this Guid g)
        {
            return g != Guid.Empty;
        }

        public static bool IsNullOrEmpty(this Guid? g)
        {
            return g == null || g.HasValue == false || g.Value == Guid.Empty;
        }

        public static Guid SafeValue(this Guid? g)
        {
            return g.HasValue 
                ? g.Value 
                : Guid.Empty;
        }

        /// <summary>
        ///     ToXElement assists with returning null instances when no data exists in the Guid.
        ///     This assists in filtering empty nodes during the construction of a complex XElement.
        /// </summary>
        /// <param name="value">The content to be tested and if present placed within the node.</param>
        /// <param name="nodeName">The name of the XElement to find.</param>
        /// <param name="exceptionMessage">The exception message to return.</param>
        /// <returns>Returns an instance of an XElement if the is not <c>Guid.Empty</c>.</returns>
        public static XElement ToXElement(this Guid value, string nodeName, out string exceptionMessage)
        {
            XElement result;
            exceptionMessage = string.Empty;

            if (nodeName.HasNoValue())
            {
                // The XElement constructor will error if the node name does not contain valid characters
                exceptionMessage =
                    string.Format("The nodeName contained invalid characters: '{0}'", nodeName ?? "null");
                result = null;
            }
            else
            {
                // Business logic defines that the value must not be an empty guid
                try
                {
                    result = new XElement(nodeName, value);
                }
                catch (XmlException ex)
                {
                    // Some character sequences are not premitted in the node name (the GUID value will not cause an exception)
                    exceptionMessage =
                        string.Format("The XElement constructor thew an exception: nodeName = '{0}', message ='{1}',",
                            nodeName, ex.Message);
                    result = null;
                }
            }

            return result;
        }
    }
}