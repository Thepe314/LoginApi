

using System.Security.Cryptography;
using System.Text;

namespace LoginApi.Service{

    public class EncryptionService
    {
        //Readonly
        private readonly byte[] _key;

        public EncryptionService(IConfiguration configuration)
        {
            _key = Convert.FromBase64String(configuration["EncryptionSettings:Key"]);
        }

        //Encrypt
        public string Encrypt(string password)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var encrypted = encryptor.TransformFinalBlock(passwordBytes, 0, passwordBytes.Length);

            //Combine VI + encrypted into one string
            var results = new byte[aes.IV.Length + encrypted.Length];
            aes.IV.CopyTo(results,0);
            encrypted.CopyTo(results, aes.IV.Length);

            return Convert.ToBase64String(results);
         }

         //Decrypt
        public string Decrypt(string encryptedPassword)
        {
            var fullBytes = Convert.FromBase64String(encryptedPassword);
            
            Console.WriteLine($"fullBytes length: {fullBytes.Length}"); // ← add this

            using var aes = Aes.Create();
            aes.Key = _key;

            var iv = new byte[16]; // ← use hardcoded 16 instead of aes.BlockSize/8
            
            Console.WriteLine($"iv length: {iv.Length}");           // ← add this
            Console.WriteLine($"encrypted length: {fullBytes.Length - iv.Length}"); // ← add this

            if (fullBytes.Length <= 16)
                throw new Exception($"Encrypted data too short: {fullBytes.Length} bytes"); // ← add this

            var encrypted = new byte[fullBytes.Length - iv.Length];
            Buffer.BlockCopy(fullBytes, 0, iv, 0, 16);
            Buffer.BlockCopy(fullBytes, 16, encrypted, 0, encrypted.Length);

            aes.IV = iv;
            using var decryptor = aes.CreateDecryptor();
            var decrypted = decryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);

            return Encoding.UTF8.GetString(decrypted);
        }

        //Verify Password
        public bool VerifyPassword(string inputPassword,string encryptedPassword)
        {
            var decrypted = Decrypt(encryptedPassword);
            return decrypted == inputPassword;
        }
    }
}