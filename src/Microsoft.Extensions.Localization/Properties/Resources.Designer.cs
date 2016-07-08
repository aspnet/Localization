// <auto-generated />
namespace Microsoft.Extensions.Localization
{
    using System.Globalization;
    using System.Reflection;
    using System.Resources;

    internal static class Resources
    {
        private static readonly ResourceManager _resourceManager
            = new ResourceManager("Microsoft.Extensions.Localization.Resources", typeof(Resources).GetTypeInfo().Assembly);

        /// <summary>
        /// he name of the string resource is missing from the resource source.
        /// </summary>
        internal static string LocalizedStringNotFound
        {
            get { return GetString("LocalizedStringNotFound"); }
        }

        /// <summary>
        /// he name of the string resource is missing from the resource source.
        /// </summary>
        internal static string FormatLocalizedStringNotFound()
        {
            return GetString("LocalizedStringNotFound");
        }
			
        /// <summary>
        /// The manifest '{0}' was not found.
        /// </summary>
        internal static string Localization_MissingManifest
        {
            get { return GetString("Localization_MissingManifest"); }
        }

        /// <summary>
        /// The manifest '{0}' was not found.
        /// </summary>
        internal static string FormatLocalization_MissingManifest(object p0)
        {
            return string.Format(CultureInfo.CurrentCulture, GetString("Localization_MissingManifest"), p0);
        }

        /// <summary>
        /// No manifests exist for the current culture.
        /// </summary>
        internal static string Localization_MissingManifest_Parent
        {
            get { return GetString("Localization_MissingManifest_Parent"); }
        }

        /// <summary>
        /// No manifests exist for the current culture.
        /// </summary>
        internal static string FormatLocalization_MissingManifest_Parent()
        {
            return GetString("Localization_MissingManifest_Parent");
        }

        private static string GetString(string name, params string[] formatterNames)
        {
            var value = _resourceManager.GetString(name);

            System.Diagnostics.Debug.Assert(value != null);

            if (formatterNames != null)
            {
                for (var i = 0; i < formatterNames.Length; i++)
                {
                    value = value.Replace("{" + formatterNames[i] + "}", "{" + i + "}");
                }
            }

            return value;
        }
    }
}
