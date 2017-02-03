// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.Localization.Internal
{

    public class POEntry
    {
        private string _translation;

        private string _original;

        public StringBuilder OriginalBuilder { get; private set; }

        public string Original
        {
            get
            {
                if (_original == null)
                {
                    _original = OriginalBuilder?.ToString();
                }

                return _original;
            }
        }

        public string OriginalPlural { get; set; }

        public StringBuilder TranslationBuilder { get; private set; }

        public string Translation
        {
            get
            {
                if (_translation == null)
                {
                    _translation = TranslationBuilder?.ToString();
                }

                return _translation;
            }
        }

        public IDictionary<int, string> TranslationPlurals { get; set; } = new Dictionary<int, string>();

        public string Untranslated { get; set; }

        public List<string> References { get; set; } = new List<string>();

        public List<string> Contexts { get; set; } = new List<string>();

        public List<string> Flags { get; set; } = new List<string>();

        public string Comment { get; set; }

        public static StringBuilder ConcatStringBuilders(StringBuilder first, StringBuilder second)
        {
            for (int i = 0; i < second.Length; i++)
            {
                first.Append(second[i]);
            }

            return first;
        }

        public void AppendTranslationLine(StringBuilder line)
        {
            if (TranslationBuilder == null)
            {
                TranslationBuilder = new StringBuilder();
            }

            _translation = null;
            ConcatStringBuilders(TranslationBuilder, line);
        }

        public void AppendOriginalLine(StringBuilder line)
        {
            if (OriginalBuilder == null)
            {
                OriginalBuilder = new StringBuilder();
            }

            ConcatStringBuilders(OriginalBuilder, line);
        }
    }
}
