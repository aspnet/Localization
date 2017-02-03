﻿// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Reflection;
using Microsoft.Extensions.Localization.Internal;
using Xunit;

namespace Microsoft.Extensions.Localization
{
    public class POManagerTest
    {
        [Fact]
        public void GetString_CacheReturnsSameObject()
        {
            var poManager = GetPOManager("BaseFile");

            var result1 = poManager.GetString("new string");
            var result2 = poManager.GetString("new string");

            Assert.Same(result1, result2);
        }

        [Fact]
        public void GetAllStrings_CacheReturnsSameObject()
        {
            var poManager = GetPOManager("BaseFile");

            var result1 = poManager.GetAllStrings(true);
            var result2 = poManager.GetAllStrings(true);

            Assert.Same(result1, result2);
        }

        private POManager GetPOManager(Type type)
        {
            return new POManager(type, "POFiles");
        }

        private POManager GetPOManager(string baseName)
        {
            return new POManager(baseName, typeof(POManagerTest).GetTypeInfo().Assembly.GetName().Name, "POFiles");
        }
    }
}
