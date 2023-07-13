using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using ChargifyNET.Json;

namespace ChargifyNET
{
    public class ComponentPricePointInfo : ChargifyBase, IComparable<ComponentPricePointInfo>, IComponentPricePointInfo
    {
        #region Field Keys
        private const string IdKey = "id";
        private const string DefaultKey = "default";
        private const string NameKey = "name";
        private const string TypeKey = "type";
        private const string PricingSchemeKey = "pricing_scheme";
        private const string ComponentIdKey = "component_id";
        private const string HandleKey = "handle";
        private const string ArchivedAtKey = "archived_at";
        private const string CreatedAtKey = "created_at";
        private const string UpdatedAtKey = "updated_at";
        private const string UseSiteExchangeRateKey = "use_site_exchange_rate";
        private const string PricesKey = "prices";
        private const string TaxIncludedKey = "tax_included";
        #endregion

        #region Constructors
        public ComponentPricePointInfo(JsonObject componentPricePointObject)
        {
            if (componentPricePointObject == null) throw new ArgumentNullException(nameof(componentPricePointObject));
            if (componentPricePointObject.Keys.Count <= 0) throw new ArgumentException("Not a valid component price point information object", nameof(componentPricePointObject));
            LoadFromJson(componentPricePointObject);
        }

        private void LoadFromJson(JsonObject obj)
        {
            foreach (string key in obj.Keys)
            {
                switch (key)
                {
                    case IdKey:
                        _id = obj.GetJSONContentAsInt(key);
                        break;
                    case DefaultKey:
                        _default = obj.GetJSONContentAsBoolean(key);
                        break;
                    case NameKey:
                        _name = obj.GetJSONContentAsString(key);
                        break;
                    case TypeKey:
                        _type = obj.GetJSONContentAsString(key);
                        break;
                    case PricingSchemeKey:
                        _pricingScheme = obj.GetJSONContentAsString(key);
                        break;
                    case ComponentIdKey:
                        _componentId = obj.GetJSONContentAsInt(key);
                        break;
                    case HandleKey:
                        _handle = obj.GetJSONContentAsString(key);
                        break;
                    case ArchivedAtKey:
                        _archivedAt = obj.GetJSONContentAsString(key);
                        break;
                    case CreatedAtKey:
                        _createdAt = DateTime.Parse(obj.GetJSONContentAsString(key));
                        break;
                    case UpdatedAtKey:
                        _updatedAt = DateTime.Parse(obj.GetJSONContentAsString(key));
                        break;
                    case UseSiteExchangeRateKey:
                        _useSiteExchangeRate = obj.GetJSONContentAsBoolean(key);
                        break;
                    case PricesKey:
                        _prices = new List<IComponentPricePointPrice>();
                        JsonArray priceArray = obj[key] as JsonArray;
                        if (priceArray != null)
                        {
                            foreach (JsonValue jsonValue in priceArray.Items)
                            {
                                JsonObject price = (JsonObject) jsonValue;
                                _prices.Add(new ComponentPricePointPrice(price));
                            }
                        }

                        // Sanity check, should be equal.
                        if (priceArray != null && priceArray.Length != _prices.Count)
                        {
                            throw new JsonParseException(string.Format("Unable to parse component price point prices ({0} != {1})", priceArray.Length, _prices.Count));
                        }
                        break;
                    case TaxIncludedKey:
                        _taxIncluded = obj.GetJSONContentAsBoolean(key);
                        break;
                }
            }
        }

        private void LoadFromNode(XmlNode priceInfoNode)
        {
            foreach (XmlNode dataNode in priceInfoNode.ChildNodes)
            {
                switch (dataNode.Name)
                {
                    case IdKey:
                        _id = dataNode.GetNodeContentAsInt();
                        break;
                    case DefaultKey:
                        _default = dataNode.GetNodeContentAsBoolean();
                        break;
                    case NameKey:
                        _name = dataNode.GetNodeContentAsString();
                        break;
                    case TypeKey:
                        _type = dataNode.GetNodeContentAsString();
                        break;
                    case PricingSchemeKey:
                        _pricingScheme = dataNode.GetNodeContentAsString();
                        break;
                    case ComponentIdKey:
                        _componentId = dataNode.GetNodeContentAsInt();
                        break;
                    case HandleKey:
                        _handle = dataNode.GetNodeContentAsString();
                        break;
                    case ArchivedAtKey:
                        _archivedAt = dataNode.GetNodeContentAsString();
                        break;
                    case CreatedAtKey:
                        _createdAt = DateTime.Parse(dataNode.GetNodeContentAsString());
                        break;
                    case UpdatedAtKey:
                        _updatedAt = DateTime.Parse(dataNode.GetNodeContentAsString());
                        break;
                    case UseSiteExchangeRateKey:
                        _useSiteExchangeRate = dataNode.GetNodeContentAsBoolean();
                        break;
                    case PricesKey:
                        _prices = new List<IComponentPricePointPrice>();
                        foreach (XmlNode childNode in dataNode.ChildNodes)
                        {
                            switch (childNode.Name)
                            {
                                case "price":
                                    _prices.Add(new ComponentPricePointPrice(childNode));
                                    break;
                            }
                        }
                        break;
                    case TaxIncludedKey:
                        _taxIncluded = dataNode.GetNodeContentAsBoolean();
                        break;
                }
            }
        }
        #endregion

        #region ComponentPricePointInformation Members
        public int Id
        {
            get { return _id; }
        }
        private int _id;

        public bool Default
        {
            get { return _default; }
        }
        private bool _default;

        public string Name
        {
            get { return _name; }
        }
        private string _name;

        public string Type
        {
            get { return _type; }
        }
        private string _type;

        public string PricingScheme
        {
            get { return _pricingScheme; }
        }
        private string _pricingScheme;

        public int ComponentId
        {
            get { return _componentId; }
        }
        private int _componentId;

        public string Handle
        {
            get { return _handle; }
        }
        private string _handle;

        public string ArchivedAt
        {
            get { return _archivedAt; }
        }
        private string _archivedAt;

        public DateTime CreatedAt
        {
            get { return _createdAt; }
        }
        private DateTime _createdAt;

        public DateTime UpdatedAt
        {
            get { return _updatedAt; }
        }
        private DateTime _updatedAt;

        public bool UseSiteExchangeRate
        {
            get { return _useSiteExchangeRate; }
        }
        private bool _useSiteExchangeRate;

        public List<IComponentPricePointPrice> Prices
        {
            get { return _prices; }
        }
        private List<IComponentPricePointPrice> _prices;

        public bool TaxIncluded
        {
            get { return _taxIncluded; }
        }
        private bool _taxIncluded;
        #endregion

        #region IComparable<ComponentPricePointInformation> Members
        public int CompareTo(ComponentPricePointInfo other)
        {
            return Id.CompareTo(other.Id);
        }
        #endregion
    }
}
