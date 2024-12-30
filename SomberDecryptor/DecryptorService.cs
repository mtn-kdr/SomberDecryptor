using System;
using System.IO;
using System.Security.Cryptography;

public class DecryptionService
{
    public static void DecryptFile(string inputFilePath, string outputFilePath, byte[] key)
    {
        try 
        {
            using (var inputFileStream = new FileStream(inputFilePath, FileMode.Open))
            {
                byte[] salt = new byte[16];
                byte[] iv = new byte[16];
                
                inputFileStream.Read(salt, 0, salt.Length);
                inputFileStream.Read(iv, 0, iv.Length);

                using (var aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;

                    using (var decryptor = aes.CreateDecryptor())
                    using (var cryptoStream = new CryptoStream(inputFileStream, decryptor, CryptoStreamMode.Read))
                    using (var outputFileStream = new FileStream(outputFilePath, FileMode.Create))
                    {
                        byte[] buffer = new byte[4096];
                        int read;
                        while ((read = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            outputFileStream.Write(buffer, 0, read);
                        }
                    }
                }
            }
        }
        catch (CryptographicException ex)
        {
            throw new Exception("Şifre çözme hatası: Yanlış anahtar veya bozuk dosya.", ex);
        }
        catch (Exception ex)
        {
            throw new Exception($"Dosya şifre çözme işlemi başarısız: {ex.Message}", ex);
        }
    }

    public static void DecryptFolder(string folderPath, byte[] key)
    {
        try
        {
            string[] encryptedFiles = Directory.GetFiles(folderPath, "*.QUEEN");
            
            foreach (string encryptedFile in encryptedFiles)
            {
                string decryptedFile = encryptedFile.Replace(".QUEEN", "");
                DecryptFile(encryptedFile, decryptedFile, key);
                
                //Şifresi çözülen dosyayı silsek mi gıcık mı olsak 
                // File.Delete(encryptedFile);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Klasör şifre çözme işlemi başarısız: {ex.Message}", ex);
        }
    }
}
