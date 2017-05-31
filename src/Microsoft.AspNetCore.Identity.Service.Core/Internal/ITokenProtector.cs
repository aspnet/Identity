// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNetCore.Identity.Service.Internal
{
    public interface ITokenProtector<TData> where TData : Token
    {
        string Protect(TData data);
        string Protect(TData data, string purpose);
        TData Unprotect(string protectedText);
        TData Unprotect(string protectedText, string purpose);
    }
}
