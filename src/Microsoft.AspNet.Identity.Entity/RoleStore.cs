using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace Microsoft.AspNet.Identity.Entity
{
    public class RoleStore<TRole, TKey> : 
        IQueryableRoleStore<TRole, TKey>
        where TRole : class,IRole<TKey>
        where TKey : IEquatable<TKey>
    {
        private bool _disposed;

        public RoleStore(EntityContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
            AutoSaveChanges = true;
        }

        public EntityContext Context { get; private set; }

        /// <summary>
        ///     If true will call SaveChanges after Create/Update/Delete
        /// </summary>
        public bool AutoSaveChanges { get; set; }

        private async Task SaveChanges()
        {
            if (AutoSaveChanges)
            {
                await Context.SaveChangesAsync();
            }
        }

        public virtual Task<TRole> GetRoleAggregate(Expression<Func<TRole, bool>> filter)
        {
            return Roles.SingleOrDefaultAsync(filter);
        }

        public async virtual Task Create(TRole role)
        {
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            await Context.AddAsync(role);
            await SaveChanges();
        }

        public async virtual Task Update(TRole role)
        {
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            await Context.UpdateAsync(role);
            await SaveChanges();
        }

        public async virtual Task Delete(TRole role)
        {
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            // TODO: why is there no DeleteAsync?
            Context.Delete(role);
            await SaveChanges();
        }

        /// <summary>
        ///     Find a role by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual Task<TRole> FindById(TKey id)
        {
            ThrowIfDisposed();
            return GetRoleAggregate(u => u.Id.Equals(id));
        }

        /// <summary>
        ///     Find a role by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual Task<TRole> FindByName(string name)
        {
            ThrowIfDisposed();
            return GetRoleAggregate(u => u.Name.ToUpper() == name.ToUpper());
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        /// <summary>
        ///     Dispose the store
        /// </summary>
        public void Dispose()
        {
            _disposed = true;
        }

        public IQueryable<TRole> Roles
        {
            get { return Context.Set<TRole>(); }
        }
    }
}
