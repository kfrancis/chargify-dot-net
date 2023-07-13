using System;
using System.Xml;
using ChargifyNET.Json;

namespace ChargifyNET
{
    public class ComponentPricePointPrice : IComponentPricePointPrice
    {
        #region Field Keys
        private const string IdKey = "id";
        private const string ComponentIdKey = "component_id";
        private const string StartingQuantityKey = "starting_quantity";
        private const string EndingQuantityKey = "ending_quantity";
        private const string UnitPriceKey = "unit_price";
        private const string PricePointIdKey = "price_point_id";
        private const string FormattedUnitPriceKey = "formatted_unit_price";
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pricePointPriceNode">A JsonObject with price point price information</param>
        public ComponentPricePointPrice(XmlNode pricePointPriceNode)
        {
            if (pricePointPriceNode == null) throw new ArgumentNullException(nameof(pricePointPriceNode));
            if (pricePointPriceNode.Name != "price_points") throw new ArgumentException("Not a valid price point price object", nameof(pricePointPriceNode));
            LoadFromNode(pricePointPriceNode);
        }

        public ComponentPricePointPrice(JsonObject pricePointPriceObject)
        {
            if (pricePointPriceObject == null) throw new ArgumentNullException(nameof(pricePointPriceObject));
            if (pricePointPriceObject.Keys.Count <= 0) throw new ArgumentException("Not a valid price point price object", nameof(pricePointPriceObject));
            LoadFromJson(pricePointPriceObject);
        }

        private void LoadFromJson(JsonObject obj)
        {
            foreach (string key in obj.Keys)
            {
                switch (key)
                {
                    case IdKey:
                        _id = obj.GetJSONContentAsInt(IdKey);
                        break;
                    case ComponentIdKey:
                        _componentId = obj.GetJSONContentAsInt(ComponentIdKey);
                        break;
                    case StartingQuantityKey:
                        _startingQuantity = obj.GetJSONContentAsInt(StartingQuantityKey);
                        break;
                    case EndingQuantityKey:
                        _endingQuantity = obj.GetJSONContentAsInt(EndingQuantityKey);
                        break;
                    case UnitPriceKey:
                        _unitPrice = obj.GetJSONContentAsDecimal(UnitPriceKey);
                        break;
                    case PricePointIdKey:
                        _pricePointId = obj.GetJSONContentAsInt(PricePointIdKey);
                        break;
                    case FormattedUnitPriceKey:
                        _formattedUnitPrice = obj.GetJSONContentAsString(FormattedUnitPriceKey);
                        break;
                }
            }
        }

        private void LoadFromNode(XmlNode obj)
        {
            foreach (XmlNode dataNode in obj.ChildNodes)
            {
                switch (dataNode.Name)
                {
                    case IdKey:
                        _id = dataNode.GetNodeContentAsInt();
                        break;
                    case ComponentIdKey:
                        _componentId = dataNode.GetNodeContentAsInt();
                        break;
                    case StartingQuantityKey:
                        _startingQuantity = dataNode.GetNodeContentAsInt();
                        break;
                    case EndingQuantityKey:
                        _endingQuantity = dataNode.GetNodeContentAsInt();
                        break;
                    case UnitPriceKey:
                        _unitPrice = decimal.Parse(dataNode.GetNodeContentAsString());
                        break;
                    case PricePointIdKey:
                        _pricePointId = dataNode.GetNodeContentAsInt();
                        break;
                    case FormattedUnitPriceKey:
                        _formattedUnitPrice = dataNode.GetNodeContentAsString();
                        break;
                }
            }
        }
        #endregion

        #region Properties
        private int _id;
        public int Id
        {
            get { return _id; }
        }

        private int _componentId;
        public int ComponentId
        {
            get { return _componentId; }
        }

        private int _startingQuantity;
        public int StartingQuantity
        {
            get { return _startingQuantity; }
        }

        private int _endingQuantity;
        public int EndingQuantity
        {
            get { return _endingQuantity; }
        }

        private decimal _unitPrice;
        public decimal UnitPrice
        {
            get { return _unitPrice; }
        }

        private int _pricePointId;
        public int PricePointId
        {
            get { return _pricePointId; }
        }

        private string _formattedUnitPrice;
        public string FormattedUnitPrice
        {
            get { return _formattedUnitPrice; }
        }
        #endregion

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
