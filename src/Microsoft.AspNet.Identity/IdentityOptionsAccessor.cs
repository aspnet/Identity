using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.Identity
{

    // TODO: Replace IdentityOptionsAccessor with this
    public class OptionsAccessor<TOptions> : IOptionsAccessor<TOptions> where TOptions : new()
    {
        private readonly IEnumerable<IOptionsSetup<TOptions>> _setup;
        private TOptions _options;
        private object _sync = new Object();

        public OptionsAccessor(IEnumerable<IOptionsSetup<TOptions>> setup)
        {
            _setup = setup;
        }

        public TOptions Options
        {
            get
            {
                lock (_sync)
                {
                    if (_options == null)
                    {
                        _options = _setup
                            .OrderBy(setup => setup.Order)
                            .Aggregate(
                                new TOptions(),
                                (options, setup) =>
                                {
                                    setup.Setup(options);
                                    return options;
                                });

                        // TODO: null out _setup enumerable w/out creating race condition?
                    }
                }
                return _options;
            }
        }
    }


    // Knows how to generate/access identity options
    public class IdentityOptionsAccessor : IOptionsAccessor<IdentityOptions>
    {
        // TODO: reconcile with lou's gist
        //public IdentityOptionsAccessor(IEnumerable<IOptionsSetup<IdentityOptions>> optionSetups)
        //{
        //    // Sort and execute to compute options
        //}
        public IdentityOptionsAccessor(IOptionsSetup<IdentityOptions> setup)
        {
            Options = new IdentityOptions();
            setup.Setup(Options);
        }


        public IdentityOptions Options { get; private set; }
    }
}