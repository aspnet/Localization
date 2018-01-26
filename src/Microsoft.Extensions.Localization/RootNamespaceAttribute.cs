// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Extensions.Localization
{
    /// <summary>
    /// Provides the RootNamespace of an Assembly. The RootNamespace of the assembly is used by Localization to
    /// determine the resource name to look for when RootNamespace differs from the AssemblyName.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public class RootNamespaceAttribute : Attribute
    {
        /// <summary>
        /// Creates a new <see cref="RootNamespaceAttribute"/>.
        /// </summary>
        /// <param name="rootNamespace">The RootNamespace for this Assembly.</param>
        public RootNamespaceAttribute(string rootNamespace)
        {
            if (string.IsNullOrEmpty(rootNamespace))
            {
                throw new ArgumentNullException(nameof(rootNamespace));
            }

            if (!IsValidNamespace(rootNamespace))
            {
                throw new ArgumentException(Resources.Exception_InvalidRootNamespace, nameof(rootNamespace));
            }

            RootNamespace = rootNamespace;
        }

        /// <summary>
        /// The RootNamespace of this Assembly. The RootNamespace of the assembly is used by Localization to
        /// determine the resource name to look for when RootNamespace differs from the AssemblyName.
        /// </summary>
        public string RootNamespace { get; }

        private static bool IsValidNamespace(string @namespace)
        {
            if (string.IsNullOrEmpty(@namespace))
            {
                return false;
            }

            if (!(char.IsLetter(@namespace[0]) || @namespace[0] == '_'))
            {
                return false;
            }

            for (int i = 1; i < @namespace.Length; i++)
            {
                if (!(char.IsLetterOrDigit(@namespace[i]) || @namespace[i] == '.'))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
