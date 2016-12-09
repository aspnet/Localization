// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System.Reflection;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace OutsideNamespace
{
    public class OutsideClass
    {
    }
}

namespace Microsoft.Extensions.Localization.Tests
{
    public class POStringLocalizerFactoryTest
    {

    }
}

namespace Microsoft.Extensions.Localization
{
    public class POStringLocalizerFactoryTest
    {
        [Fact]
        public void Create_Type_OutsideNamespace()
        {
            // Arrange
            var options = new LocalizationOptions();
            options.ResourcesPath = "POFiles";
            var localizationOptions = new Mock<IOptions<LocalizationOptions>>();
            localizationOptions.Setup(o => o.Value).Returns(options);

            var factory = new POStringLocalizerFactory(localizationOptions.Object);

            // Act
            var localizer = factory.Create(typeof(OutsideNamespace.OutsideClass));

            // Assert
            Assert.Equal("msg str", localizer["msg id"]);
        }

        [Fact]
        public void Create_Type()
        {
            // Arrange
            var options = new LocalizationOptions();
            options.ResourcesPath = "POFiles";
            var localizationOptions = new Mock<IOptions<LocalizationOptions>>();
            localizationOptions.Setup(o => o.Value).Returns(options);

            var factory = new POStringLocalizerFactory(localizationOptions.Object);

            // Act
            var localizer = factory.Create(typeof(Tests.POStringLocalizerFactoryTest));

            // Assert
            Assert.Equal("value", localizer["key"]);
        }

        [Fact]
        public void Create_BaseName()
        {
            // Arrange
            var options = new LocalizationOptions();
            options.ResourcesPath = "POFiles";
            var localizationOptions = new Mock<IOptions<LocalizationOptions>>();
            localizationOptions.Setup(o => o.Value).Returns(options);

            var factory = new POStringLocalizerFactory(localizationOptions.Object);

            // Act
            var localizer = factory.Create(
                nameof(POStringLocalizerFactoryTest),
                typeof(POStringLocalizerFactoryTest).GetTypeInfo().Assembly.GetName().Name);

            // Assert
            Assert.Equal("value", localizer["key"]);
        }
    }
}
