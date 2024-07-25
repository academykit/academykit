namespace Lingtren.Application.Common.Dtos
{
    /// <summary>
    /// An entity class that represents search result.
    /// </summary>
    ///
    /// <typeparam name="T">The type of the items in the search result.</typeparam>
    ///
    /// <remarks>
    /// Note that the properties are implemented without any validation.
    /// </remarks>
    public class SearchResult<T>
    {
        /// <summary>
        /// Get or set current page
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Get or set page size
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Get or set total page
        /// </summary>
        public int TotalPage { get; set; }

        /// <summary>
        /// Get or set total count
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public IList<T> Items { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchResult{T}"/> class.
        /// </summary>
        public SearchResult() { }
    }
}
