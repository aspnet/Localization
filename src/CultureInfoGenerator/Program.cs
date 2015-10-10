// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Dnx.Runtime;
using Newtonsoft.Json;

namespace CultureInfoGenerator
{
    public class CultureResult
    {
        public string OS { get; set; }
        public string Framework { get; set; }
        public IEnumerable<string> Result { get; set; }
    }

    public class Program
    {
        private readonly string _appName;
        private readonly string _appPath;
        private string cultureListPath;
        private string resultPath;

        public Program(IApplicationEnvironment appEnvironment)
        {
            _appName = appEnvironment.ApplicationName;
            _appPath = appEnvironment.ApplicationBasePath;
        }

        public void Main(string[] args)
        {
            cultureListPath = Path.GetFullPath(args.Length > 0 ? args[0] : Path.Combine(_appPath, "../Microsoft.Extensions.Globalization.CultureInfoCache/CultureInfoList.cs"));
            resultPath = Path.GetFullPath(args.Length > 0 ? args[0] : Path.Combine(_appPath, "../Microsoft.Extensions.Globalization.CultureInfoCache/GeneratedResults/"));

            if (!Directory.Exists(resultPath))
                Directory.CreateDirectory(resultPath);

            var OSVersion = Environment.OSVersion;
            var DotNetVersion = Environment.Version;

            var cultures = CultureInfo.GetCultures(
                    CultureTypes.NeutralCultures
                    | CultureTypes.InstalledWin32Cultures
                    | CultureTypes.SpecificCultures);

            var json = JsonConvert.SerializeObject(new CultureResult
            {
                OS = OSVersion.ToString(),
                Framework = DotNetVersion.ToString(),
                Result = cultures.Select(x => x.Name)
            });
            File.WriteAllText(resultPath + OSVersion.ToString().Replace(" ", "_") + ".json", json);

            Combine();
        }

        public void Combine()
        {
            var results = Directory.GetFiles(resultPath);
            var collection = new HashSet<string>();
            var info = new List<KeyValuePair<string, string>>();
            var osString = "";
            foreach (var path in results)
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<CultureResult>(File.ReadAllText(path));
                    info.Add(new KeyValuePair<string, string> (result.OS, result.Framework));
                    foreach (var r in result.Result)
                    {
                        collection.Add(r);
                    }
                    osString += $"{Path.GetFileNameWithoutExtension(path)}, ";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"There were something wrong with the result {path}: {ex.Message}");
                }
            }

            osString = osString.TrimEnd(' ').TrimEnd(',');

            using (var writer = new StreamWriter(cultureListPath, false))
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
        /// This list of known cultures was generated by {_appName} on
");
                writer.WriteLine(string.Join($",{Environment.NewLine}", info.Select(c => $"        /// {c.Key} {c.Value}")));
                writer.WriteLine($@"
        /// As new versions of .NET Framework and Windows are released, this list should be regenerated to ensure it
        /// contains the latest culture names.
        /// </summary>
        public static readonly HashSet<string> KnownCultureNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {{"
            );
                writer.WriteLine(string.Join($",{Environment.NewLine}", collection.Select(c => $"            \"{c}\"")));
                writer.WriteLine(
@"        };
    }
}");

                Console.WriteLine($"{collection.Count} culture names written to {cultureListPath}");
            }
        }
    }
}
