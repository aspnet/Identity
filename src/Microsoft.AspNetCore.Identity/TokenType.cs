using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Identity
{
    /// <summary>
    /// Specifies the format used for generating tokens.
    /// </summary>
    public enum TokenType
    {
        /// <summary>
        /// Indicates generating token for email link.
        /// </summary>
        Email = 0,
        /// <summary>
        /// Indicates generating token for phone.
        /// </summary>
        Phone = 1
    }
}
