// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Globalization;

namespace Microsoft.AspNet.Localization
{
    /// <summary>
    /// Details about the culture for an <see cref="Http.HttpRequest"/>.
    /// </summary>
    public class RequestCulture
    {
        /// <summary>
        /// Creates a new <see cref="RequestCulture"/> object has its <see cref="Culture"/> and <see cref="UICulture"/>
        /// properties set to the same <see cref="CultureInfo"/> value.
        /// </summary>
        /// <param name="culture">The <see cref="CultureInfo"/> for the request.</param>
		/// <param name="timeZone">The <see cref="TimeZoneInfo"/> for the request.</param>
        public RequestCulture(CultureInfo culture, TimeZoneInfo timeZone)
            : this(culture, culture, timeZone)
        {
        }

        /// <summary>
        /// Creates a new <see cref="RequestCulture"/> object has its <see cref="Culture"/> and <see cref="UICulture"/>
        /// properties set to the same <see cref="CultureInfo"/> value.
        /// </summary>
        /// <param name="culture">The culture for the request.</param>
		/// <param name="timeZoneId">The time zone for the request.</param>
        public RequestCulture(string culture, string timeZoneId)
            : this(culture, culture, timeZoneId)
        {
        }

        /// <summary>
        /// Creates a new <see cref="RequestCulture"/> object has its <see cref="Culture"/> and <see cref="UICulture"/>
        /// properties set to the respective <see cref="CultureInfo"/> values provided.
        /// </summary>
        /// <param name="culture">The culture for the request to be used for formatting.</param>
        /// <param name="uiCulture">The culture for the request to be used for text, i.e. language.</param>
		/// <param name="timeZoneId">The time zone for the request to be used for dates.</param>
        public RequestCulture(string culture, string uiCulture, string timeZoneId)
            : this (new CultureInfo(culture), new CultureInfo(uiCulture), TimeZoneInfo.FindSystemTimeZoneById(timeZoneId))
        {
        }

        /// <summary>
        /// Creates a new <see cref="RequestCulture"/> object has its <see cref="Culture"/> and <see cref="UICulture"/> and <see cref="TimeZoneInfo"/>
        /// properties set to the respective <see cref="CultureInfo"/> and <see cref="TimeZoneInfo"/> values provided.
        /// </summary>
        /// <param name="culture">The <see cref="CultureInfo"/> for the request to be used for formatting.</param>
        /// <param name="uiCulture">The <see cref="CultureInfo"/> for the request to be used for text, i.e. language.</param>
		/// <param name="timeZone">The <see cref="TimeZoneInfo"/> for the request to be used for dates.</param>
        public RequestCulture(CultureInfo culture, CultureInfo uiCulture, TimeZoneInfo timeZone)
        {
            if (culture == null)
            {
                throw new ArgumentNullException(nameof(culture));
            }

            if (uiCulture == null)
            {
                throw new ArgumentNullException(nameof(uiCulture));
            }

			if (timeZone == null) {
				throw new ArgumentNullException(nameof(timeZone));
			}

			Culture = culture;
            UICulture = uiCulture;
			TimeZone = timeZone;
        }

        /// <summary>
        /// Gets the <see cref="CultureInfo"/> for the request to be used for formatting.
        /// </summary>
        public CultureInfo Culture { get; }

        /// <summary>
        /// Gets the <see cref="CultureInfo"/> for the request to be used for text, i.e. language;
        /// </summary>
        public CultureInfo UICulture { get; }

		public TimeZoneInfo TimeZone { get; }
    }
}
