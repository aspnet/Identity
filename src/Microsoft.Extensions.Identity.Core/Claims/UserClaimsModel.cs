using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Microsoft.Extensions.Identity.Claims
{
    /// <summary>
    /// Represents the mappings from properties to <see cref="Claim"/>s
    /// for a given user <typeparamref name="TUser"/>.
    /// </summary>
    public class UserClaimsModel<TUser>
        where TUser : class
    {
        /// <summary>
        /// The list of claim formats supported.
        /// </summary>
        public IList<IClaimFormatter> ClaimFormats { get; }

        /// <summary>
        /// Gets the list of mappings of properties from the
        /// <typeparamref name="TUser"/> into <see cref="Claim"/>.
        /// </summary>
        public IList<ClaimsMapping> Mappings { get; }

        /// <summary>
        /// Configures a custom mapping.
        /// </summary>
        /// <param name="configureMapping">The function to produce claims based on a user.</param>
        public void Map(Func<TUser,IEnumerable<Claim>> configureMapping)
        {
            Mappings.Add(new CustomMapping(user => configureMapping((TUser)user)));
        }

        private class CustomMapping : ClaimsMapping
        {
            public CustomMapping(Func<object,IEnumerable<Claim>> mapping)
            {
                Mapping = mapping;
            }

            public Func<object, IEnumerable<Claim>> Mapping { get; }

            public override IEnumerable<Claim> Map(object user) => Mapping(user);
        }
    }
}
