using System.Xml;
using System.Xml.Linq;

namespace Platform.Common
{
    /// <summary>
    ///     Extensions for Booleans.
    /// </summary>
    public static class BooleanExtensions
    {
        /// <summary>
        ///     Converts the boolean to an Integer.
        /// </summary>
        /// <param name="b">The extension method boolean.</param>
        /// <returns>The bool as an Int.</returns>
        public static int ToInt(this bool b)
        {
            return b ? 1 : 0;
        }

        /// <summary>
        ///     Returns an XElement instance named NodeName containing Value or <c>null</c> if Value is false.
        /// </summary>
        /// <param name="value">The value placed within the node.</param>
        /// <param name="nodeName">The name of the XElement.</param>
        /// <param name="exceptionMessage">The exception message to return.</param>
        /// <returns>
        ///     An XElement instance named NodeName containing Value or <c>null</c> if Value is false.
        /// </returns>
        public static XElement ToXElement(this bool value, string nodeName, out string exceptionMessage)
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
                // Business logic requires the value to be true
                try
                {
                    result = new XElement(nodeName, value);
                }
                catch (XmlException ex)
                {
                    // Some character sequences are not premitted in the node name (the bool value will not cause an exception)
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