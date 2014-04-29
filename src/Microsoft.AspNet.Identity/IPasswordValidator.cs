using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Identity
{
    /// <summary>
    ///     Used to validate passwords
    /// </summary>
    public interface IPasswordValidator<TUser> where TUser : class
    {
        /// <summary>
        ///     Validate the item
        /// </summary>
        /// <returns></returns>
        Task<IdentityResult> ValidateAsync(string password, UserManager<TUser> manager, CancellationToken cancellationToken = default(CancellationToken));
    }
}