namespace Microsoft.Extensions.Identity.Claims
{
    /// <summary>
    /// Default implementation for <see cref="IClaimNameFormatter"/>.
    /// </summary>
    public class DefaultClaimNameFormatter : IClaimNameFormatter
    {
        /// <inheritdoc />
        public string Format(string propertyName) => propertyName;
    }
}
