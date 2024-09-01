using System.Security.Cryptography;
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

        /// <summary>
        /// Generates a random nonce using a secure random number generator.
        /// </summary>
        /// <returns>A base64-encoded string representing the nonce.</returns>
        public static string GenerateNonce()
        {
            var randomBytes = new byte[32];
            RandomNumberGenerator.Fill(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}
