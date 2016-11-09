using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Provides list of users that has registered activity in specified time span
        /// </summary>
        /// <param name="activitySpan">Time span for activity</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the result of the asynchronous query, a list of online users</returns>
        Task<IQueryable<TUser>> GetOnlineUsers(TimeSpan activitySpan, CancellationToken cancellationToken);

        /// <summary>
        /// Updates user's last activity timestamp
        /// </summary>
        /// <param name="user">The user, whose activity is being updated</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task UpdateActivityTimeAsync(TUser user, CancellationToken cancellationToken);


        /// <summary>
        /// Resets user's last activity timestamp
        /// </summary>
        /// <param name="user">The user, whose activity is being reset</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task ResetActivityTimeAsync(TUser user, CancellationToken cancellationToken);


        /// <summary>
        /// Checks if user has registered activity in specified time span
        /// </summary>
        /// <param name="user">The user, whose activity is being checked</param>
        /// <param name="activitySpan">Time span for checking activity</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that represents the result of the asynchronous query, a flag indicating user's activity</returns>
        Task<bool> IsUserActiveAsync(TUser user, TimeSpan activitySpan, CancellationToken cancellationToken);


        /// <summary>
        /// Gets activity timestamp of the specified user
        /// </summary>
        /// <param name="user">The user, whose activity timestamp is being retrived</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>Activity timestamp of the specified user</returns>
        Task<DateTimeOffset?> GetUserActivityTimestampAsync(TUser user, CancellationToken cancellationToken);


    }
}
