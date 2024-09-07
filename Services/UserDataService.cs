/**
 * @QumartSeller_Client
 * https://github.com/Nailed34/QumartSeller_Client-demo.git
 *
 * Copyright (c) 2024 https://github.com/Nailed34
 * Released under the MIT license
 */

using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ClientWPF.Services
{
    public enum EUserDataLoadStatus
    {
        None, Loaded, NewFile, Error
    }

    // User data struct used for serialization
    file struct UserData
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    internal class UserDataService
    {
        /// <summary>
        /// Username from save file
        /// </summary>
        public string Username 
        { 
            get => _username; 
            set => _username = value;
        }
        private static string _username = "";

        /// <summary>
        /// Password from save file
        /// </summary>
        public string Password 
        {
            get => _password;
            set => _password = value;
        }
        private static string _password = "";

        /// <summary>
        /// Current status of user data loading
        /// </summary>
        public EUserDataLoadStatus UserDataLoadStatus 
        {
            get => _userDataLoadStatus;
            private set => _userDataLoadStatus = value; 
        }
        private static EUserDataLoadStatus _userDataLoadStatus = EUserDataLoadStatus.None;

        // File name for saving data
        private static string AuthFilePath { get; set; } = "UserData.enc";

        // Encryption keys
        private static string EncryptionKey { get; set; } = "MLmk3RYjMffZLXciiCIrt7c9qNSr";
        private static string EncryptionSecretIV { get; set; } = "qoJMXaHOV5VuEblhzfCERhG1no5R";
        private static Aes Aes { get; set; }

        static UserDataService()
        {
            Aes = Aes.Create();
            Aes.IV = GetEncryptionSecretIV();
            Aes.Key = GetEncryptionKey();
        }

        /// <summary>
        /// Load user data info from file, return result status
        /// </summary>
        public EUserDataLoadStatus LoadUserData()
        {
            FileInfo fileInfo = new FileInfo(AuthFilePath);

            if (!fileInfo.Exists)
                return UserDataLoadStatus = EUserDataLoadStatus.NewFile;

            try
            {
                string bin = ReadFileEncrypted();
                var userData = JsonSerializer.Deserialize<UserData>(bin);

                Username = userData.Username;
                Password = userData.Password;

                return UserDataLoadStatus = EUserDataLoadStatus.Loaded;
            }
            catch
            {
                return UserDataLoadStatus = EUserDataLoadStatus.Error;
            }
        }

        /// <summary>
        /// Save new values of user data to file, return bool result. (Also change UserDataLoadStatus to loaded on success)
        /// </summary>
        public bool SaveUserData()
        {
            try
            {
                string saveData = JsonSerializer.Serialize(new UserData() { Username = Username, Password = Password });
                WriteFileEncrypted(saveData);

                UserDataLoadStatus = EUserDataLoadStatus.Loaded;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static byte[] GetEncryptionKey()
        {
            SHA256 sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(EncryptionKey));
        }
        private static byte[] GetEncryptionSecretIV()
        {
            MD5 md5 = MD5.Create();
            return md5.ComputeHash(Encoding.UTF8.GetBytes(EncryptionSecretIV));
        }

        private static void WriteFileEncrypted(string data)
        {
            try
            {
                using (FileStream fileStream = new FileStream(AuthFilePath, FileMode.Create))
                {
                    using (CryptoStream encFileStream = new CryptoStream(fileStream, Aes.CreateEncryptor(Aes.Key, Aes.IV), CryptoStreamMode.Write))
                    {
                        byte[] bin = Encoding.Unicode.GetBytes(data);
                        encFileStream.Write(bin, 0, bin.Length);
                    }
                }              
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }

        private static string ReadFileEncrypted()
        {
            try
            {
                string finalString = "";

                using (FileStream fileStream = File.OpenRead(AuthFilePath))
                {
                    using (CryptoStream encFileStream = new CryptoStream(fileStream, Aes.CreateDecryptor(Aes.Key, Aes.IV), CryptoStreamMode.Read))
                    {
                        byte[] buffer = new byte[fileStream.Length];

                        int totalRead = 0;
                        while (totalRead < buffer.Length)
                        {
                            int bytesRead = encFileStream.Read(buffer.AsSpan(totalRead));
                            if (bytesRead == 0) break;
                            totalRead += bytesRead;
                        }
                        finalString = Encoding.Unicode.GetString(buffer).TrimEnd('\0');
                    }
                }
                return finalString;           
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}
