using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.Claims;

namespace Microsoft.Extensions.Identity.Claims
{
    /// <summary>
    /// Default implementation for <see cref="IUserClaimsMapper{TUser}"/>
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    public class UserClaimsMapper<TUser> : IUserClaimsMapper<TUser> where TUser : class
    {
        private UserClaimsModel<TUser> _model;

        /// <summary>
        /// Initializes a new instance of <see cref="UserClaimsMapper{TUser}"/>.
        /// </summary>
        /// <param name="providers">The list of <see cref="IUserClaimsMappingsProvider{TUser}"/> for this
        /// <see cref="IUserClaimsMapper{TUser}"/>.</param>
        public UserClaimsMapper(IEnumerable<IUserClaimsMappingsProvider<TUser>> providers) => BuildMapingModel(providers);

        private void BuildMapingModel(IEnumerable<IUserClaimsMappingsProvider<TUser>> providers)
        {
            var model = new UserClaimsModel<TUser>();
            foreach (var provider in providers)
            {
                provider.ConfigureMappings(model);
            }

            _model = model;
        }

        /// <inheritdoc />
        public void Map(TUser user, ClaimsIdentity identity)
        {
            foreach (var mapping in _model.Mappings)
            {
                identity.AddClaims(mapping.Map(user));
            }
        }
    }
}
