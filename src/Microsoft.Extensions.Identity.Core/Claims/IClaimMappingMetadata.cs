namespace Microsoft.Extensions.Identity
{
    /// <summary>
    /// Metadata describing the mapping of an user property into one or more
    /// claims.
    /// </summary>
    public interface IClaimMappingMetadata
    {
        /// <summary>
        /// Gets the name of the claim or the prefix to use for the claim names
        /// generated from mapping a user property to one or more claims.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the format to use to produce the value of the mapped claim or claims.
        /// </summary>
        string Format { get; }
    }
}