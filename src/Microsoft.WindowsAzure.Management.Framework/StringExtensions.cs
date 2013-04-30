namespace Microsoft.WindowsAzure.Management.Framework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Provides useful extension methods for strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Determines if a string is null or empty.
        /// </summary>
        /// <param name="value">
        /// The string to check.
        /// </param>
        /// <returns>
        /// True if the string is null or string.Empty otherwise false.
        /// </returns>
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Determines if a string is not null or empty.
        /// </summary>
        /// <param name="value">
        /// The string to check.
        /// </param>
        /// <returns>
        /// True if the string is something other than null or string.Empty otherwise false.
        /// </returns>
        public static bool IsNotNullOrEmpty(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }
    }
}
