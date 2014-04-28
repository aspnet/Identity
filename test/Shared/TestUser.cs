using System;

namespace Microsoft.AspNet.Identity.Test
{
    public class TestUser
    {
        public TestUser()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; private set; }
        public string UserName { get; set; }
    }
}