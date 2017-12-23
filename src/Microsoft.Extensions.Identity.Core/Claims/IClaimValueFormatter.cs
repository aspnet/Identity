namespace Microsoft.Extensions.Identity.Claims
{
    /// <summary>
    /// A type capable of formatting a user property value into
    /// a <see cref="string"/> that can be used as the value for
    /// a Claim.
    /// </summary>
    public interface IClaimFormatter
    {
        /// <summary>
        /// Whether or not a given format is supported by this <see cref="IClaimFormatter"/>.
        /// </summary>
        /// <param name="formatName">The name of the format.</param>
        /// <returns>true if the format is supported.</returns>
        bool SupportsFormat(string formatName);
     
        /// <summary>
        /// Formats <paramref name="value"/> into a <see cref="string"/>.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <returns>The formatted value.</returns>
        string Format(object value);
    }
}