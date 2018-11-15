using Edison.Devices.Onboarding.Common.Models;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Edison.Devices.Onboarding.Common.Helpers
{
    public class CommandParserHelper
    {
        public static string SerializeAndEncryptCommand(Command command, string socketKey)
        {
            string value = JsonConvert.SerializeObject(command);
            if (!string.IsNullOrEmpty(value))
            {
                return StringCipher.Encrypt(value, socketKey);
            }
            return string.Empty;
        }

        public static Command DeserializeAndDecryptCommand(string commandData, string socketKey)
        {
            if (!string.IsNullOrEmpty(commandData))
            {
                string decryptedData = StringCipher.Decrypt(commandData, socketKey);
                return JsonConvert.DeserializeObject<Command>(decryptedData);
            }
            return null;
        }
    }
}
