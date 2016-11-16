// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Localization
{
    public class POWithCultureStringLocalizer : POStringLocalizer
    {
        private CultureInfo _culture;

        public POWithCultureStringLocalizer(
            string baseName,
            string location,
            IOptions<LocalizationOptions> localizationOptions,
            CultureInfo culture)
                : base(baseName, location, localizationOptions)
        {
            _culture = culture;
        }

        public POWithCultureStringLocalizer(
            Type resourceSource,
            IOptions<LocalizationOptions> localizationOptions,
            CultureInfo culture)
                : base(resourceSource, localizationOptions)
        {
            _culture = culture;
        }

        public override IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
            GetAllStrings(includeParentCultures, _culture);

        protected override string GetStringSafely(string name, CultureInfo culture)
        {
            return base.GetStringSafely(name, _culture);
        }
    }
}
