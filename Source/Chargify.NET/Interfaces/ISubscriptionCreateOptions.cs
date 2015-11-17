using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ChargifyNET
{
    public interface ISubscriptionCreateOptions
    {
        string ProductHandle { get; set; }
        int? ProductID { get; set; }

        int? CustomerID { get; set; }
        string CustomerReference { get; set; }
        CustomerAttributes CustomerAttributes { get; set; }

        int? PaymentProfileID { get; set; }
        PaymentProfileAttributes PaymentProfileAttributes { get; set; }
        CreditCardAttributes CreditCardAttributes { get; set; }

        string CancellationMessage { get; set; }
        DateTime? NextBillingAt { get; set; }
        bool? ExpirationTracksNextBillingChange { get; set; }
        string VatNumber { get; set; }
        string CouponCode { get; set; }
        PaymentCollectionMethod? PaymentCollectionMethod { get; set; }
        string AgreementTerms { get; set; }
        bool? ProductChangeDelayed { get; set; }

        // TODO: Add this
        //CalendarBillingAttributes CalendarBilling { get; set; } 

        //Dictionary<string, string> Metafields { get; set; }
        string ReferralCode { get; set; }
        //Dictionary<int, string> Components { get; set; }
    }

    [XmlType("subscription")]
    [Serializable]
    public class SubscriptionCreateOptions: ISubscriptionCreateOptions
    {
        [XmlElement("agreement_terms")]
        public string AgreementTerms { get; set; }

        [XmlElement("cancellation_message")]
        public string CancellationMessage { get; set; }

        //[XmlElement("components")]
        //public Dictionary<int, string> Components { get; set; }

        [XmlElement("coupon_code")]
        public string CouponCode { get; set; }

        [XmlElement("customer_attributes")]
        public CustomerAttributes CustomerAttributes { get; set; }

        [XmlElement("customer_id")]
        public int? CustomerID { get; set; }

        [XmlElement("customer_reference")]
        public string CustomerReference { get; set; }

        [XmlElement("expiration_tracks_next_billing_change")]
        public bool? ExpirationTracksNextBillingChange { get; set; }

        //[XmlElement("metafields")]
        //public Dictionary<string, string> Metafields { get; set; }

        [XmlElement("next_billing_at")]
        public DateTime? NextBillingAt { get; set; }

        [XmlElement("payment_collection_method")]
        public PaymentCollectionMethod? PaymentCollectionMethod { get; set; }

        [XmlElement("payment_profile_attributes")]
        public PaymentProfileAttributes PaymentProfileAttributes { get; set; }

        [XmlElement("credit_card_attributes")]
        public CreditCardAttributes CreditCardAttributes { get; set; }

        [XmlElement("payment_profile_id")]
        public int? PaymentProfileID { get; set; }

        [XmlElement("product_change_delayed")]
        public bool? ProductChangeDelayed { get; set; }

        [XmlElement("product_handle")]
        public string ProductHandle { get; set; }

        [XmlElement("product_id")]
        public int? ProductID { get; set; }

        [XmlElement("ref")]
        public string ReferralCode { get; set; }

        [XmlElement("vat_number")]
        public string VatNumber { get; set; }
    }
}
