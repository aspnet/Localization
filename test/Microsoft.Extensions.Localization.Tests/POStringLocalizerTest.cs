// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Testing;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Microsoft.Extensions.Localization
{
    public class POStringLocalizerTest
    {
        [Fact]
        public void GetString_WithParameters()
        {
            // Arrange
            var localization = CreatePOLocalizer("BaseFile");

            // Act
            var result = localization["replace with stuff", "THING"];

            // Assert
            Assert.Equal("replace THING with stuff", result.Value);
        }

        [Fact]
        [ReplaceCulture("en-US", "en-US")]
        public void GetString_Base()
        {
            // Arrange
            var localizer = CreatePOLocalizer("BaseFile");

            // Act
            var result = localizer["this is a multiline"];

            // Assert
            Assert.Equal("Multi line str", result.Value);
        }

        [Fact]
        [ReplaceCulture("en-US", "en-US")]
        public void GetString_WalksCultureTree()
        {
            // Arrange
            var localizer = CreatePOLocalizer("CultureFile");

            // Act
            var result = localizer["culture"];

            // Assert
            Assert.Equal("culture en", result.Value);
        }

        [Fact]
        [ReplaceCulture("en-US", "en-US")]
        public void GetString_CrossProject_Embed()
        {
            // Arrange
            var assembly = typeof(LocalizationWebsite.Program).GetTypeInfo().Assembly;
            var localizer = CreatePOLocalizer("BaseFileProj", "POFiles", assembly);

            // Act
            var result = localizer["base id proj"];

            // Assert
            Assert.Equal("base str proj", result);
        }

        [Fact]
        [ReplaceCulture("en-US", "en-US")]
        public void GetString_FindsFiles()
        {
            // Arrange
            var localizer = CreatePOLocalizer("NonEmbed", "NonEmbed");

            // Act
            var result = localizer["nonembed"];

            // Assert
            Assert.Equal("nonembed base", result.Value);
        }

        [Fact]
        [ReplaceCulture("en-US", "en-US")]
        public void GetString_MissingValueGivesKey()
        {
            // Arrange
            var localizer = CreatePOLocalizer("BaseFile");

            // Act
            var result = localizer["this key isn't in the file"];

            // Assert
            Assert.Equal("this key isn't in the file", result);
        }

        [Fact]
        [ReplaceCulture("en-US", "en-US")]
        public void GetAllStrings_IncludeParentCultures()
        {
            // Arrange
            var localizer = CreatePOLocalizer("CultureFile");

            // Act
            var result = localizer.GetAllStrings(includeParentCultures: true);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        [ReplaceCulture("en-US", "en-US")]
        public void GetAllString_ExcludeParentCultures()
        {
            // Arrange
            var localizer = CreatePOLocalizer("CultureFile");

            // Act
            var result = localizer.GetAllStrings(includeParentCultures: false);

            // Assert
            Assert.Equal(1, result.Count());
        }

        [Fact]
        [ReplaceCulture("fr-FR", "fr-FR")]
        public void WithCulture()
        {
            // Arrange
            IStringLocalizer localizer = CreatePOLocalizer("CultureFile");

            // Act
            localizer = localizer.WithCulture(new CultureInfo("en-US"));
            var result = localizer.GetAllStrings(includeParentCultures: true);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains("en-US", result.First());
        }

        private POStringLocalizer CreatePOLocalizer(string file)
        {
            return CreatePOLocalizer(file, "POFiles");
        }

        private POStringLocalizer CreatePOLocalizer(string file, string resourcePath)
        {
            return CreatePOLocalizer(file, resourcePath, typeof(POStringLocalizerFactoryTest).GetTypeInfo().Assembly);
        }

        private POStringLocalizer CreatePOLocalizer(string file, string resourcePath, Assembly assembly)
        {
            var options = new LocalizationOptions();
            options.ResourcesPath = resourcePath;
            var localizationOptions = new Mock<IOptions<LocalizationOptions>>();
            localizationOptions.Setup(o => o.Value).Returns(options);

            return new POStringLocalizer(
                file,
                assembly.GetName().Name,
                localizationOptions.Object);
        }
    }
}
