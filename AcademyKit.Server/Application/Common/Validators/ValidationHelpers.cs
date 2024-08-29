using System.Text.RegularExpressions;

namespace AcademyKit.Server.Application.Common.Validators;

/// <summary>
/// Provides common validation methods.
/// </summary>
public static class ValidationHelpers
{
    /// <summary>
    /// Validates if the provided email is in a correct format.
    /// </summary>
    /// <param name="email">The email address to validate.</param>
    /// <returns>True if the email is valid, otherwise false.</returns>
    public static bool ValidEmail(string email)
    {
        const string emailRegex =
            @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}"
            + @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\"
            + @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

        return new Regex(emailRegex).IsMatch(email);
    }

    /// <summary>
    /// Validates if the provided mobile number is in correct format.
    /// </summary>
    /// <param name="mobileNumber">the mobile number to validate</param>
    /// <returns>True if the mobile number is valid, otherwise false</returns>
    public static bool ValidMobileNumber(string mobileNumber)
    {
        const string mobilePattern = @"^[+\d]+$";
        return new Regex(mobilePattern).IsMatch(mobileNumber);
    }

    /// <summary>
    /// Validates if the provided password meets the required criteria.
    /// </summary>
    /// <param name="pw">The password string to validate.</param>
    /// <returns>
    /// <c>true</c> if the password contains at least one lowercase letter, one uppercase letter, one digit, and one special character; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasValidPassword(string pw)
    {
        var lowercase = new Regex("[a-z]+");
        var uppercase = new Regex("[A-Z]+");
        var digit = new Regex("(\\d)+");
        var symbol = new Regex("(\\W)+");
        return lowercase.IsMatch(pw)
            && uppercase.IsMatch(pw)
            && digit.IsMatch(pw)
            && symbol.IsMatch(pw);
    }
}
