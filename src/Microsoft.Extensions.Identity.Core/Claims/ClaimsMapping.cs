using System.Collections.Generic;
using System.Security.Claims;

namespace Microsoft.Extensions.Identity.Claims
{
    /// <summary>
    /// Represents the mapping of a given property to a <see cref="Claim"/>
    /// </summary>
    public abstract class ClaimsMapping
    {
        /// <summary>
        /// Maps one or more properties from the <paramref name="user"/>
        /// into one or more <see cref="Claim"/>
        /// </summary>
        /// <param name="user">The user from which to map properties from.</param>
        /// <returns></returns>
        public abstract IEnumerable<Claim> Map(object user);
    }
}