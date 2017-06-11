namespace Identity.ClientApplications.WebSite
{
    public class RemoveScopeViewModel
    {
        public RemoveScopeViewModel(string name, string scope)
        {
            Name = name;
            Scope = scope;
        }

        public string Name { get; }
        public string Scope { get; }
    }
}
