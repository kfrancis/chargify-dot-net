namespace ChargifyNET
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class ComponentPricePointInfo : IComparable<ComponentPricePointInfo>, IComponentPricePointInfo
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("default")]
        public bool Default { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("pricing_scheme")]
        public string PricingScheme { get; set; }

        [JsonPropertyName("component_id")]
        public int ComponentId { get; set; }

        [JsonPropertyName("handle")]
        public string Handle { get; set; }

        [JsonPropertyName("archived_at")]
        public DateTime? ArchivedAt { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [JsonPropertyName("use_site_exchange_rate")]
        public bool UseSiteExchangeRate { get; set; }

        [JsonPropertyName("currency_prices")]
        public IEnumerable<ComponentPricePointCurrencyPrice> CurrencyPrices { get; set; }

        [JsonPropertyName("prices")]
        public IEnumerable<ComponentPricePointPrice> Prices { get; set; }

        [JsonPropertyName("tax_included")]
        public bool TaxIncluded { get; set; }

        public int CompareTo(ComponentPricePointInfo other)
        {
            return Id.CompareTo(other.Id);
        }
    }
}
