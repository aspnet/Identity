
using System;

namespace Microsoft.AspNet.Identity.Entity
{
    public class IdentityRole : IdentityRole<string>
    {
        public IdentityRole()
        {
            Id = Guid.NewGuid().ToString();
        }

        public IdentityRole(string name) : this()
        {
            Name = name;
        }
    }

    public class IdentityRole<TKey> : IRole<TKey> where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }
        public string Name { get; set; }
    }
}
