namespace Lingtren.Infrastructure.Helpers
{
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// handle to convert string to slug
    /// </summary>
    public static class SlugExtensions
    {

        /// <summary>
        ///  Handles to removes all accents from the string
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string RemoveAccents(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            text = text.Normalize(NormalizationForm.FormD);
            char[] chars = text
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) !=
                   UnicodeCategory.NonSpacingMark).ToArray();

            return new string(chars).Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Turn a string into a slug by removing all accents,
        /// special characters, additional spaces, substituting
        /// spaces with hyphens & making it lower-case.
        /// </summary>
        /// <param name="phrase">The string to turn into a slug.</param>
        /// <returns></returns>
        public static string Slugify(this string phrase)
        {
            // Remove all accents and make the string lower case.  
            string output = phrase.RemoveAccents().ToLower();

            // Remove all special characters from the string.  
            output = Regex.Replace(output, @"[^A-Za-z0-9\s-]", "");

            // Remove all additional spaces in favour of just one.  
            output = Regex.Replace(output, @"\s+", " ").Trim();

            // Replace all spaces with the hyphen.  
            output = Regex.Replace(output, @"\s", "-");

            // Return the slug.  
            return output;
        }
    }
}

