// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Xunit;

namespace Microsoft.Extensions.Localization.Tests
{
    public class RootNamespaceAttributeTest
    {
        [Fact]
        public void InvalidRootNamespace_ThrowsArgumentException()
        {
            // Arrange
            var rootNamespace = "Invalid?RootNamespace";
            var expectedMessage = Resources.Exception_InvalidRootNamespace + Environment.NewLine + "Parameter name: rootNamespace";

            // Assert
            var exception = Assert.Throws<ArgumentException>(() => {
                // Act
                var attribute = new RootNamespaceAttribute(rootNamespace);
            });
            Assert.Equal(expectedMessage, exception.Message);
        }
    }
}
