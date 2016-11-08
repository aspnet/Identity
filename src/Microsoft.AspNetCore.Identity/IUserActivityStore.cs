using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Identity
{
    /// <summary>
    /// Provides an abstraction for a storing information which can be used to determine
    /// which users are online.
    /// </summary>
    /// <typeparam name="TUser">The type that represents a user.</typeparam>
    public interface IUserActivityStore<TUser> : IUserStore<TUser> where TUser : class
    {

        /// <summary>
        /// Provides list of users that has shown activity during specified time span
        /// </summary>
        /// <param name="idleTimeout"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>List of online users</returns>
        /// 
        Task<IList<TUser>> GetOnlineUsers(TimeSpan idleTimeout, CancellationToken cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IdentityResult> UpdateActivityTimeAsync(TUser user, CancellationToken cancellationToken);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IdentityResult> ResetActivityTimeAsync(TUser user, CancellationToken cancellationToken);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="activitySpan"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> IsUserActiveAsync(TUser user, TimeSpan activitySpan, CancellationToken cancellationToken);


    }
}
