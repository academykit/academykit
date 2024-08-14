namespace AcademyKit.Infrastructure.Helpers
{
    using System.Data;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using AcademyKit.Application.Common.Dtos;
    using AcademyKit.Application.Common.Exceptions;
    using AcademyKit.Domain.Common;
    using AcademyKit.Infrastructure.Common;
    using Newtonsoft.Json;

    /// <summary>
    /// This class is the helper class for this project.
    /// </summary>
    ///
    /// <threadsafety>
    /// This class is immutable and thread safe.
    /// </threadsafety>
    public static partial class CommonHelper
    {
        /// <summary>
        /// Represents the request property name for the current user.
        /// </summary>
        public const string CurrentUserPropertyName = "CurrentUser";
        private const string SemanticVersionPattern = @"^\d+\.\d+\.\d+(-\w+)?$"; // Basic semver regex

        [GeneratedRegex(SemanticVersionPattern)]
        public static partial Regex SemanticVersionRegex();

        /// <summary>
        /// Represents the JSON serializer settings.
        /// </summary>
        private static readonly JsonSerializerSettings SerializerSettings =
            new()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DateFormatString = "MM/dd/yyyy HH:mm:ss",
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };

        /// <summary>
        /// Checks whether the given search criteria is <c>null</c> or incorrect.
        /// </summary>
        ///
        /// <param name="criteria">The search criteria to check.</param>
        ///
        /// <exception cref="ArgumentNullException">If the <paramref name="criteria"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If the <paramref name="criteria"/> is incorrect,
        /// e.g. PageNumber is negative, or PageNumber is positive and PageSize is not positive.</exception>
        public static void CheckSearchCriteria(BaseSearchCriteria criteria)
        {
            ValidateArgumentNotNull(criteria, nameof(criteria));

            if (criteria.Page < 0)
            {
                throw new ArgumentException("Page number can't be negative.", nameof(criteria));
            }

            if (criteria.Page > 0 && criteria.Size < 1)
            {
                throw new ArgumentException(
                    "Page size should be positive, if page number is positive.",
                    nameof(criteria)
                );
            }
        }

        /// <summary>
        /// Checks whether the found entity is not null.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The found entity.</param>
        /// <exception cref="EntityNotFoundException">If <paramref name="entity"/> is null.</exception>
        public static void CheckFoundEntity<T>(T entity)
        {
            if (entity == null)
            {
                throw new EntityNotFoundException($"{typeof(T).Name} was not found.");
            }
        }

        /// <summary>
        /// Checks whether the found entity is not null.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="entity">The found entity.</param>
        /// <param name="entityId">The entity identifier.</param>
        /// <exception cref="EntityNotFoundException">If <paramref name="entity"/> is null.</exception>
        public static void CheckFoundEntity<T>(T entity, Guid entityId)
            where T : IdentifiableEntity
        {
            if (entity == null)
            {
                throw new EntityNotFoundException(
                    $"{typeof(T).Name} with Id='{entityId}' was not found."
                );
            }
        }

        /// <summary>
        /// Validates that <paramref name="param"/> is valid Guid.
        /// </summary>
        ///
        /// <param name="param">The parameter to validate.</param>
        /// <param name="paramName">The name of the parameter.</param>
        ///
        /// <exception cref="ArgumentException">If <paramref name="param"/> is not positive number.</exception>
        public static void ValidateArgumentGuid(Guid param, string paramName)
        {
            if (param == Guid.Empty)
            {
                throw new ArgumentException($"{paramName} must be not empty.", paramName);
            }
        }

        /// <summary>
        /// Validates that <paramref name="param"/> is positive number.
        /// </summary>
        ///
        /// <param name="param">The parameter to validate.</param>
        /// <param name="paramName">The name of the parameter.</param>
        ///
        /// <exception cref="ArgumentException">If <paramref name="param"/> is not positive number.</exception>
        public static void ValidateArgumentPositive(long param, string paramName)
        {
            if (param <= 0)
            {
                throw new ArgumentException($"{paramName} should be positive.", paramName);
            }
        }

        /// <summary>
        /// Validates that <paramref name="param"/> is not negative.
        /// </summary>
        ///
        /// <param name="param">The parameter to validate.</param>
        /// <param name="paramName">The name of the parameter.</param>
        ///
        /// <exception cref="ArgumentException">If <paramref name="param"/> is negative number.</exception>
        public static void ValidateArgumentNotNegative(decimal param, string paramName)
        {
            if (param < 0)
            {
                throw new ArgumentException($"{paramName} should not be negative.", paramName);
            }
        }

        /// <summary>
        /// Validates that <paramref name="param"/> is not <c>null</c>.
        /// </summary>
        ///
        /// <typeparam name="T">The type of the parameter, must be reference type.</typeparam>
        ///
        /// <param name="param">The parameter to validate.</param>
        /// <param name="paramName">The name of the parameter.</param>
        ///
        /// <exception cref="ArgumentNullException">If <paramref name="param"/> is <c>null</c>.</exception>
        public static void ValidateArgumentNotNull<T>(T param, string paramName)
            where T : class
        {
            if (param == null)
            {
                throw new ArgumentNullException(paramName, $"{paramName} cannot be null.");
            }
        }

        /// <summary>
        /// Validates that <paramref name="param"/> is not <c>null</c> or empty.
        /// </summary>
        ///
        /// <param name="param">The parameter to validate.</param>
        /// <param name="paramName">The name of the parameter.</param>
        ///
        /// <exception cref="ArgumentNullException">If <paramref name="param"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="param"/> is empty.</exception>
        public static void ValidateArgumentNotNullOrEmpty(string param, string paramName)
        {
            ValidateArgumentNotNull(param, paramName);
            if (string.IsNullOrWhiteSpace(param))
            {
                throw new ArgumentException($"{paramName} cannot be empty.", paramName);
            }
        }

        /// <summary>
        /// Use Expression to add extension method to IQueryable.
        /// So it support order by nested property name.
        /// </summary>
        /// <typeparam name="T">The type of queried entities.</typeparam>
        /// <param name="source">The Queryable source.</param>
        /// <param name="orderName">The order name.</param>
        /// <param name="colName">The column name.</param>
        /// <returns>The new IQueryable with applied ordering.</returns>
        /// <remarks>Thrown exceptions will be propagated.</remarks>
        private static IQueryable<T> OrderbyFromColumnName<T>(
            IQueryable<T> source,
            string orderName,
            string colName
        )
        {
            var props = colName.Split('.');
            var type = typeof(T);
            var arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            foreach (var prop in props)
            {
                var pi = type.GetPublicProperties()
                    .FirstOrDefault(p => p.Name.Equals(prop, StringComparison.OrdinalIgnoreCase));
                if (pi == null)
                {
                    throw new ServiceException($"'{colName}' is not a valid SortBy value.");
                }

                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }

            var delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            var lambda = Expression.Lambda(delegateType, expr, arg);
            var resultExp = Expression.Call(
                typeof(Queryable),
                orderName,
                new[] { typeof(T), type },
                source.Expression,
                lambda
            );
            return source.Provider.CreateQuery<T>(resultExp);
        }

        /// <summary>
        /// Gets all public properties of the given type.
        /// </summary>
        /// <param name="type">The type to get properties for.</param>
        /// <returns>All public properties of the given type.</returns>
        private static IEnumerable<PropertyInfo> GetPublicProperties(this Type type)
        {
            if (!type.IsInterface)
            {
                return type.GetProperties();
            }

            return (new Type[] { type })
                .Concat(type.GetInterfaces())
                .SelectMany(i => i.GetProperties());
        }

        /// <summary>
        /// Applies OrderBy extension method to the given Queryable source.
        /// </summary>
        /// <typeparam name="T">The type of queried entities.</typeparam>
        /// <param name="source">The Queryable source.</param>
        /// <param name="ordering">The order column name.</param>
        /// <returns>The new Queryable with applied OrderBy.</returns>
        /// <remarks>Thrown exceptions will be propagated.</remarks>
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string ordering)
        {
            return OrderbyFromColumnName(source, "OrderBy", ordering);
        }

        /// <summary>
        /// Applies OrderByDescending extension method to the given Queryable source.
        /// </summary>
        /// <typeparam name="T">The type of queried entities.</typeparam>
        /// <param name="source">The Queryable source.</param>
        /// <param name="ordering">The order column name.</param>
        /// <returns>The new Queryable with applied OrderByDescending.</returns>
        /// <remarks>Thrown exceptions will be propagated.</remarks>
        public static IQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string ordering)
        {
            return OrderbyFromColumnName(source, "OrderByDescending", ordering);
        }

        /// <summary>
        /// Adds items to the list.
        /// </summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="items">The items to add.</param>
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                list.Add(item);
            }
        }

        /// <summary>
        /// Gets JSON description of the object.
        /// </summary>
        ///
        /// <param name="obj">The object to describe.</param>
        /// <returns>The JSON description of the object.</returns>
        public static string GetObjectDescription(object obj)
        {
            try
            {
                if (obj is System.IO.Stream stream)
                {
                    return stream.Length.ToString();
                }

                return JsonConvert.SerializeObject(obj, SerializerSettings);
            }
            catch
            {
                return "[Can't express this value]";
            }
        }

        /// <summary>
        /// Generates the slug name based for entity
        /// </summary>
        /// <typeparam name="TEntity">The entity type</typeparam>
        /// <param name="unitOfWork">The unit of work</param>
        /// <param name="checkIfExists">The function to check if the slug already exist or not</param>
        /// <param name="name">The initial title to generate slug from.</param>
        /// <param name="counter">The initial value of slug index</param>
        public static string GetEntityTitleSlug<TEntity>(
            IUnitOfWork unitOfWork,
            Func<string, Expression<Func<TEntity, bool>>> checkIfExists,
            string name,
            int counter = 0
        )
            where TEntity : class
        {
            var title = name;
            if (counter != 0)
            {
                title = $"{title} {counter}";
            }

            var slug = title.Slugify();
            var repo = unitOfWork.GetRepository<TEntity>();
            var exists = repo.Exists(checkIfExists(slug));
            if (exists)
            {
                counter++;
                return GetEntityTitleSlug(unitOfWork, checkIfExists, name, counter);
            }

            return slug;
        }

        /// <summary>
        /// Generates the slug name based for entity
        /// </summary>
        /// <typeparam name="TEntity">The entity type</typeparam>
        /// <param name="unitOfWork">The unit of work</param>
        /// <param name="checkIfExists">The function to check if the slug already exist or not</param>
        /// <param name="name">The initial title to generate slug from.</param>
        /// <param name="counter">The initial value of slug index</param>
        public static async Task<string> GetEntityTitleSlugAsync<TEntity>(
            IUnitOfWork unitOfWork,
            Func<string, Expression<Func<TEntity, bool>>> checkIfExists,
            string name,
            int counter = 0
        )
            where TEntity : class
        {
            var title = name;
            if (counter != 0)
            {
                title = $"{title} {counter}";
            }

            var slug = title.Slugify();
            var repo = unitOfWork.GetRepository<TEntity>();
            var exists = await repo.ExistsAsync(checkIfExists(slug)).ConfigureAwait(false);
            if (exists)
            {
                counter++;
                return await GetEntityTitleSlugAsync(unitOfWork, checkIfExists, name, counter)
                    .ConfigureAwait(false);
            }

            return slug;
        }

        // T is a generic class
        public static DataTable ConvertToDataTable<T>(List<T> models)
        {
            // creating a data table instance and typed it as our incoming model
            // as I make it generic, if you want, you can make it the model typed you want.
            DataTable dataTable = new(typeof(T).Name);

            //Get all the properties of that model
            var Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Loop through all the properties
            // Adding Column name to our data-table
            foreach (var prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            // Adding Row and its value to our dataTable
            foreach (var item in models)
            {
                var values = new object[Props.Length];
                for (var i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                // Finally add value to datatable
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        /// <summary>
        /// check email format
        /// </summary>
        /// <param name="email">email</param>
        /// <returns>bool</returns>
        public static bool ValidateEmailFormat(string email)
        {
            var emailPattern =
                @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}"
                + @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\"
                + @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            return Regex.IsMatch(email, emailPattern);
        }

        /// <summary>
        /// Sanitize the user provided value to prevent Log Injection in .NET
        /// </summary>
        /// <param name="value">The text to sanitize</param>
        /// <returns>Sanitized string</returns>
        public static string SanitizeForLogger(this string value)
        {
            return value.Replace("\n", "").Replace("\r", "");
        }

        /// <summary>
        /// Filter the provided tags to get the latest semantic version
        /// </summary>
        /// <param name="tags">the tags to be filtered</param>
        /// <returns>The tag of latest semantic version</returns>
        public static string FilterLatestSemanticVersion(IEnumerable<string> tags)
        {
            var semanticVersions = tags.Select(static tag =>
                {
                    var lastColonIndex = tag.LastIndexOf(':');
                    if (lastColonIndex >= 0)
                    {
                        return tag[(lastColonIndex + 1)..];
                    }
                    else
                    {
                        return tag;
                    }
                })
                .Where(tag => SemanticVersionRegex().IsMatch(tag)) // Filter out non-semver tags
                .Select(tag => new Version(tag.Split('-')[0])) // Parse versions, ignore pre-release
                .OrderByDescending(v => v) // Sort in descending order
                .ToList();

            return semanticVersions.Count > 0 ? semanticVersions.First().ToString() : null;
        }
    }
}
