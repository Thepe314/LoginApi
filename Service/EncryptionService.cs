

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

        //Encrypt: Turns plain password into unreadable format
        public string Encrypt(string password)
        {
            // Create a new Aes encryption instance
            using var aes = Aes.Create();

            //Set the secret key (32 bytes)
            aes.Key = _key;

            // Generate a random IV (Initialization Vector) each time
            // IV ensures same password produces different encrypted output each time
            aes.GenerateIV();
            
            // Create the encryptor using the key and IV
            using var encryptor = aes.CreateEncryptor();

            // Convert the plain text password into bytes
            var passwordBytes = Encoding.UTF8.GetBytes(password);

            // Encrypt the password bytes into encrypted bytes
            var encrypted = encryptor.TransformFinalBlock(passwordBytes, 0, passwordBytes.Length);

            // Combine IV + encrypted bytes into one array
            // We store IV with the encrypted data so we can use it during decryption
            // IV is not secret, it just needs to be the same during decrypt
            var results = new byte[aes.IV.Length + encrypted.Length];
            aes.IV.CopyTo(results,0);
            encrypted.CopyTo(results, aes.IV.Length);

            // Convert combined bytes to Base64 string for safe DB storage
            return Convert.ToBase64String(results);
         }

         //Decrypt: Convert the encrypted password into plain text pw
        public string Decrypt(string encryptedPassword)
        {
            // Convert Base64 string back to bytes
            // This gives us the combined IV + encrypted bytes we stored
            var fullBytes = Convert.FromBase64String(encryptedPassword);
            
            Console.WriteLine($"fullBytes length: {fullBytes.Length}"); 

            // Create a new AES instance for decryption
            using var aes = Aes.Create();

            // Set the same secret key used during encryption
            aes.Key = _key;

             // Extract the IV from first 16 bytes
            // We need the same IV that was used during encryption
            var iv = new byte[16]; 
            
            Console.WriteLine($"iv length: {iv.Length}");          
            Console.WriteLine($"encrypted length: {fullBytes.Length - iv.Length}"); 

            // Guard: encrypted data must be longer than IV (16 bytes)
            // If not, data is corrupted or was not encrypted with AES
            if (fullBytes.Length <= 16)
                throw new Exception($"Encrypted data too short: {fullBytes.Length} bytes"); 

             // Extract the actual encrypted data (everything after the IV)
            var encrypted = new byte[fullBytes.Length - iv.Length];

            // Copy first 16 bytes into IV
            Buffer.BlockCopy(fullBytes, 0, iv, 0, 16);

            // Copy remaining bytes into encrypted array
            Buffer.BlockCopy(fullBytes, 16, encrypted, 0, encrypted.Length);

            // Set the extracted IV so AES knows how to decrypt
            aes.IV = iv;

            // Create the decryptor using same key and extracted IV
            using var decryptor = aes.CreateDecryptor();

            // Decrypt the encrypted bytes back to plain text bytes
            var decrypted = decryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);

            // Convert decrypted bytes back to plain text string
            return Encoding.UTF8.GetString(decrypted);
        }

        //Verify Password :Check if input password matches the encrypted password in db
        public bool VerifyPassword(string inputPassword,string encryptedPassword)
        {
            // Decrypt the stored password back to plain text
            var decrypted = Decrypt(encryptedPassword);

             // Compare decrypted password with what user typed
             // Returns true if match, false if not
            return decrypted == inputPassword;
        }
    }
}