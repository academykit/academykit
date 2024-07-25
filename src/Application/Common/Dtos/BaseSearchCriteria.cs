using Lingtren.Domain.Enums;

namespace Lingtren.Application.Common.Dtos
{
    /// <summary>
    /// Base search criteria
    /// </summary>
    public class BaseSearchCriteria
    {
        /// <summary>
        /// Gets or sets the current user id, this is used to filter the results based on current user and must be always provided to the service.
        /// </summary>
        /// <value></value>
        public Guid CurrentUserId { get; set; }

        /// <summary>
        /// Get or set page
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Get or set size
        /// </summary>
        public int Size { get; set; } = 10;

        /// <summary>
        /// Get or set search query
        /// </summary>
        //public string Search { get; set; }
        private string _search;

        public string Search
        {
            get => _search;
            set => _search = value?.Trim().ToLower();
        }

        /// <summary>
        /// Gets or sets the sort by property.
        /// </summary>
        public string SortBy { get; set; }

        /// <summary>
        /// Gets or sets the type of the sort.
        /// </summary>
        public SortType SortType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseSearchCriteria"/> class.
        /// </summary>
        public BaseSearchCriteria() { }
    }
}
