using System;

namespace Microsoft.Extensions.Identity
{
    /// <summary>
    /// Represents the mapping of a property into a claim.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class UserClaimAttribute : Attribute, IClaimMappingMetadata
    {
        /// <summary>
        /// Creates a new instance of <see cref="UserClaimAttribute"/>.
        /// </summary>
        public UserClaimAttribute()
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="UserClaimAttribute"/>.
        /// </summary>
        /// <param name="claimName">The name of the claim mapped to the property
        /// marked by this attribute.</param>
        public UserClaimAttribute(string claimName)
        {
            if (claimName == null)
            {
                throw new ArgumentNullException(nameof(claimName));
            }

            Name = claimName;
        }

        /// <summary>
        /// Creates a new instance of <see cref="UserClaimAttribute"/>.
        /// </summary>
        /// <param name="claimName">The name of the claim mapped to the property
        /// marked by this attribute.</param>
        /// <param name="format">The format to use to generate the value of the
        /// claim.</param>
        public UserClaimAttribute(string claimName, string format)
        {
            if (claimName == null)
            {
                throw new ArgumentNullException(nameof(claimName));
            }

            if (format == null)
            {
                throw new ArgumentNullException(nameof(format));
            }

            Name = claimName;
            Format = format;
        }

        /// <summary>
        /// Gets the name of the claim the property maps to.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The format to apply to produce a value for this claim.
        /// </summary>
        public string Format { get; set; }
    }
}
