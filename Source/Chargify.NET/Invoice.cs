namespace ChargifyNET
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using ChargifyNET.Json;

    /// <summary>
    /// Invoice Billing allows you to bill your customers manually by sending them an invoice each month. Subscriptions with invoice billing enabled will not be charged automatically.
    /// </summary>
    public class Invoice : ChargifyEntity, IInvoice
    {
        #region Field Keys
        private const string SubscriptionIDKey = "subscription_id";
        private const string StatementIDKey = "statement_id";
        private const string SiteIDKey = "site_id";
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
        private Invoice() : base() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="invoiceXML">An XML string containing a invoice node</param>
        public Invoice(string invoiceXML)
            : base()
        {
            // get the XML into an XML document
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(invoiceXML);
            if (doc.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", "invoiceXML");
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
            throw new ArgumentException("XML does not contain invoice information", "invoiceXML");
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="invoiceNode">An xml node with invoice information</param>
        public Invoice(XmlNode invoiceNode)
            : base()
        {
            if (invoiceNode == null) throw new ArgumentNullException("invoiceNode");
            if (invoiceNode.Name != "invoice") throw new ArgumentException("Not a vaild invoice node", "invoiceNode");
            if (invoiceNode.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", "invoiceNode");
            this.LoadFromNode(invoiceNode);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="invoiceObject">An JsonObject with invoice information</param>
        public Invoice(JsonObject invoiceObject)
            : base()
        {
            if (invoiceObject == null) throw new ArgumentNullException("invoiceObject");
            if (invoiceObject.Keys.Count <= 0) throw new ArgumentException("Not a vaild invoice object", "invoiceObject");
            this.LoadFromJSON(invoiceObject);
        }

        private void LoadFromJSON(JsonObject obj)
        {
            // loop through the keys of this JsonObject to get invoice info, and parse it out
            foreach (string key in obj.Keys)
            {
                switch (key)
                {
                    case IDKey:
                        m_id = obj.GetJSONContentAsInt(key);
                        break;
                    case SubscriptionIDKey:
                        _subscriptionID = obj.GetJSONContentAsInt(key);
                        break;
                    case StatementIDKey:
                        _statementID = obj.GetJSONContentAsInt(key);
                        break;
                    case SiteIDKey:
                        _siteID = obj.GetJSONContentAsInt(key);
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
                            foreach (JsonObject charge in chargesArray.Items)
                            {
                                _charges.Add(charge.GetJSONContentAsCharge("charge"));
                            }
                        }
                        // Sanity check, should be equal.
                        if (chargesArray.Length != _charges.Count)
                        {
                            throw new JsonParseException(string.Format("Unable to parse charges ({0} != {1})", chargesArray.Length, _charges.Count));
                        }
                        break;
                    case PaymentsAndCreditsKey:
                        break;
                    default:
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
                    case SubscriptionIDKey:
                        _subscriptionID = dataNode.GetNodeContentAsInt();
                        break;
                    case StatementIDKey:
                        _statementID = dataNode.GetNodeContentAsInt();
                        break;
                    case SiteIDKey:
                        _siteID = dataNode.GetNodeContentAsInt();
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
                                default:
                                    break;
                            }
                        }
                        break;
                    case PaymentsAndCreditsKey:
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion
        
        #region IInvoice Members
        /// <summary>
        /// The subscription unique id within Chargify
        /// </summary>
        public int SubscriptionID { get { return this._subscriptionID; } }
        private int _subscriptionID = int.MinValue;

        /// <summary>
        /// The statement unique id within Chargify
        /// </summary>
        public int StatementID { get { return this._statementID; } }
        private int _statementID = int.MinValue;

        /// <summary>
        /// The site unique id within Chargify
        /// </summary>
        public int SiteID { get { return this._siteID; } }
        private int _siteID = int.MinValue;

        /// <summary>
        /// The current state of the subscription associated with this invoice. Please see the documentation for Subscription States
        /// </summary>
        public SubscriptionState State { get { return this._state; } }
        private SubscriptionState _state = SubscriptionState.Unknown;

        /// <summary>
        /// Gives the current invoice amount in the number of cents (ie. the sum of charges, in cents)
        /// </summary>
        public int TotalAmountInCents { get { return this._totalAmountInCents; } }
        private int _totalAmountInCents = int.MinValue;

        /// <summary>
        /// Gives the current invoice amount in the number of cents (ie. the sum of charges, in dollars and cents)
        /// </summary>
        public decimal TotalAmount { get { return Convert.ToDecimal(this._totalAmountInCents) / 100; } }

        /// <summary>
        /// The date/time when the invoice was paid in full
        /// </summary>
        public DateTime PaidAt { get { return this._paidAt; } }
        private DateTime _paidAt = DateTime.MinValue;

        /// <summary>
        /// The creation date/time for this invoice
        /// </summary>
        public DateTime CreatedAt { get { return this._createdAt; } }
        private DateTime _createdAt = DateTime.MinValue;

        /// <summary>
        /// The date/time of last update for this invoice
        /// </summary>
        public DateTime UpdatedAt { get { return this._updatedAt; } }
        private DateTime _updatedAt = DateTime.MinValue;

        /// <summary>
        /// Gives the current outstanding invoice balance in the number of cents
        /// </summary>
        public int AmountDueInCents { get { return this._amountDueInCents; } }
        private int _amountDueInCents = int.MinValue;

        /// <summary>
        /// Gives the current outstanding invoice balance in the number of dollars and cents
        /// </summary>
        public decimal AmountDue { get { return Convert.ToDecimal(this._amountDueInCents) / 100; }  }

        /// <summary>
        /// The unique (to this site) identifier for this invoice
        /// </summary>
        public string Number { get { return this._number; } }
        private string _number = string.Empty;

        /// <summary>
        /// A list of charges applied to this invoice
        /// </summary>
        public List<ICharge> Charges { get { return _charges; } }
        private List<ICharge> _charges = new List<ICharge>();

        /// <summary>
        /// A list of the financial transactions that modify the amount due
        /// </summary>
        public List<IInvoicePaymentAndCredit> PaymentsAndCredits { get { return this._paymentsAndCredits; } }
        private List<IInvoicePaymentAndCredit> _paymentsAndCredits = new List<IInvoicePaymentAndCredit>();
        #endregion
    }
}
