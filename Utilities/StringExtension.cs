using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyAdmin.Utilities
{
    public static class StringExtension
    {
        /// <summary>
        /// Trims the String and returns Null if it's empty space
        /// </summary>
        public static string TrimFix(this string rawString)
        {
            if (!string.IsNullOrWhiteSpace(rawString))
            {
                return rawString.Trim();
            }
            return null;
        }

        /// <summary>
        /// Sets first Character to Lowercase
        /// </summary>
        public static string ToCamelCase(this string str)
        {
            return string.IsNullOrEmpty(str) || str.Length < 2 ?
                str : char.ToLowerInvariant(str[0]) + str.Substring(1);
        }

        /// <summary>
        /// Pluralizes the Current string
        /// </summary>
        public static string ToPlural(this string str)
        {
            if (!str.EndsWith("s"))
                return str + "s";

            return str + "es";
        }
    }
}
