using System.ComponentModel.DataAnnotations;

namespace Identity.ClientApplications.WebSite.Identity.Models.ApplicationViewModels
{
    public class CreateApplicationViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}
