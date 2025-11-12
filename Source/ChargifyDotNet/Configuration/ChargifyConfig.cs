#region License, Terms and Conditions

// ...existing license header...

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
#if NET8_0_OR_GREATER
using Microsoft.Extensions.Configuration;
#endif

namespace ChargifyDotNet.Configuration
{
#if NET8_0_OR_GREATER
    /// <summary>
    ///     Default implementation of IChargifyConfig for modern .NET targets using
    ///     Microsoft.Extensions.Configuration.
    ///     Expected JSON structure:
    ///     "Chargify": {
    ///     "DefaultAccount": "main",
    ///     "UseJSON": false,
    ///     "Accounts": [ { "Name": "main", "Site": "https://....", "ApiKey": "...", "ApiPassword": "...", "SharedKey": "...",
    ///     "CvvRequired": true } ]
    ///     }
    /// </summary>
    public class ChargifyConfig : IChargifyConfig
    {
        private readonly IReadOnlyList<AccountConfig> _accounts;

        public ChargifyConfig(IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            var section = configuration.GetSection("Chargify");
            DefaultAccount = section["DefaultAccount"];
            UseJSON = bool.TryParse(section["UseJSON"], out var useJsonParsed) && useJsonParsed;

            var accounts = new List<AccountConfig>();
            section.GetSection("Accounts").Bind(accounts);

            _accounts = accounts;
        }

        public string? DefaultAccount { get; }
        public bool UseJSON { get; }

        public IChargifyAccountConfig GetDefaultOrFirst()
        {
            var acct = (!string.IsNullOrEmpty(DefaultAccount)
                           ? _accounts.FirstOrDefault(a => a.Name == DefaultAccount)
                           : null) ??
                       _accounts[0];

            return acct ?? throw new InvalidOperationException("No Chargify accounts configured (Chargify:Accounts)");
        }

        public string GetSharedKeyForDefaultOrFirstSite()
        {
            return GetDefaultOrFirst().SharedKey;
        }

        private class AccountConfig : IChargifyAccountConfig
        {
            public string Name { get; } = string.Empty;
            public string Site { get; } = string.Empty;
            public string ApiKey { get; } = string.Empty;
            public string ApiPassword { get; } = string.Empty;
            public string SharedKey { get; } = string.Empty;
            public bool CvvRequired { get; set; }
        }
    }
#endif
}
