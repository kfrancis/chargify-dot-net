namespace ChargifyDotNet.Tests.Base
{
    /// <summary>
    /// Strongly typed settings loaded from configuration (appsettings.json + overrides).
    /// </summary>
    public class ChargifySettings
    {
        public string? ApiKey { get; set; }
        public string? Password { get; set; }
        public string? Url { get; set; }
        public string? SharedKey { get; set; }
        public bool UseJson { get; set; } = false;
        public string Protocol { get; set; } = "Tls12"; // Maps to SecurityProtocolType
    }
}
