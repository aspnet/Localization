// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Microsoft.Extensions.Localization.Internal;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Localization
{
    public class POStringLocalizer : IStringLocalizer
    {
        private readonly POManager _poManager;
        private readonly Type _resourceSource;
        private readonly IOptions<LocalizationOptions> _localizationOptions;
        private readonly string _baseName;
        private readonly string _location;

        public POStringLocalizer(Type resourceSource, IOptions<LocalizationOptions> localizationOptions)
        {
            _resourceSource = resourceSource;
            _localizationOptions = localizationOptions;

            _poManager = new POManager(resourceSource, GetResourcePath(resourceSource, localizationOptions.Value));
        }

        private static string GetResourcePath(Type resourceSource, LocalizationOptions options)
        {
            var assembly = resourceSource.GetTypeInfo().Assembly;
            var resourceLocationAttribute = assembly.GetCustomAttribute<ResourceLocationAttribute>();

            return resourceLocationAttribute?.ResourceLocation ?? options.ResourcesPath;
        }

        public POStringLocalizer(string baseName, string location, IOptions<LocalizationOptions> localizationOptions)
        {
            _baseName = baseName;
            _location = location;
            _localizationOptions = localizationOptions;
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

                // TODO: Add more supported format styles
                var value = string.Format(format ?? name, arguments);
                return new LocalizedString(name, value, resourceNotFound: format == null);
            }
        }

        public virtual IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return GetAllStrings(includeParentCultures, CultureInfo.CurrentUICulture);
        }

        protected IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures, CultureInfo culture)
        {
            var poEntries = _poManager.GetAllStrings(includeParentCultures, culture);

            foreach (var entries in poEntries)
            {
                yield return new LocalizedString(entries.Key, entries.Value.Translation);
            }
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            if (culture == null)
            {
                return this;
            }
            else
            {
                return _resourceSource == null ?
                    new POWithCultureStringLocalizer(_baseName, _location, _localizationOptions, culture)
                    : new POWithCultureStringLocalizer(_resourceSource, _localizationOptions, culture);
            }
        }

        protected virtual string GetStringSafely(string name, CultureInfo culture)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return culture == null ? _poManager.GetString(name) : _poManager.GetString(name, culture);
        }
    }
}
