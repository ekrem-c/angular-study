namespace Platform.Common
{
    /// <summary>
    ///     The Operation Result result returned when performing a <c>DeepCompareOperationalResult</c>
    /// </summary>
    public class DeepCompareOperationalResult
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DeepCompareOperationalResult" /> class
        /// </summary>
        public DeepCompareOperationalResult()
        {
            Differences = string.Empty;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the two items under test were functionally identical
        /// </summary>
        public bool ObjectsAreIdentical { get; set; }

        /// <summary>
        ///     Gets or sets a string that details all the differences found between two objects
        /// </summary>
        public string Differences { get; set; }
    }
}