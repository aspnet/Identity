﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace Microsoft.AspNet.Identity.Entity
{
    public class RoleStore<TRole, TKey> : 
        IQueryableRoleStore<TRole>
        where TRole : IdentityRole
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
        ///     If true will call SaveChanges after CreateAsync/UpdateAsync/DeleteAsync
        /// </summary>
        public bool AutoSaveChanges { get; set; }

        private async Task SaveChanges(CancellationToken cancellationToken)
        {
            if (AutoSaveChanges)
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
        }

        public virtual Task<TRole> GetRoleAggregate(Expression<Func<TRole, bool>> filter, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Roles.SingleOrDefaultAsync(filter, cancellationToken);
        }

        public async virtual Task Create(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            await Context.AddAsync(role, cancellationToken);
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
            await Context.UpdateAsync(role, cancellationToken);
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
            Context.Delete(role);
            await SaveChanges(cancellationToken);
        }

        public Task<string> GetRoleId(TRole role, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.FromResult(role.Id);
        }

        public Task<string> GetRoleName(TRole role, CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.FromResult(role.Name);
        }


        public virtual TKey ConvertId(string userId)
        {
            return (TKey)Convert.ChangeType(userId, typeof(TKey));
        }

        /// <summary>
        ///     FindByLoginAsync a role by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<TRole> FindById(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var roleId = ConvertId(id);
            return Roles.SingleOrDefaultAsync(r => r.Id.Equals(roleId), cancellationToken);
            //return GetRoleAggregate(u => u.Id.Equals(id));
        }

        /// <summary>
        ///     FindByLoginAsync a role by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<TRole> FindByName(string name, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Roles.SingleOrDefaultAsync(r => r.Name.ToUpper() == name.ToUpper(), cancellationToken);
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
