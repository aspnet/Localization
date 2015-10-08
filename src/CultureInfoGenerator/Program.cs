// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Dnx.Runtime;
using Microsoft.Win32;

namespace CultureInfoGenerator
{
    public class Program
    {
        private readonly string _appName;
        private readonly string _appPath;

        public Program(IApplicationEnvironment appEnvironment)
        {
            _appName = appEnvironment.ApplicationName;
            _appPath = appEnvironment.ApplicationBasePath;
        }

        public void Main(string[] args)
        {
            var outputFilePath = Path.GetFullPath(args.Length > 0 ? args[0] : Path.Combine(_appPath, "../Microsoft.Extensions.Globalization.CultureInfoCache/CultureInfoList.cs"));
            var netFxVersion = Get45or451FromRegistry();
            var windowsVersion = Environment.OSVersion;

            using (var writer = new StreamWriter(outputFilePath, false))
            {
                writer.WriteLine($@"// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// *************************** THIS FILE IS GENERATED BY A TOOL ***************************
// To make changes to this file look at the CultureInfoGenerator project in this solution.

using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.Globalization
{{
    /// <summary>
    /// Contains a list of known culture names that can be used to create a <see cref=""System.Globalization.CultureInfo""/>.
    /// </summary>
    public static partial class CultureInfoCache
    {{
        /// <summary>
        /// This list of known cultures was generated by {_appName} using .NET Framework {netFxVersion} on
        /// {windowsVersion}.
        /// As new versions of .NET Framework and Windows are released, this list should be regenerated to ensure it
        /// contains the latest culture names.
        /// </summary>
        public static readonly HashSet<string> KnownCultureNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {{"
            );

                var cultures = CultureInfo.GetCultures(
                    CultureTypes.NeutralCultures
                    | CultureTypes.InstalledWin32Cultures
                    | CultureTypes.SpecificCultures);

                writer.WriteLine(string.Join($",{Environment.NewLine}", cultures.Select(c => $"            \"{c.Name}\"")));
                writer.WriteLine(
@"        };
    }
}");

                Console.WriteLine($"{cultures.Length} culture names written to {outputFilePath}");
            }
        }

        // .NET Framework detection code copied from https://msdn.microsoft.com/en-us/library/hh925568%28v=vs.110%29.aspx#net_d
        private static string Get45or451FromRegistry()
        {
            using (var ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)
                .OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
            {
                var releaseKey = Convert.ToInt32(ndpKey.GetValue("Release"));
                return CheckFor45DotVersion(releaseKey);
            }
        }

        // Checking the version using >= will enable forward compatibility,  
        // however you should always compile your code on newer versions of 
        // the framework to ensure your app works the same. 
        private static string CheckFor45DotVersion(int releaseKey)
        {
            if (releaseKey >= 393273)
            {
                return "4.6 RC or later";
            }
            if (releaseKey >= 379893)
            {
                return "4.5.2 or later";
            }
            if (releaseKey >= 378675)
            {
                return "4.5.1 or later";
            }
            if (releaseKey >= 378389)
            {
                return "4.5 or later";
            }
            // This line should never execute. A non-null release key should mean 
            // that 4.5 or later is installed. 
            return "No 4.5 or later version detected";
        }
    }
}
