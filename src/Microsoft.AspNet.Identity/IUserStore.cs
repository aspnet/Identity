using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Identity
{
    /// <summary>
    ///     Interface that exposes basic user management apis
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IUserStore<TUser> : IDisposable where TUser : class
    {
        /// <summary>
        ///     Returns the user id for a user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Returns the user's name
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string> GetUserName(TUser user, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Set the user name
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Insert a new user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Create(TUser user, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     UpdateAsync a user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Update(TUser user, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     DeleteAsync a user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Delete(TUser user, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Finds a user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TUser> FindById(string userId, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Returns the user associated with this name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TUser> FindByName(string name, CancellationToken cancellationToken = default(CancellationToken));

    }
}