using System;
using System.Collections.Generic;

namespace Edison.Core.Common.Models
{
    public class UserModel
    {
        public string UserId { get; set; }
        public string AzureObjectId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
