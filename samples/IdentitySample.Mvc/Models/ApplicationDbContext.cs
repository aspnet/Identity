using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB.Identity;

namespace IdentitySample.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
    }
}
