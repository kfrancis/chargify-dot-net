namespace ChargifyNET
{
    using System;
    using System.Collections.Generic;

    public interface IComponentPricePointInfo
    {
        string ArchivedAt { get; }
        int ComponentId { get; }
        DateTime CreatedAt { get; }
        bool Default { get; }
        string Handle { get; }
        int Id { get; }
        string Name { get; }
        List<IComponentPricePointPrice> Prices { get; }
        string PricingScheme { get; }
        bool TaxIncluded { get; }
        string Type { get; }
        DateTime UpdatedAt { get; }
        bool UseSiteExchangeRate { get; }
    }
}
