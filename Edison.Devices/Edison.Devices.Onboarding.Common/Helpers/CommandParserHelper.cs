using Edison.Devices.Onboarding.Common.Models;
using Newtonsoft.Json;

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

        public static string SerializeCommand(Command command)
        {
            return JsonConvert.SerializeObject(command);
        }

        public static Command DeserializeCommand(string commandData)
        {
            return JsonConvert.DeserializeObject<Command>(commandData);
        }

        public static string EncryptMessage(string message, string socketKey)
        {
            return StringCipher.Encrypt(message, socketKey);
        }

        public static string DecryptMessage(string message, string socketKey)
        {
            return StringCipher.Decrypt(message, socketKey);
        }
    }
}
