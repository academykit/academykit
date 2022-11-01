namespace Lingtren.Application.Common.Interfaces
{
    using Lingtren.Application.Common.Dtos;
    using Microsoft.EntityFrameworkCore;
    public static class IQueryablePageListExtensions
    {
        /// <summary>
        /// Converts the specified source to <see cref="SearchResult{T}"/> by the specified <paramref name="pageIndex"/> and <paramref name="pageSize"/>.
        /// </summary>
        /// <typeparam name="T">The type of the source.</typeparam>
        /// <param name="source">The source to paging.</param>
        /// <param name="pageNumber">The current number of the page.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An instance of the inherited from <see cref="PagedList{T}"/> interface.</returns>
        public static async Task<SearchResult<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageNumber,
        int pageSize, CancellationToken cancellationToken = default(CancellationToken))
        {
            var count = await source.CountAsync(cancellationToken).ConfigureAwait(false);
            var items = await source.Skip((pageNumber - 1) * pageSize)
                                    .Take(pageSize).ToListAsync(cancellationToken).ConfigureAwait(false);

            var pagedList = new SearchResult<T>()
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = count,
                Items = items,
                TotalPage = (int)Math.Ceiling(count / (double)pageSize)
            };

            return pagedList;
        }

        /// <summary>
        /// Converts the specified source to <see cref="SearchResult{T}"/> by the specified <paramref name="pageIndex"/> and <paramref name="pageSize"/>.
        /// </summary>
        /// <typeparam name="T">The type of the source.</typeparam>
        /// <param name="source">The source to paging.</param>
        /// <param name="pageNumber">The current number of the page.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An instance of the inherited from <see cref="PagedList{T}"/> interface.</returns>
        public static SearchResult<T> ToPagedList<T>(this IQueryable<T> source, int pageNumber,
        int pageSize, CancellationToken cancellationToken = default(CancellationToken))
        {
            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize)
                                    .Take(pageSize).ToList();

            var pagedList = new SearchResult<T>()
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = count,
                Items = items,
                TotalPage = (int)Math.Ceiling(count / (double)pageSize)
            };

            return pagedList;
        }

        /// <summary>
        /// Converts the specified source to <see cref="SearchResult{T}"/> by the specified <paramref name="pageIndex"/> and <paramref name="pageSize"/>.
        /// </summary>
        /// <typeparam name="T">The type of the source.</typeparam>
        /// <param name="source">The source to paging.</param>
        /// <param name="pageNumber">The current number of the page.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An instance of the inherited from <see cref="PagedList{T}"/> interface.</returns>
        public static SearchResult<T> ToIPagedList<T>(this IList<T> source, int pageNumber,
        int pageSize, CancellationToken cancellationToken = default(CancellationToken))
        {
            var count = source.Count;
            var items = source.Skip((pageNumber - 1) * pageSize)
                                    .Take(pageSize).ToList();

            var pagedList = new SearchResult<T>()
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = count,
                Items = items,
                TotalPage = (int)Math.Ceiling(count / (double)pageSize)
            };

            return pagedList;
        }
    }
}
