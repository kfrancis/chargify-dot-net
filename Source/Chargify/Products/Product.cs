using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Chargify
{
    [XmlRoot("product")]
    public class Product : IChargifyEntity
    {
        public int price_in_cents { get; set; }
        public string name { get; set; }
        public string handle { get; set; }
        public string description { get; set; }
        public string accounting_code { get; set; }
        public IntervalUnit interval_unit { get; set; }
        public int interval { get; set; }

        [XmlIgnore]
        public ProductFamily product_family { get; set; }
        [XmlIgnore]
        public string return_url { get; set; }
        [XmlIgnore]
        public int? trial_interval { get; set; }
        [XmlIgnore]
        public string return_params { get; set; }
        [XmlIgnore]
        public string expiration_interval { get; set; }
        [XmlIgnore]
        public DateTime? updated_at { get; set; }
        [XmlIgnore]
        public int id { get; set; }
        [XmlIgnore]
        public string update_return_url { get; set; }
        [XmlIgnore]
        public DateTime? created_at { get; set; }
        [XmlIgnore]
        public string trial_interval_unit { get; set; }
        [XmlIgnore]
        public string expiration_interval_unit { get; set; }
        [XmlIgnore]
        public DateTime? archived_at { get; set; }
        [XmlIgnore]
        public bool request_credit_card { get; set; }
        [XmlIgnore]
        public bool require_credit_card { get; set; }
        [XmlIgnore]
        public int? initial_charge_in_cents { get; set; }
        [XmlIgnore]
        public int? trial_price_in_cents { get; set; }
    }

    public enum IntervalUnit
    {
        [XmlEnum("month")]
        month,
        [XmlEnum("day")]
        day
    }
}
