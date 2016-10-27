using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Localization
{
    public class POStringLocalizer : IStringLocalizer
    {
        private readonly POManager _poManager;

        public POStringLocalizer(Type resourceSource, IOptions<LocalizationOptions> localizationOptions)
        {
            _poManager = new POManager(resourceSource, localizationOptions.Value.ResourcesPath);
        }

        public POStringLocalizer(string baseName, string location, IOptions<LocalizationOptions> localizationOptions)
        {
            _poManager = new POManager(baseName, location, localizationOptions.Value.ResourcesPath);
        }

        public LocalizedString this[string name]
        {
            get
            {
                if (name == null)
                {
                    throw new ArgumentNullException(nameof(name));
                }

                var value = GetStringSafely(name, null);
                return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                if (name == null)
                {
                    throw new ArgumentNullException(nameof(name));
                }

                var format = GetStringSafely(name, null);
                var value = string.Format(format ?? name, arguments);
                return new LocalizedString(name, value, resourceNotFound: format == null);
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            throw new NotImplementedException();
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        protected string GetStringSafely(string name, CultureInfo culture)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return culture == null ? _poManager.GetString(name) : _poManager.GetString(name, culture);
        }
    }
}
