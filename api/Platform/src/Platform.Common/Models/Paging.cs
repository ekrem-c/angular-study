namespace Platform.Common
{
    public class Paging
    {
        /// <summary>
        ///     Gets or sets the page.
        /// </summary>
        /// <value>The page.</value>
        public int Page { get; set; } = 1;

        /// <summary>
        ///     Gets or sets the size of the page.
        /// </summary>
        /// <value>The size of the page.</value>
        public int PageSize { get; set; } = 10;
    }
}