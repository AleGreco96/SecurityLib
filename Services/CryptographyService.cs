using System.Text;
using System.Security.Cryptography;

namespace SecurityLib.Services
{
    public class CryptographyService
    {
        public bool VerifyPassword(string password, string passwordHash, string passwordSalt)
        {
            try
            {
                byte[] byteSalt = Convert.FromBase64String(passwordSalt);

                byte[] passwordBytes = Encoding.Unicode.GetBytes(password);
                byte[] combined = new byte[passwordBytes.Length + byteSalt.Length];
                Buffer.BlockCopy(passwordBytes, 0, combined, 0, passwordBytes.Length);
                Buffer.BlockCopy(byteSalt, 0, combined, passwordBytes.Length, byteSalt.Length);

                using var sha256 = SHA256.Create();

                byte[] hashBytes = sha256.ComputeHash(combined);

                string computedHash = Convert.ToBase64String(hashBytes);

                return computedHash == passwordHash;

            } catch (Exception exception)
            {
                Console.WriteLine($"Error: {exception.Message}");
                return false;
            }   
        }

        public KeyValuePair<string, string> EncryptPassword(string password)
        {
            string passwordHash = string.Empty;
            string passwordSalt = string.Empty;

            try
            {
                byte[] byteSalt = new byte[5];
                RandomNumberGenerator.Fill(byteSalt);

                byte[] passwordBytes = Encoding.Unicode.GetBytes(password);
                byte[] combined = new byte[passwordBytes.Length + byteSalt.Length];
                Buffer.BlockCopy(passwordBytes, 0, combined, 0, passwordBytes.Length);
                Buffer.BlockCopy(byteSalt, 0, combined, passwordBytes.Length, byteSalt.Length);

                using var sha256 = SHA256.Create();
                byte[] hashBytes = sha256.ComputeHash(combined);

                passwordHash = Convert.ToBase64String(hashBytes);
                passwordSalt = Convert.ToBase64String(byteSalt);
            }
            catch
            {
                throw;
            }

            return new KeyValuePair<string, string>(passwordSalt, passwordHash);
        }
    }
}
