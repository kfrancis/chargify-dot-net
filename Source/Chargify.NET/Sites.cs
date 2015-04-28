using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChargifyNET
{
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
