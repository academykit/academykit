using System.Text.RegularExpressions;

namespace AcademyKit.Application.Common.Helpers
{
    /// <summary>
    /// This class is the helper class for this project.
    /// </summary>
    ///
    /// <threadsafety>
    /// This class is immutable and thread safe.
    /// </threadsafety>
    public static class CommonHelper
    {
        /// <summary>
        /// Removes the HTML tags from the text
        /// </summary>
        /// <param name="text">The text which needs to be cleaned</param>
        /// <returns></returns>
        public static string RemoveHtmlTags(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            var textOnly = Regex.Replace(text, "<.*?>", "");

            return textOnly;
        }
    }
}
