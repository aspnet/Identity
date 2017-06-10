using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.ClientApplications.WebSite.Identity.Models.ApplicationViewModels
{
    public class RemoveApplicationViewModel
    {
        public RemoveApplicationViewModel(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
