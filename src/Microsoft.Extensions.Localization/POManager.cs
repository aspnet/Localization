using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Microsoft.Extensions.Localization
{
    public class POManager
    {
        private readonly string _baseName;
        private readonly Assembly _assembly;
        private readonly string _resourcesRelativePath;

        private static readonly POParser POParser = new POParser();

        public POManager(string baseName, string location, string resourcesRelativePath)
        {
            throw new NotImplementedException();

            //_baseName = baseName;
            //var assemblyName = new AssemblyName(location);
            //_assembly = Assembly.Load(assemblyName);
            //_resourcesRelativePath = resourcesRelativePath;
        }

        public POManager(Type resourceSource, string resourcesRelativePath)
        {
            _baseName = resourceSource.Name;
            _assembly = resourceSource.GetTypeInfo().Assembly;
            _resourcesRelativePath = resourcesRelativePath + ".";
        }

        public string GetString(string name)
        {
            return GetString(name, CultureInfo.CurrentUICulture);
        }

        public string GetString(string name, CultureInfo culture)
        {
            return GetPOResults(culture)[name].Translation;
        }

        private IDictionary<string, POEntry> GetPOResults(CultureInfo culture)
        {
            var text = GetPOText(culture);
            var translations = ParsePOFile(text);

            return translations;
        }

        private IDictionary<string, POEntry> ParsePOFile(string poText)
        {
            var translations = new Dictionary<string, POEntry>(StringComparer.OrdinalIgnoreCase);

            POParser.ParseLocalizationStream(poText, translations, true);

            return translations;
        }

        private string GetPOText(CultureInfo culture)
        {
            using (var stream = _assembly.GetManifestResourceStream(GetResourceName(culture)))
            {
                if (stream == null)
                {
                    throw new NotImplementedException("Do something smart!");
                }
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private string GetResourceName(CultureInfo culture)
        {
            var baseNamespace = _assembly.GetName().Name;
            return $"{GetResourcePrefix(_baseName, baseNamespace, _resourcesRelativePath)}.{culture.Name}.po";
        }

        private string GetResourcePrefix(string resourceName, string baseNamespace, string resourcesRelativePath)
        {
            return string.IsNullOrEmpty(resourcesRelativePath)
                ? _baseName
                : baseNamespace + "." + resourcesRelativePath + TrimPrefix(resourceName, baseNamespace + ".");
        }

        private static string TrimPrefix(string name, string prefix)
        {
            if (name.StartsWith(prefix, StringComparison.Ordinal))
            {
                return name.Substring(prefix.Length);
            }

            return name;
        }
    }
}
