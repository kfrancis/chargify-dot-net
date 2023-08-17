namespace ChargifyNET
{
    using System;
    using System.Collections.Generic;

    public interface IComponentPricePointInfo
    {
        IEnumerable<ComponentPricePointCurrencyPrice> CurrencyPrices { get; set; }

        IEnumerable<ComponentPricePointPrice> Prices { get; set; }

        bool TaxIncluded { get; }
    }
}
