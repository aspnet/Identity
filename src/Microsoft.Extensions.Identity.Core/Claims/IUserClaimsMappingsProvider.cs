using System.Security.Claims;

namespace Microsoft.Extensions.Identity.Claims
{
    /// <summary>
    /// A provider for mappings between a <typeparamref name="TUser"/> and <see cref="Claim"/>.
    /// </summary>
    /// <typeparam name="TUser">The type of the mappings.</typeparam>
    public interface IUserClaimsMappingsProvider<TUser> where TUser : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="claimMappings"></param>
        void ConfigureMappings(UserClaimsModel<TUser> claimMappings);
    }
}