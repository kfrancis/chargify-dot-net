using System;
using System.Collections.Generic;
using System.Xml;
using ChargifyNET.Json;

namespace ChargifyNET
{
    /// <summary>
    /// Invoice Billing allows you to bill your customers manually by sending them an invoice each month. Subscriptions with invoice billing enabled will not be charged automatically.
    /// </summary>
    public class Invoice : ChargifyEntity, IInvoice
    {
        #region Field Keys
        private const string SubscriptionIdKey = "subscription_id";
        private const string StatementIdKey = "statement_id";
        private const string SiteIdKey = "site_id";
        private const string StateKey = "state";
        private const string TotalAmountInCentsKey = "total_amount_in_cents";
        private const string PaidAtKey = "paid_at";
        private const string CreatedAtKey = "created_at";
        private const string UpdatedAtKey = "updated_at";
        private const string AmountDueInCentsKey = "amount_due_in_cents";
        private const string NumberKey = "number";
        private const string ChargesKey = "charges";
        private const string PaymentsAndCreditsKey = "payments_and_credits";
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        private Invoice()
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="invoiceXml">An XML string containing a invoice node</param>
        public Invoice(string invoiceXml)
        {
            // get the XML into an XML document
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(invoiceXml);
            if (doc.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(invoiceXml));
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in doc.ChildNodes)
            {
                if (elementNode.Name == "invoice")
                {
                    LoadFromNode(elementNode);
                    return;
                }
            }
            // if we get here, then no invoice info was found
            throw new ArgumentException("XML does not contain invoice information", nameof(invoiceXml));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="invoiceNode">An xml node with invoice information</param>
        public Invoice(XmlNode invoiceNode)
        {
            if (invoiceNode == null) throw new ArgumentNullException(nameof(invoiceNode));
            if (invoiceNode.Name != "invoice") throw new ArgumentException("Not a vaild invoice node", nameof(invoiceNode));
            if (invoiceNode.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(invoiceNode));
            LoadFromNode(invoiceNode);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="invoiceObject">An JsonObject with invoice information</param>
        public Invoice(JsonObject invoiceObject)
        {
            if (invoiceObject == null) throw new ArgumentNullException(nameof(invoiceObject));
            if (invoiceObject.Keys.Count <= 0) throw new ArgumentException("Not a vaild invoice object", nameof(invoiceObject));
            LoadFromJson(invoiceObject);
        }

        private void LoadFromJson(JsonObject obj)
        {
            // loop through the keys of this JsonObject to get invoice info, and parse it out
            foreach (string key in obj.Keys)
            {
                switch (key)
                {
                    case IDKey:
                        m_id = obj.GetJSONContentAsInt(key);
                        break;
                    case SubscriptionIdKey:
                        _subscriptionId = obj.GetJSONContentAsInt(key);
                        break;
                    case StatementIdKey:
                        _statementId = obj.GetJSONContentAsInt(key);
                        break;
                    case SiteIdKey:
                        _siteId = obj.GetJSONContentAsInt(key);
                        break;
                    case StateKey:
                        _state = obj.GetJSONContentAsSubscriptionState(key);
                        break;
                    case TotalAmountInCentsKey:
                        _totalAmountInCents = obj.GetJSONContentAsInt(key);
                        break;
                    case PaidAtKey:
                        _paidAt = obj.GetJSONContentAsDateTime(key);
                        break;
                    case CreatedAtKey:
                        _createdAt = obj.GetJSONContentAsDateTime(key);
                        break;
                    case UpdatedAtKey:
                        _updatedAt = obj.GetJSONContentAsDateTime(key);
                        break;
                    case AmountDueInCentsKey:
                        _amountDueInCents = obj.GetJSONContentAsInt(key);
                        break;
                    case NumberKey:
                        _number = obj.GetJSONContentAsString(key);
                        break;
                    case ChargesKey:
                        _charges = new List<ICharge>();
                        JsonArray chargesArray = obj[key] as JsonArray;
                        if (chargesArray != null)
                        {
                            foreach (var jsonValue in chargesArray.Items)
                            {
                                var charge = (JsonObject) jsonValue;
                                _charges.Add(charge.GetJSONContentAsCharge("charge"));
                            }
                        }
                        // Sanity check, should be equal.
                        if (chargesArray != null && chargesArray.Length != _charges.Count)
                        {
                            throw new JsonParseException(string.Format("Unable to parse charges ({0} != {1})", chargesArray.Length, _charges.Count));
                        }
                        break;
                    case PaymentsAndCreditsKey:
                        break;
                }
            }
        }

        private void LoadFromNode(XmlNode obj)
        {
            // loop through the nodes to get invoice info
            foreach (XmlNode dataNode in obj.ChildNodes)
            {
                switch (dataNode.Name)
                {
                    case IDKey:
                        m_id = dataNode.GetNodeContentAsInt();
                        break;
                    case SubscriptionIdKey:
                        _subscriptionId = dataNode.GetNodeContentAsInt();
                        break;
                    case StatementIdKey:
                        _statementId = dataNode.GetNodeContentAsInt();
                        break;
                    case SiteIdKey:
                        _siteId = dataNode.GetNodeContentAsInt();
                        break;
                    case StateKey:
                        _state = dataNode.GetNodeContentAsSubscriptionState();
                        break;
                    case TotalAmountInCentsKey:
                        _totalAmountInCents = dataNode.GetNodeContentAsInt();
                        break;
                    case PaidAtKey:
                        _paidAt = dataNode.GetNodeContentAsDateTime();
                        break;
                    case CreatedAtKey:
                        _createdAt = dataNode.GetNodeContentAsDateTime();
                        break;
                    case UpdatedAtKey:
                        _updatedAt = dataNode.GetNodeContentAsDateTime();
                        break;
                    case AmountDueInCentsKey:
                        _amountDueInCents = dataNode.GetNodeContentAsInt();
                        break;
                    case NumberKey:
                        _number = dataNode.GetNodeContentAsString();
                        break;
                    case ChargesKey:
                         _charges = new List<ICharge>();
                        foreach (XmlNode childNode in dataNode.ChildNodes)
                        {
                            switch (childNode.Name)
                            {
                                case "charge":
                                    _charges.Add(childNode.GetNodeContentAsCharge());
                                    break;
                            }
                        }
                        break;
                    case PaymentsAndCreditsKey:
                        break;
                }
            }
        }
        #endregion
        
        #region IInvoice Members
        /// <summary>
        /// The subscription unique id within Chargify
        /// </summary>
        public int SubscriptionID { get { return _subscriptionId; } }
        private int _subscriptionId = int.MinValue;

        /// <summary>
        /// The statement unique id within Chargify
        /// </summary>
        public int StatementID { get { return _statementId; } }
        private int _statementId = int.MinValue;

        /// <summary>
        /// The site unique id within Chargify
        /// </summary>
        public int SiteID { get { return _siteId; } }
        private int _siteId = int.MinValue;

        /// <summary>
        /// The current state of the subscription associated with this invoice. Please see the documentation for Subscription States
        /// </summary>
        public SubscriptionState State { get { return _state; } }
        private SubscriptionState _state = SubscriptionState.Unknown;

        /// <summary>
        /// Gives the current invoice amount in the number of cents (ie. the sum of charges, in cents)
        /// </summary>
        public int TotalAmountInCents { get { return _totalAmountInCents; } }
        private int _totalAmountInCents = int.MinValue;

        /// <summary>
        /// Gives the current invoice amount in the number of cents (ie. the sum of charges, in dollars and cents)
        /// </summary>
        public decimal TotalAmount { get { return Convert.ToDecimal(_totalAmountInCents) / 100; } }

        /// <summary>
        /// The date/time when the invoice was paid in full
        /// </summary>
        public DateTime PaidAt { get { return _paidAt; } }
        private DateTime _paidAt = DateTime.MinValue;

        /// <summary>
        /// The creation date/time for this invoice
        /// </summary>
        public DateTime CreatedAt { get { return _createdAt; } }
        private DateTime _createdAt = DateTime.MinValue;

        /// <summary>
        /// The date/time of last update for this invoice
        /// </summary>
        public DateTime UpdatedAt { get { return _updatedAt; } }
        private DateTime _updatedAt = DateTime.MinValue;

        /// <summary>
        /// Gives the current outstanding invoice balance in the number of cents
        /// </summary>
        public int AmountDueInCents { get { return _amountDueInCents; } }
        private int _amountDueInCents = int.MinValue;

        /// <summary>
        /// Gives the current outstanding invoice balance in the number of dollars and cents
        /// </summary>
        public decimal AmountDue { get { return Convert.ToDecimal(_amountDueInCents) / 100; }  }

        /// <summary>
        /// The unique (to this site) identifier for this invoice
        /// </summary>
        public string Number { get { return _number; } }
        private string _number = string.Empty;

        /// <summary>
        /// A list of charges applied to this invoice
        /// </summary>
        public List<ICharge> Charges { get { return _charges; } }
        private List<ICharge> _charges = new List<ICharge>();

        /// <summary>
        /// A list of the financial transactions that modify the amount due
        /// </summary>
        public List<IInvoicePaymentAndCredit> PaymentsAndCredits { get { return _paymentsAndCredits; } }
        private List<IInvoicePaymentAndCredit> _paymentsAndCredits = new List<IInvoicePaymentAndCredit>();
        #endregion
    }
}
