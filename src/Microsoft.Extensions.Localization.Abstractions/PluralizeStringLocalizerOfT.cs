// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Extensions.Localization
{
    /// <summary>
    /// Provides pluralized strings for <typeparamref name="TResourceSource"/>.
    /// </summary>
    /// <typeparam name="TResourceSource">The <see cref="Type"/> to provide pluralized strings for.</typeparam>
    public class PluralizeStringLocalizer<TResourceSource> : IPluralizeStringLocalizer<TResourceSource>
    {
        private IPluralizeStringLocalizer _localizer;

        /// <summary>
        /// Creates a new <see cref="PluralizeStringLocalizer{TResourceSource}"/>.
        /// </summary>
        /// <param name="factory">The <see cref="IStringLocalizerFactory"/> to use.</param>
        public PluralizeStringLocalizer(IStringLocalizerFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            _localizer = (IPluralizeStringLocalizer) factory.Create(typeof(TResourceSource));
        }

        /// <inheritdoc />
        public virtual IStringLocalizer WithCulture(CultureInfo culture) => _localizer.WithCulture(culture);

        /// <inheritdoc />
        public virtual LocalizedString this[string name]
        {
            get
            {
                if (name == null)
                {
                    throw new ArgumentNullException(nameof(name));
                }

                return _localizer[name];
            }
        }

        /// <inheritdoc />
        public virtual LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                if (name == null)
                {
                    throw new ArgumentNullException(nameof(name));
                }

                return _localizer[name, arguments];
            }
        }

        /// <inheritdoc />
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
            _localizer.GetAllStrings(includeParentCultures);

        /// <inheritdoc />
        public LocalizedString Pluralize(string name, int count) =>
            _localizer.Pluralize(name, count);

        /// <inheritdoc />
        public PluralizationRule GetPluralRule(string twoLetterISOLanguageName) => 
            _localizer.GetPluralRule(twoLetterISOLanguageName);
    }
}