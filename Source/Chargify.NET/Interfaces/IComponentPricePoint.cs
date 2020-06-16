namespace ChargifyNET
{
    /// <summary>
    /// Component Price Point for components with multiple price points
    /// </summary>
    public interface IComponentPricePoint
    {
        /// <summary>
        /// Gets or sets the component identifier.
        /// </summary>
        /// <value>
        /// The component identifier.
        /// </value>
        int ComponentId { get; set; }

        /// <summary>
        /// Gets or sets the price point.
        /// </summary>
        /// <value>
        /// The price point.
        /// </value>
        string PricePoint { get; set; }
    }
}
