namespace Microsoft.AspNet.Identity
{
    // TODO: find a non identity home for this (DI?)
    public interface IOptionsSetup<in TOptions>
    {
        int Order { get; }
        void Setup(TOptions options);
    }

    // TODO: find a non identity home for this (DI?)
    public interface IOptionsAccessor<out TOptions> where TOptions : new()
    {
        TOptions Options { get; }
    }

    /// <summary>
    ///     Default configuration for identity
    /// </summary>
    public class DefaultIdentitySetup : IOptionsSetup<IdentityOptions>
    {
        public int Order { get { return -1; } }

        public void Setup(IdentityOptions options)
        {
        }
    }
}