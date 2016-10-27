using System;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Localization
{
    public class POStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly IOptions<LocalizationOptions> _options;

        public POStringLocalizerFactory(IOptions<LocalizationOptions> options)
        {
            _options = options;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            return new POStringLocalizer(resourceSource, _options);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return new POStringLocalizer(baseName, location, _options);
        }
    }
}
