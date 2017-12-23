using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;

namespace Microsoft.Extensions.Identity.Claims
{
    /// <summary>
    /// <see cref="IUserClaimsMappingsProvider{TUser}"/> that uses attributes on the <typeparamref name="TUser"/>
    /// to define mappings of properties into claims.
    /// </summary>
    /// <typeparam name="TUser">The type of the user.</typeparam>
    public class AttributeUserClaimsMappingProvider<TUser> : IUserClaimsMappingsProvider<TUser> where TUser : class
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AttributeUserClaimsMappingProvider{TUser}"/>.
        /// </summary>
        /// <param name="claimNameFormatter">The <see cref="IClaimNameFormatter"/> to use to format names.</param>
        public AttributeUserClaimsMappingProvider(IClaimNameFormatter claimNameFormatter)
        {
            NameFormatter = claimNameFormatter;
        }

        /// <summary>
        /// Gets the <see cref="IClaimNameFormatter"/> used to format claim names.
        /// </summary>
        public IClaimNameFormatter NameFormatter { get; }

        /// <inheritdoc />
        public void ConfigureMappings(UserClaimsModel<TUser> claimMappings)
        {
            var valueFormatter = new Formatter(claimMappings.ClaimFormats);
            var mappings = GenerateMappings(
                typeof(TUser),
                valueFormatter);

            foreach (var mapping in mappings)
            {
                claimMappings.Mappings.Add(mapping);
            }
        }

        private IList<ClaimsMapping> GenerateMappings(
            Type type,
            Formatter formatter)
        {
            var result = new List<ClaimsMapping>();
            var info = type.GetTypeInfo();
            var properties = info.GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                var attributes = properties[i].GetCustomAttributes();
                IClaimMappingMetadata metadata = null;
                foreach (var attribute in attributes)
                {
                    if (attribute is IClaimMappingMetadata metadataValue)
                    {
                        metadata = metadataValue;
                    }
                }

                if (metadata != null)
                {
                    var mapping = CreateMapping(
                        properties[i],
                        metadata,
                        formatter);

                    result.Add(mapping);
                }
            }

            return result;
        }

        private ClaimsMapping CreateMapping(
            PropertyInfo propertyInfo,
            IClaimMappingMetadata metadata,
            Formatter formatter)
        {
            if ((propertyInfo.PropertyType.IsPrimitive || 
                propertyInfo.PropertyType.Equals(typeof(string))) ||
                propertyInfo.PropertyType.Equals(typeof(DateTime)) ||
                propertyInfo.PropertyType.Equals(typeof(DateTimeOffset)) &&
                    !propertyInfo.PropertyType.IsPointer)
            {
                return new PropertyClaimsMapping(
                    propertyInfo,
                    metadata,
                    NameFormatter,
                    formatter.GetFormat(metadata.Format));
            }

            throw new InvalidOperationException($"Can't create a mapping for property {propertyInfo.Name} of type:{propertyInfo.PropertyType}.");
        }

        private class PropertyClaimsMapping : ClaimsMapping
        {
            public PropertyClaimsMapping(
                PropertyInfo property,
                IClaimMappingMetadata metadata,
                IClaimNameFormatter formatter,
                IClaimValueFormatter valueFormatter)
            {
                Property = property;
                Metadata = metadata;
                Formatter = formatter;
                ValueFormatter = valueFormatter;
            }

            public PropertyInfo Property { get; }
            public IClaimMappingMetadata Metadata { get; }
            public IClaimNameFormatter Formatter { get; }
            public IClaimValueFormatter ValueFormatter { get; }

            public override IEnumerable<Claim> Map(object user)
            {
                var value = Property.GetValue(user);

                var claimValue = Metadata.Format == null ? value?.ToString() : ValueFormatter.Format(value);
                if (claimValue != null)
                {
                    var claimName = Metadata.Name ?? Formatter.Format(Property.Name);
                    yield return new Claim(claimName, claimValue);
                }
            }
        }

        private class Formatter
        {
            public Formatter(IList<IClaimValueFormatter> claimFormats)
            {
                ClaimFormats = claimFormats;
            }

            public IList<IClaimValueFormatter> ClaimFormats { get; }

            public IClaimValueFormatter GetFormat(string name)
            {
                for (int i = 0; i < ClaimFormats.Count; i++)
                {
                    if (ClaimFormats[i].SupportsFormat(name))
                    {
                        return ClaimFormats[i];
                    }
                }

                return null;
            }
        }
    }
}
