// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Xunit;

namespace Microsoft.AspNet.Identity.Test
{
    public class IdentityResultTest
    {
        [Fact]
        public void VerifyDefaultConstructor()
        {
            var result = new IdentityResult();
            Assert.False(result.Succeeded);
            Assert.Equal(1, result.Failures.Count());
            Assert.Equal(IdentityFailure.Unknown, result.Failures.First());
        }

        [Fact]
        public void NullFailureListUsesUnknownFailure()
        {
            var result = new IdentityResult(null);
            Assert.False(result.Succeeded);
            Assert.Equal(1, result.Failures.Count());
            Assert.Equal(IdentityFailure.Unknown, result.Failures.First());
        }
    }
}