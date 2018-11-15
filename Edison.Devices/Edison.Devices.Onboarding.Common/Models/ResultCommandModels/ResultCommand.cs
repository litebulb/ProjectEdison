using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edison.Devices.Onboarding.Common.Models
{
    public class ResultCommand
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }

        public ResultCommand()
        {

        }

        public static T CreateFailedCommand<T>(string message) where T : ResultCommand, new()
        { 
            return new T()
            {
                IsSuccess = false,
                ErrorMessage = message
            };
        }

        public static ResultCommand CreateFailedCommand(string message)
        {
            return new ResultCommand()
            {
                IsSuccess = false,
                ErrorMessage = message
            };
        }

        public static ResultCommand CreateSuccessCommand()
        {
            return new ResultCommand()
            {
                IsSuccess = true
            };
        }
    }
}
