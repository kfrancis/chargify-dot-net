namespace ChargifyNET;

using System.Text.Json.Serialization;

// Breaking convention here by making a straight DTO because i actually want to finish this ticket.
// Just use a damned json deserializer
public class ComponentPricePointCurrencyPrice
{
    /// <summary>
    /// The currency price ID if this is set by definitive pricing, or null if currency conversion is used.
    /// </summary>
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; }

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("formatted_price")]
    public string FormattedPrice { get; set; }

    [JsonPropertyName("price_id")]
    public int PriceId { get; set; }

    [JsonPropertyName("price_point_id")]
    public int PricePointId { get; set; }
}
