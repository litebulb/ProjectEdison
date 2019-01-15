using System;
using Newtonsoft.Json;

namespace Edison.Mobile.Admin.Client.Core.Models
{
    public sealed class ResultCommandGenerateCSR : ResultCommand
    {
        public string Csr { get; set; }
    }
}
