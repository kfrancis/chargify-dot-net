namespace ChargifyNET
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using ChargifyNET.Json;


    public class Component : IComparable<Component>, IComponentPricePointInfo
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get;set; }

        [JsonPropertyName("handle")]
        public string Handle { get; set; }

        [JsonPropertyName("pricing_scheme")]
        public string PricingScheme { get; set; }

        [JsonPropertyName("unit_name")]
        public string UnitName { get; set; }

        [JsonPropertyName("unit_price")]
        public decimal UnitPrice { get; set; }

        [JsonPropertyName("product_family_id")]
        public int ProductFamilyId { get; set; }

        [JsonPropertyName("product_family_name")]
        public string ProductFamilyName { get; set; }

        [JsonPropertyName("price_per_unit_in_cents")]
        public int? PricePerUnitInCents { get; set; }

        [JsonPropertyName("kind")]
        public ComponentType Kind { get; set; }

        [JsonPropertyName("archived")]
        public bool Archived { get; set; }

        [JsonPropertyName("taxable")]
        public bool Taxable { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("default_price_point_id")]
        public int DefaultPricePointId { get; set; }

        [JsonPropertyName("prices")]
        public IEnumerable<ComponentPricePointPrice> Prices { get; set; }

        [JsonPropertyName("price_point_count")]
        public int PricePointCount { get; set; }

        [JsonPropertyName("price_points_url")]
        public string PricePointsUrl { get; set; }

        [JsonPropertyName("default_price_point_name")]
        public string DefaultPricePointName { get; set; }

        [JsonPropertyName("tax_code")]
        public string TaxCode { get; set; }

        [JsonPropertyName("recurring")]
        public bool Recurring { get; set; }

        [JsonPropertyName("upgrade_charge")]
        public string UpgradeCharge { get; set; }

        [JsonPropertyName("downgrade_credit")]
        public string DowngradeCredit { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [JsonPropertyName("archived_at")]
        public DateTime? ArchivedAt { get; set; }

        [JsonPropertyName("hide_date_range_on_invoice")]
        public bool HideDateRangeOnInvoice { get; set; }

        [JsonPropertyName("allow_fractional_quantities")]
        public bool AllowFractionalQuantities { get; set; }

        [JsonPropertyName("use_site_exchange_rate")]
        public bool UseSiteExchangeRate { get; set; }

        [JsonPropertyName("item_category")]
        public string ItemCategory { get;set; }

        [JsonPropertyName("accounting_code")]
        public string AccountingCode { get; set; }

        [JsonPropertyName("currency_prices")]
        public IEnumerable<ComponentPricePointCurrencyPrice> CurrencyPrices { get; set; }

        [JsonIgnore]
        public bool TaxIncluded => false;

        #region IComparable<ComponentInfo> Members

        /// <summary>
        /// Compare this ComponentInfo to another
        /// </summary>
        public int CompareTo(Component other)
        {
            return this.Id.CompareTo(other.Id);
        }

        #endregion
    }
}
