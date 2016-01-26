// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Hosting;

namespace LocalizationWebsite
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseDefaultConfiguration(args)
                .UseStartup("LocalizationWebsite")
                .Build();

            host.Run();
        }
    }
}