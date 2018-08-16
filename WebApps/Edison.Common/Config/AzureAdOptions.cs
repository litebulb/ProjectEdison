﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Edison.Common.Config
{
    public class AzureAdOptions
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string Instance { get; set; }

        public string TenantId { get; set; }
    }
}
