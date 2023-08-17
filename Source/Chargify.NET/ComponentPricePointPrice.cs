using System;
using System.Text.Json.Serialization;
using System.Xml;
using ChargifyNET.Json;

namespace ChargifyNET
{
    public class ComponentPricePointPrice : IComparable<ComponentPricePointPrice>, IComparable<IComponentPricePointPrice>
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("component_id")]
        public int ComponentId { get; set; }

        [JsonPropertyName("starting_quantity")]
        public int? StartingQuantity { get; set; }

        [JsonPropertyName("ending_quantity")]
        public int? EndingQuantity { get; set; }

        [JsonPropertyName("unit_price")]
        public decimal UnitPrice { get; set; }

        [JsonPropertyName("price_point_id")]
        public int PricePointId { get; set; }

        [JsonPropertyName("formatted_unit_price")]
        public string FormattedUnitPrice { get; set; }

        #region IComparable<ComponentAttributes> Members

        /// <summary>
        /// Compare this ComponentAttributes to another
        /// </summary>
        public int CompareTo(ComponentPricePointPrice other)
        {
            return PricePointId.CompareTo(other.PricePointId);
        }

        #endregion

        #region IComparable<IComponentAttributes> Members

        /// <summary>
        /// Compare this IComponentAttributes to another
        /// </summary>
        public int CompareTo(IComponentPricePointPrice other)
        {
            return PricePointId.CompareTo(other.PricePointId);
        }

        #endregion
    }
}
