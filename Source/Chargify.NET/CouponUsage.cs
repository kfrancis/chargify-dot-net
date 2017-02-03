using System;
using System.Xml;
using ChargifyNET.Json;

namespace ChargifyNET
{
    public class CouponUsage : ChargifyBase, ICouponUsage, IComparable<CouponUsage>
    {
        #region Field Keys
        private const string ProductNameKey = "name";
        private const string ProductIdKey = "id";
        private const string RevenueKey = "revenue";
        private const string SignupsKey = "signups";
        private const string SavingsKey = "savings";
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor.  Values set to default
        /// </summary>
        public CouponUsage()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="couponUsageXml">XML containing coupon usage info (in expected format)</param>
        public CouponUsage(string couponUsageXml)
        {
            // get the XML into an XML document
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(couponUsageXml);
            if (doc.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(couponUsageXml));
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in doc.ChildNodes)
            {
                if (elementNode.Name == "object")
                {
                    LoadFromNode(elementNode);
                    return;
                }
            }
            // if we get here, then no info was found
            throw new ArgumentException("XML does not contain coupon usage information", nameof(couponUsageXml));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="couponUsageNode">XML containing coupon info (in expected format)</param>
        internal CouponUsage(XmlNode couponUsageNode)
        {
            if (couponUsageNode == null) throw new ArgumentNullException(nameof(couponUsageNode));
            if (couponUsageNode.Name != "object") throw new ArgumentException("Not a vaild coupon usage node", nameof(couponUsageNode));
            if (couponUsageNode.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(couponUsageNode));
            LoadFromNode(couponUsageNode);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="couponUsageObject">JsonObject containing coupon usage info (in expected format)</param>
        public CouponUsage(JsonObject couponUsageObject)
        {
            if (couponUsageObject == null) throw new ArgumentNullException(nameof(couponUsageObject));
            if (couponUsageObject.Keys.Count <= 0) throw new ArgumentException("Not a vaild coupon usage object", nameof(couponUsageObject));
            LoadFromJson(couponUsageObject);
        }

        /// <summary>
        /// Load data from a JsonObject
        /// </summary>
        /// <param name="obj">The JsonObject containing coupon data</param>
        private void LoadFromJson(JsonObject obj)
        {
            // loop through the keys of this JsonObject to get coupon info, and parse it out
            foreach (string key in obj.Keys)
            {
                switch (key)
                {
                    case ProductNameKey:
                        _productName = obj.GetJSONContentAsString(key);
                        break;
                    case ProductIdKey:
                        _productId = obj.GetJSONContentAsInt(key);
                        break;
                    case RevenueKey:
                        _revenue = obj.GetJSONContentAsDecimal(key);
                        break;
                    case SavingsKey:
                        _savings = obj.GetJSONContentAsDecimal(key);
                        break;
                    case SignupsKey:
                        _signups = obj.GetJSONContentAsInt(key);
                        break;
                }
            }
        }

        /// <summary>
        /// Load data from a coupon node
        /// </summary>
        /// <param name="couponNode">The coupon node</param>
        private void LoadFromNode(XmlNode couponNode)
        {
            foreach (XmlNode dataNode in couponNode.ChildNodes)
            {
                switch (dataNode.Name)
                {
                    case ProductNameKey:
                        _productName = dataNode.GetNodeContentAsString();
                        break;
                    case ProductIdKey:
                        _productId = dataNode.GetNodeContentAsInt();
                        break;
                    case RevenueKey:
                        _revenue = dataNode.GetNodeContentAsDecimal();
                        break;
                    case SavingsKey:
                        _savings = dataNode.GetNodeContentAsDecimal();
                        break;
                    case SignupsKey:
                        _signups = dataNode.GetNodeContentAsInt();
                        break;
                }
            }
        }

        #endregion

        #region ICouponUsage Members
        /// <summary>
        ///  Product name
        /// </summary>
        public string ProductName {
            get { return _productName; }
            set { _productName = value; }
        }
        private string _productName = string.Empty;

        /// <summary>
        ///  Product ID
        /// </summary>
        public int ProductId
        {
            get {  return _productId;}
            set { _productId = value; }
        }
        private int _productId = Int32.MinValue;

        /// <summary>
        /// Revenue from subscriptions that used coupon
        /// </summary>
        public decimal Revenue
        {
            get { return _revenue; }
            set { _revenue = value; }
        }
        private decimal _revenue = decimal.MinValue;

        /// <summary>
        /// Number of signups that used coupon
        /// </summary>
        public int Signups
        {
            get { return _signups; }
            set { _signups = value; }
        }
        private int _signups = int.MinValue;

        /// <summary>
        /// Savings given by the coupon for this product
        /// </summary>
        public decimal Savings
        {
            get { return _savings; }
            set { _savings = value; }
        }
        private decimal _savings = decimal.MinValue;

        #endregion

        #region IComparable<ICouponUsage> Members

        /// <summary>
        /// Method for comparing one coupon usage to another
        /// </summary>
        public int CompareTo(ICouponUsage other)
        {
            return Revenue.CompareTo(other.Revenue);
        }

        #endregion

        #region IComparable<Coupon> Members

        /// <summary>
        /// Method for comparing one coupon usage to another
        /// </summary>
        public int CompareTo(CouponUsage other)
        {
            return Revenue.CompareTo(other.Revenue);
        }
        #endregion
    }
}
