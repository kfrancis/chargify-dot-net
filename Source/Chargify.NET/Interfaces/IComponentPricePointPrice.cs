namespace ChargifyNET
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IComponentPricePointPrice
    {
        public int Id { get; }
        public int ComponentId { get; }
        public int StartingQuantity { get; }
        public int EndingQuantity { get; }
        public decimal UnitPrice { get; }
        public int PricePointId { get; }
        public string FormattedUnitPrice { get; }
    }
}
