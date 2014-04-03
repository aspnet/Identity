using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
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

        private async Task SaveChanges(CancellationToken cancellationToken)
        {
            if (AutoSaveChanges)
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
        }

        public virtual Task<TRole> GetRoleAggregate(Expression<Func<TRole, bool>> filter)
        {
            return Roles.SingleOrDefaultAsync(filter);
        }

        public async virtual Task Create(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            await Context.AddAsync(role);
            await SaveChanges(cancellationToken);
        }

        public async virtual Task Update(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            await Context.UpdateAsync(role);
            await SaveChanges(cancellationToken);
        }

        public async virtual Task Delete(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            // TODO: why is there no DeleteAsync?
            Context.Delete(role);
            await SaveChanges(cancellationToken);
        }

        /// <summary>
        ///     Find a role by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<TRole> FindById(TKey id, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Task.FromResult(Roles.SingleOrDefault(r => r.Id.Equals(id)));
            //return GetRoleAggregate(u => u.Id.Equals(id));
        }

        /// <summary>
        ///     Find a role by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<TRole> FindByName(string name, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Task.FromResult(Roles.SingleOrDefault(r => r.Name.ToUpper() == name.ToUpper()));
            //return GetRoleAggregate(u => u.Name.ToUpper() == name.ToUpper());
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
