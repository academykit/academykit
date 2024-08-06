namespace AcademyKit.Infrastructure.Helpers
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Threading.Tasks;

    public class PasswordGenerator
    {
        private static readonly char[] UpperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        private static readonly char[] LowerChars = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        private static readonly char[] Digits = "0123456789".ToCharArray();
        private static readonly char[] SpecialChars =
            "!@#$%^&*()-_=+[]{}|;:'\",.<>?/`~".ToCharArray();
        private static readonly char[] AllChars = UpperChars
            .Concat(LowerChars)
            .Concat(Digits)
            .Concat(SpecialChars)
            .ToArray();

        public static async Task<string> GenerateStrongPassword(int length)
        {
            if (length < 8)
            {
                throw new ArgumentException("Password length must be at least 8 characters.");
            }

            var password = new char[length];

            // Ensure at least one character from each category is included
            password[0] = UpperChars[GetRandomInt(UpperChars.Length)];
            password[1] = LowerChars[GetRandomInt(LowerChars.Length)];
            password[2] = Digits[GetRandomInt(Digits.Length)];
            password[3] = SpecialChars[GetRandomInt(SpecialChars.Length)];

            // Fill the remaining characters with random selection from all characters
            for (var i = 4; i < length; i++)
            {
                password[i] = AllChars[GetRandomInt(AllChars.Length)];
            }

            // Shuffle the array to ensure randomness
            password = password.OrderBy(x => GetRandomInt(password.Length)).ToArray();

            return await Task.FromResult(new string(password));
        }

        private static int GetRandomInt(int max)
        {
            using var rng = RandomNumberGenerator.Create();
            var uint32Buffer = new byte[4];
            rng.GetBytes(uint32Buffer);
            var randomNumber = BitConverter.ToUInt32(uint32Buffer, 0);
            return (int)(randomNumber % max);
        }
    }
}
