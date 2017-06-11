namespace Identity.ClientApplications.WebSite
{
    public class RemoveLogoutUriViewModel
    {
        public RemoveLogoutUriViewModel(string name, string logoutUri)
        {
            Name = name;
            LogoutUri = logoutUri;
        }

        public string Name { get; }
        public string LogoutUri { get; }
    }
}
