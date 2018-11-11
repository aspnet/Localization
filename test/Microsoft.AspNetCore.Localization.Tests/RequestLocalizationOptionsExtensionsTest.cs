// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Xunit;

namespace Microsoft.AspNetCore.Localization
{
    public class RequestLocalizationOptionsExtensionsTest
    {
        [Fact]
        public void AddRequestCultureProvider_ShouldBeInsertedAtFirstPostion()
        {
            // Arrange
            var options = new RequestLocalizationOptions();

            // Act
            options.AddRequestCultureProvider(new CustomRequestCultureProvider(context => {
                return Task.FromResult(new ProviderCultureResult("ar-YE"));
            }));

            // Assert
            Assert.IsType<CustomRequestCultureProvider>(options.RequestCultureProviders[0]);
        }
    }
}
