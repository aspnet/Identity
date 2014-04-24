namespace Microsoft.AspNet.Identity
{
    // TODO: find a non identity home for this (DI?)
    public interface IOptionsSetup<in TOptions>
    {
        int ExecutionOrder { get; }
        void Setup(TOptions options);
    }

    // TODO: find a non identity home for this (DI?)
    public interface IOptionsAccessor<out TOptions>
    {
        TOptions Options { get; }
    }

    /// <summary>
    ///     Default configuration for identity
    /// </summary>
    public class DefaultIdentitySetup : IOptionsSetup<IdentityOptions>
    {
        public int ExecutionOrder { get { return -1; } }

        public void Setup(IdentityOptions options)
        {
        }
    }
}