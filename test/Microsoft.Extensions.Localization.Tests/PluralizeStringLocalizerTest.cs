// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xunit;

namespace Microsoft.Extensions.Localization.Tests
{
    public class PluralizeStringLocalizerTest
    {
        [Theory]
        [InlineData("Comment", 0, "Comments")]
        [InlineData("Comment", 1, "Comment")]
        [InlineData("Comment", 2, "Comments")]
        public void PluralizeStringLocalizer_Pluralize(string name, int count, string expectedValue)
        {
            // Arrange
            var localizer = new InMemoryPluralizeStringLocalizer();
            LocalizedString resource;

            // Act
            resource = localizer.Pluralize(name, count);

            // Assert
            Assert.Equal(expectedValue, resource.Value);
        }

        [Theory]
        [InlineData("{0} Comment", 0, "0 Comments")]
        [InlineData("{0} Comment", 1, "1 Comment")]
        [InlineData("{0} Comment", 2, "2 Comments")]
        public void PluralizeStringLocalizer_PluralizeWithArgument(string name, int count, string expectedValue)
        {
            // Arrange
            var localizer = new InMemoryPluralizeStringLocalizer();
            LocalizedString resource;

            // Act
            resource = localizer.Pluralize(name, count);

            // Assert
            Assert.Equal(expectedValue, resource.Value);
        }

        [Theory]
        [InlineData("Person", 1, "Person")]
        [InlineData("{0} Person", 1, "1 Person")]
        [InlineData("{0} Person", 2, "2 Person")]
        public void PluralizeStringLocalizer_Pluralize_MissingResourceReturnsDefaultName(string name, int count, string expectedValue)
        {
            // Arrange
            var localizer = new InMemoryPluralizeStringLocalizer();
            LocalizedString resource;

            // Act
            resource = localizer.Pluralize(name, count);

            // Assert
            Assert.True(resource.ResourceNotFound);
            Assert.Equal(expectedValue, resource.Value);
        }

        [Fact]
        public void PluralizeStringLocalizer_GetPluralRule_InvalidCultureThrows()
        {
            // Arrange
            var localizer = new InMemoryPluralizeStringLocalizer();

            // Act & Assert
            Assert.Throws(typeof(NotSupportedException), () => 
                localizer.GetPluralRule("??"));
        }

        public class InMemoryPluralizeStringLocalizer : IPluralizeStringLocalizer
        {
            private static IDictionary<string, IList<string>> _resources =
                new Dictionary<string, IList<string>>()
                {
                    { "Comment", new List<string>(new string[] { "Comment", "Comments" }) },
                    { "{0} Comment", new List<string>(new string[] { "{0} Comment", "{0} Comments" }) }
                };

            public LocalizedString this[string name]
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public LocalizedString this[string name, params object[] arguments]
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
            {
                throw new NotImplementedException();
            }

            public PluralizationRule GetPluralRule(string twoLetterISOLanguageName)
            {
                if (twoLetterISOLanguageName == "en")
                {
                    return n => (n == 1) ? 0 : 1;
                }
                else
                {
                    throw new NotSupportedException();
                }
            }

            public LocalizedString Pluralize(string name, int count)
            {
                var culture = CultureInfo.CurrentUICulture;
                var pluralRule = GetPluralRule(culture.TwoLetterISOLanguageName);
                var plurality = pluralRule(count);
                var format = _resources.ContainsKey(name) ? 
                    _resources[name][plurality] : null;
                var value = string.Format(format ?? name, count);

                return new LocalizedString(name, value, resourceNotFound: format == null);
            }

            public IStringLocalizer WithCulture(CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}
