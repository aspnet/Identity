namespace Identity.ClientApplications.WebSite
{
    public class ChangeApplicationNameViewModel
    {
        public ChangeApplicationNameViewModel()
        {
        }

        public ChangeApplicationNameViewModel(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
