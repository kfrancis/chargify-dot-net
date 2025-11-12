#region License, Terms and Conditions
// ...existing license header...
#endregion

using System;
#if NETFULL
using System.Configuration;
#endif
#if NET8_0_OR_GREATER
using Microsoft.Extensions.Configuration;
#endif

namespace ChargifyDotNet.Configuration
{
    /// <summary>
    /// Factory to obtain an IChargifyConfig across different target frameworks.
    /// </summary>
    public static class ChargifyConfigFactory
    {
#if NETFULL
        public static IChargifyConfig Create()
        {
            if (ConfigurationManager.GetSection("chargify") is not ChargifyConfigSection section)
            {
                throw new ConfigurationErrorsException("Missing <chargify> section in configuration.");
            }

            return section;
        }
#endif
#if NET8_0_OR_GREATER
        public static IChargifyConfig Create(IConfiguration configuration)
        {
            return new ChargifyConfig(configuration);
        }
#endif
    }
}
