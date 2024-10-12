using System.Security.Cryptography;

namespace BAL.CompanyServices.Helpers
{
    public static class PasswordHelper
    {
        public static string GenerateRandomPassword(int length)
        {
            if (length < 4) throw new ArgumentException("Password length must be at least 4 characters.");

            const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
            const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "1234567890";
            const string specialChars = "!@#$%^&*()";

            // Ensure the password contains at least one character from each category
            var password = new char[length];
            password[0] = GetRandomCharacter(lowerChars);
            password[1] = GetRandomCharacter(upperChars);
            password[2] = GetRandomCharacter(digits);
            password[3] = GetRandomCharacter(specialChars);

            // Fill the rest of the password length with random characters
            const string validChars = lowerChars + upperChars + digits + specialChars;
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] byteBuffer = new byte[sizeof(uint)];
                for (int i = 4; i < length; i++)
                {
                    rng.GetBytes(byteBuffer);
                    uint num = BitConverter.ToUInt32(byteBuffer, 0);
                    password[i] = validChars[(int)(num % (uint)validChars.Length)];
                }
            }

            // Shuffle the result to avoid having predictable positions
            return Shuffle(password);
        }

        private static char GetRandomCharacter(string chars)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] byteBuffer = new byte[sizeof(uint)];
                rng.GetBytes(byteBuffer);
                uint num = BitConverter.ToUInt32(byteBuffer, 0);
                return chars[(int)(num % (uint)chars.Length)];
            }
        }

        private static string Shuffle(char[] array)
        {
            Random random = new Random();
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                // Swap
                char temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
            return new string(array);
        }
    }
}
