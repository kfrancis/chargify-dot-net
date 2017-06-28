namespace ChargifyNET
{
    /// <summary>
    /// The type of cleanup being performed
    /// </summary>
    public enum SiteCleanupScope
    {
        /// <summary>
        /// Only customer related information will be purged
        /// </summary>
        Customers,

        /// <summary>
        /// All customers, subscriptions, products, families, coupons, components, etc. Everything.
        /// </summary>
        All
    }
}
