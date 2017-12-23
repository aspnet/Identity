using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.Identity.Claims
{
    /// <summary>
    /// A formatter for property names.
    /// </summary>
    public interface IClaimNameFormatter
    {
        /// <summary>
        /// Formats the name of a given property into a given claim.
        /// </summary>
        /// <param name="propertyName">The name of the property to format.</param>
        /// <returns>The formatted property name.</returns>
        string Format(string propertyName);
    }
}
