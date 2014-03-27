
using System;

namespace Microsoft.AspNet.Identity.Entity
{
    public class IdentityRole : IdentityRole<string>
    {
        public IdentityRole(string name) : base(name) { }
    }

    public class IdentityRole<TKey> : IRole<TKey> where TKey : IEquatable<TKey>
    {
        public IdentityRole(string name)
        {
            Name = name;
        }

        public TKey Id { get; set; }
        public string Name { get; set; }
    }
}
