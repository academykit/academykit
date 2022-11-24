namespace Lingtren.Infrastructure.Services
{
    using Lingtren.Application.Common.Interfaces;
    using Lingtren.Infrastructure.Common;
    using Lingtren.Infrastructure.Persistence;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class UnitOfWork : Disposable, IUnitOfWork
    {
        /// <summary>
        /// Gets the db context.
        /// </summary>
        /// <returns>The instance of type <cref name="ApplicationDbContext"/>.</returns>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Gets the db context.
        /// </summary>
        /// <returns>The instance of type <cref name="ApplicationDbContext"/>.</returns>
        ApplicationDbContext IUnitOfWork.DbContext => _context;

        /// <summary>
        /// The repositories
        /// </summary>
        private Dictionary<Type, object> repositories;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork" /> class.
        /// </summary>
        /// <param name="context">The db context.</param>
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Gets the specified repository for the <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>An instance of type inherited from <see cref="IRepository{TEntity}"/> interface.</returns>
        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            repositories ??= new Dictionary<Type, object>();

            var type = typeof(TEntity);
            if (!repositories.ContainsKey(type))
            {
                repositories[type] = new Repository<TEntity>(_context);
            }

            return (IRepository<TEntity>)repositories[type];
        }

        /// <summary>
        /// Saves all pending changes
        /// </summary>
        /// <returns>The number of objects in an Added, Modified, or Deleted state</returns>
        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        /// <summary>
        /// Saves all pending changes
        /// </summary>
        /// <returns>The number of objects in an Added, Modified, or Deleted state</returns>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        public override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
    }
}
