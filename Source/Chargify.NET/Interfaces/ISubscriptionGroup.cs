using ChargifyNET;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChargifyNET
{
    public interface ISubscriptionGroup : IComparable<ISubscriptionGroup>
    {
        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        /// <value>
        /// The customer identifier.
        /// </value>
        int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the payment profile.
        /// </summary>
        /// <value>
        /// The payment profile.
        /// </value>
        IPaymentProfileView PaymentProfile { get; set; }

        /// <summary>
        /// Gets or sets the subscription ids.
        /// </summary>
        /// <value>
        /// The subscription ids.
        /// </value>
        int[] SubscriptionIds { get; set; }

        /// <summary>
        /// Gets or sets the created at.
        /// </summary>
        /// <value>
        /// The created at.
        /// </value>
        string CreatedAt { get; set; }
    }
}
