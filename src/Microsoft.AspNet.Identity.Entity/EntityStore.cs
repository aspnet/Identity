using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.SqlServer;

namespace Microsoft.AspNet.Identity.Entity
{
    public class EFContext : DbContext
    {
        public DbSet<EFUser> Users { get; set; }
        //public DbSet<TRole> Roles { get; set; }

        public EFContext(IServiceProvider serviceProvider)
        : base(serviceProvider) { }

        public EFContext() : base() { }

        protected override void OnConfiguring(EntityConfigurationBuilder builder)
        {
            builder.SqlServerConnectionString(@"Server=(localdb)\v11.0;Database=SimpleIdentity1;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<EFUser>()
                .Key(u => u.Id)
                .Properties(ps => ps.Property(u => u.UserName));
        }
    }

    public class EFUser
    {
        public EFUser()
        {
            UserName = "";
            Id = "";
            Email = "";
            PasswordHash = "";
            SecurityStamp = "";
        }

        public virtual string Id { get; set; }
        public virtual string UserName { get; set; }

        /// <summary>
        ///     Email
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        ///     True if the email is confirmed, default is false
        /// </summary>
        public virtual int EmailConfirmed { get; set; }

        /// <summary>
        ///     The salted/hashed form of the user password
        /// </summary>
        public virtual string PasswordHash { get; set; }

        /// <summary>
        ///     A random value that should change whenever a users credentials have changed (password changed, login removed)
        /// </summary>
        public virtual string SecurityStamp { get; set; }
    }

    public class EntityStore<TUser> : IQueryableUserStore<TUser>, IUserSecurityStampStore<TUser>, IUserEmailStore<TUser>, IUserPasswordStore<TUser> where TUser : EFUser
    {
        public EntityStore(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
        }

        public DbContext Context { get; private set; }

        public void Dispose()
        {
        }

        protected virtual Task<TUser> GetUserAggregate(Expression<Func<TUser, bool>> filter, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(Users.SingleOrDefault(filter));
        }

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.UserName);
        }

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken = new CancellationToken())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.UserName = userName;
            return Task.FromResult(0);
        }

        public async virtual Task CreateAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            await Context.AddAsync(user, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
        }

        public async virtual Task UpdateAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            await Context.UpdateAsync(user, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
        }

        public async virtual Task DeleteAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            Context.Delete(user);
            await Context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        ///     Find a user by id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            return GetUserAggregate(u => u.Id.Equals(userId), cancellationToken);
        }

        /// <summary>
        ///     Find a user by name
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<TUser> FindByNameAsync(string userName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            return GetUserAggregate(u => u.UserName.ToUpper() == userName.ToUpper(), cancellationToken);
        }

        public IQueryable<TUser> Users
        {
            get { return Context.Set<TUser>(); }
        }

        /// <summary>
        ///     Set the security stamp for the user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="stamp"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Get the security stamp for a user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.SecurityStamp);
        }

        /// <summary>
        ///     Returns whether the user email is confirmed
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.EmailConfirmed != 0);
        }

        /// <summary>
        ///     Set IsEmailConfirmed on the user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="confirmed"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.EmailConfirmed = (confirmed ? 1 : 0);
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Set the user email
        /// </summary>
        /// <param name="user"></param>
        /// <param name="email"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.Email = email;
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Get the user's email
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.Email);
        }

        /// <summary>
        ///     FindByLoginAsync a user by email
        /// </summary>
        /// <param name="email"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<TUser> FindByEmailAsync(string email, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            return GetUserAggregate(u => u.Email.ToUpper() == email.ToUpper(), cancellationToken);
        }

        /// <summary>
        ///     Set the password hash for a user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="passwordHash"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Get the password hash for a user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.PasswordHash);
        }

        /// <summary>
        ///     Returns true if the user has a password set
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.PasswordHash != null);
        }

    }
}

