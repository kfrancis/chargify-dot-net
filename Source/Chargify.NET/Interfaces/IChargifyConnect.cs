#region License, Terms and Conditions
//
// IChargifyConnect.cs
//
// Authors: Kori Francis <twitter.com/djbyter>, David Ball
// Copyright (C) 2010 Clinical Support Systems, Inc. All rights reserved.
// 
//  THIS FILE IS LICENSED UNDER THE MIT LICENSE AS OUTLINED IMMEDIATELY BELOW:
//
//  Permission is hereby granted, free of charge, to any person obtaining a
//  copy of this software and associated documentation files (the "Software"),
//  to deal in the Software without restriction, including without limitation
//  the rights to use, copy, modify, merge, publish, distribute, sublicense,
//  and/or sell copies of the Software, and to permit persons to whom the
//  Software is furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
//  FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
//  DEALINGS IN THE SOFTWARE.
//
#endregion
namespace ChargifyNET
{
    #region Imports
    using System;
    using System.Collections.Generic;
    using System.Net;
    #endregion

    /// <summary>
    /// Wrapper interface
    /// </summary>
    public interface IChargifyConnect
    {
        #region Adjustments
        /// <summary>
        /// Method for applying an adjustment to a subscription
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to adjust</param>
        /// <param name="amount">The amount (in dollars and cents)</param>
        /// <param name="memo">A helpful explaination of the adjustment</param>
        /// <returns>The adjustment object if successful, null otherwise.</returns>
        IAdjustment CreateAdjustment(int SubscriptionID, decimal amount, string memo);
        /// <summary>
        /// Method for applying an adjustment to a subscription
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to adjust</param>
        /// <param name="amount">The amount (in dollars and cents)</param>
        /// <param name="memo">A helpful explaination of the adjustment</param>
        /// <param name="method">A string that toggles how the adjustment should be applied</param>
        /// <returns>The adjustment object if successful, null otherwise.</returns>
        IAdjustment CreateAdjustment(int SubscriptionID, decimal amount, string memo, AdjustmentMethod method);
        /// <summary>
        /// Method for applying an adjustment to a subscription
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to adjust</param>        
        /// <param name="amount_in_cents">The amount (in cents)</param>
        /// <param name="memo">A helpful explaination of the adjustment</param>
        /// <returns>The adjustment object if successful, null otherwise.</returns>
        IAdjustment CreateAdjustment(int SubscriptionID, int amount_in_cents, string memo);
        /// <summary>
        /// Method for applying an adjustment to a subscription
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to adjust</param>        
        /// <param name="amount_in_cents">The amount (in cents)</param>
        /// <param name="memo">A helpful explaination of the adjustment</param>
        /// <param name="method">A string that toggles how the adjustment should be applied</param>
        /// <returns>The adjustment object if successful, null otherwise.</returns>
        IAdjustment CreateAdjustment(int SubscriptionID, int amount_in_cents, string memo, AdjustmentMethod method);
        #endregion

        #region Migrations
        /// <summary>
        /// Method that returns information about what would be done to the subscription if you migrated using these settings
        /// </summary>
        /// <param name="SubscriptionID">The subscription to run this preview against</param>
        /// <param name="ProductID">The product ID to migrate to</param>
        /// <param name="IncludeTrial">Should the trial be included?</param>
        /// <param name="IncludeInitialCharge">Should the initial charge (if applicable) be included?</param>
        /// <param name="IncludeCoupons">Should existing coupons be included?</param>
        /// <returns>Details about the migration, if successful. Null otherwise.</returns>
        IMigration PreviewMigrateSubscriptionProduct(int SubscriptionID, int ProductID, bool? IncludeTrial, bool? IncludeInitialCharge, bool? IncludeCoupons);
        /// <summary>
        /// Method that returns information about what would be done to the subscription if you migrated using these settings
        /// </summary>
        /// <param name="SubscriptionID">The subscription to run this preview against</param>
        /// <param name="ProductID">The product ID to migrate to</param>
        /// <returns>Details about the migration, if successful. Null otherwise.</returns>
        IMigration PreviewMigrateSubscriptionProduct(int SubscriptionID, int ProductID);
        /// <summary>
        /// Method that returns information about what would be done to the subscription if you migrated using these settings
        /// </summary>
        /// <param name="Subscription">The subscription to run this preview against</param>
        /// <param name="Product">The product ID to migrate to</param>
        /// <returns>Details about the migration, if successful. Null otherwise.</returns>
        IMigration PreviewMigrateSubscriptionProduct(ISubscription Subscription, IProduct Product);
        #endregion

        #region Components
        /// <summary>
        /// Method for turning on or off a component
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to modify</param>
        /// <param name="ComponentID">The ID of the component (on/off only) to modify</param>
        /// <param name="SetEnabled">True if wanting to turn the component "on", false otherwise.</param>
        /// <returns>IComponentAttributes object if successful, null otherwise.</returns>
        IComponentAttributes SetComponent(int SubscriptionID, int ComponentID, bool SetEnabled);
        #endregion

        #region Coupons
        /// <summary>
        /// Method to add a coupon to a subscription using the API
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to modify</param>
        /// <param name="CouponCode">The code of the coupon to apply to the subscription</param>
        /// <returns>The subscription details if successful, null otherwise.</returns>
        ISubscription AddCoupon(int SubscriptionID, string CouponCode);
        #endregion

        #region Billing Portal
        /// <summary>
        /// Retrieves the billing portal management link from Chargify. You must have the billing portal feature enabled for this to work.
        /// </summary>
        /// <param name="ChargifyId">The ID of the customer (not reference)</param>
        /// <returns>The billing portal management link and additional information</returns>
        IBillingManagementInfo GetManagementLink(int ChargifyId);
        #endregion

        #region Invoices
        /// <summary>
        /// Gets a list of invoices
        /// </summary>
        /// <returns></returns>
        IDictionary<int, Invoice> GetInvoiceList();
        #endregion

        #region Metadata
        /// <summary>
        /// Returns a list of all metadata for a resource.
        /// </summary>
        /// <typeparam name="T">The type of resource. Currently either Subscription or Customer</typeparam>
        /// <returns>The metadata result containing the response</returns>
        IMetadataResult GetMetadata<T>();

        /// <summary>
        /// Retrieve all metadata for a specific resource (like a specific customer or subscription).
        /// </summary>
        /// <typeparam name="T">The type of resource. Currently either Subscription or Customer</typeparam>
        /// <param name="resourceID">The Chargify identifier for the resource</param>
        /// <param name="page">Which page to return</param>
        /// <returns>The metadata result containing the response</returns>
        IMetadataResult GetMetadataFor<T>(int resourceID, int? page);

        /// <summary>
        /// Allows you to set a group of metadata for a specific resource
        /// </summary>
        /// <typeparam name="T">The type of resource. Currently either Subscription or Customer</typeparam>
        /// <param name="chargifyID">The Chargify identifier for the resource</param>
        /// <param name="metadata">The list of metadata to set</param>
        /// <returns>The metadata result containing the response</returns>
        List<IMetadata> SetMetadataFor<T>(int chargifyID, List<Metadata> metadata);

        /// <summary>
        /// Allows you to set a single metadata for a specific resource
        /// </summary>
        /// <typeparam name="T">The type of resource. Currently either Subscription or Customer</typeparam>
        /// <param name="chargifyID">The Chargify identifier for the resource</param>
        /// <param name="metadata">The list of metadata to set</param>
        /// <returns>The metadata result containing the response</returns>
        List<IMetadata> SetMetadataFor<T>(int chargifyID, Metadata metadata);
        #endregion

        #region Sites
        /// <summary>
        /// Clean up a site in test mode.
        /// </summary>
        /// <param name="CleanupScope">What should be cleaned? DEFAULT IS CUSTOMERS ONLY.</param>
        /// <returns>True if complete, false otherwise</returns>
        /// <remarks>If used against a production site, the result will always be false.</remarks>
        bool ClearTestSite(SiteCleanupScope? CleanupScope = SiteCleanupScope.Customers);
        #endregion

        #region Payments

        /// <summary>
        /// Chargify allows you to record payments that occur outside of the normal flow of payment processing.
        /// These payments are considered external payments.A common case to apply such a payment is when a 
        /// customer pays by check or some other means for their subscription.
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to apply this manual payment record to</param>
        /// <param name="Amount">The decimal amount of the payment (ie. 10.00 for $10)</param>
        /// <param name="Memo">The memo to include with the manual payment</param>
        /// <returns>The payment result, null otherwise.</returns>
        IPayment AddPayment(int SubscriptionID, decimal Amount, string Memo);

        /// <summary>
        /// Chargify allows you to record payments that occur outside of the normal flow of payment processing.
        /// These payments are considered external payments.A common case to apply such a payment is when a 
        /// customer pays by check or some other means for their subscription.
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to apply this manual payment record to</param>
        /// <param name="AmountInCents">The amount in cents of the payment (ie. $10 would be 1000 cents)</param>
        /// <param name="Memo">The memo to include with the manual payment</param>
        /// <returns>The payment result, null otherwise.</returns>
        IPayment AddPayment(int SubscriptionID, int AmountInCents, string Memo);
        #endregion

        #region Subscription Override
        /// <summary>
        /// This API endpoint allows you to set certain subscription fields that are usually managed for you automatically. Some of the fields can be set via the normal Subscriptions Update API, but others can only be set using this endpoint.
        /// </summary>
        /// <param name="SubscriptionID"></param>
        /// <param name="OverrideDetails"></param>
        /// <returns>The details returned by Chargify</returns>
        bool SetSubscriptionOverride(int SubscriptionID, ISubscriptionOverride OverrideDetails);
        /// <summary>
        /// This API endpoint allows you to set certain subscription fields that are usually managed for you automatically. Some of the fields can be set via the normal Subscriptions Update API, but others can only be set using this endpoint.
        /// </summary>
        /// <param name="SubscriptionID"></param>
        /// <param name="ActivatedAt"></param>
        /// <param name="CanceledAt"></param>
        /// <param name="CancellationMessage"></param>
        /// <param name="ExpiresAt"></param>
        /// <returns></returns>
        bool SetSubscriptionOverride(int SubscriptionID, DateTime? ActivatedAt = null, DateTime? CanceledAt = null, string CancellationMessage = null, DateTime? ExpiresAt = null);
        #endregion

        /// <summary>
        /// Method for adding a metered component usage to the subscription
        /// </summary>
        /// <param name="SubscriptionID">The subscriptionID to modify</param>
        /// <param name="ComponentID">The ID of the component (metered or quantity) to add a usage of</param>
        /// <param name="Quantity">The number of usages to add</param>
        /// <param name="Memo">The memo for the usage</param>
        /// <returns>The usage added if successful, otherwise null.</returns>
        IUsage AddUsage(int SubscriptionID, int ComponentID, int Quantity, string Memo);
        /// <summary>
        /// Get or set the API key
        /// </summary>
        string apiKey { get; set; }
        /// <summary>
        /// Method to create a new product and add it to the site
        /// </summary>
        /// <param name="ProductFamilyID">The product family to which this new product should be added</param>
        /// <param name="NewProduct">The new product details</param>
        /// <returns>The completed product information</returns>
        IProduct CreateProduct(int ProductFamilyID, IProduct NewProduct);
        /// <summary>
        /// Allows the creation of a product
        /// </summary>
        /// <param name="ProductFamilyID">The family to which this product belongs</param>
        /// <param name="Name">The name of the product</param>
        /// <param name="Handle">The handle to be used for this product</param>
        /// <param name="PriceInCents">The price (in cents)</param>
        /// <param name="Interval">The time interval used to determine the recurring nature of this product</param>
        /// <param name="IntervalUnit">Either days, or months</param>
        /// <param name="AccountingCode">The accounting code used for this product</param>
        /// <param name="Description">The product description</param>
        /// <returns>The created product</returns>
        IProduct CreateProduct(int ProductFamilyID, string Name, string Handle, int PriceInCents, int Interval, IntervalUnit IntervalUnit, string AccountingCode, string Description);
        /// <summary>
        /// Method for creating a new product family via the API
        /// </summary>
        /// <param name="newFamily">The new product family details</param>
        /// <returns>The created product family information</returns>
        IProductFamily CreateProductFamily(IProductFamily newFamily);
        /// <summary>
        /// Get a list of product families
        /// </summary>
        /// <returns>A list of product families (keyed by product family id)</returns>
        IDictionary<int, IProductFamily> GetProductFamilyList();
        /// <summary>
        /// Load the requested product family from chargify by its handle
        /// </summary>
        /// <param name="Handle">The Chargify ID or handle of the product</param>
        /// <returns>The product family with the specified chargify ID</returns>
        IProductFamily LoadProductFamily(string Handle);
        /// <summary>
        /// Load the requested product family from chargify by its handle
        /// </summary>
        /// <param name="ID">The Chargify ID of the product</param>
        /// <returns>The product family with the specified chargify ID</returns>
        IProductFamily LoadProductFamily(int ID);
        /// <summary>
        /// Create a new one-time charge
        /// </summary>
        /// <param name="SubscriptionID">The subscription that will be charged</param>
        /// <param name="Charge">The charge parameters</param>
        /// <returns></returns>
        ICharge CreateCharge(int SubscriptionID, ICharge Charge);
        ///// <summary>
        ///// Create a new one-time charge
        ///// </summary>
        ///// <param name="SubscriptionID">The subscription that will be charged</param>
        ///// <param name="amount">The amount to charge the customer</param>
        ///// <param name="memo">A description of the charge</param>
        ///// <returns>The charge details</returns>
        //ICharge CreateCharge(int SubscriptionID, decimal amount, string memo);
        /// <summary>
        /// Create a new one-time charge
        /// </summary>
        /// <param name="SubscriptionID">The subscription that will be charged</param>
        /// <param name="amount">The amount to charge the customer</param>
        /// <param name="memo">A description of the charge</param>
        /// <param name="delayCharge">(Optional) Should the charge be billed during the next assessment? Default = false</param>
        /// <param name="useNegativeBalance">(Optional) Should the subscription balance be taken into consideration? Default = false</param>
        /// <returns>The charge details</returns>
        ICharge CreateCharge(int SubscriptionID, decimal amount, string memo, bool useNegativeBalance = false, bool delayCharge = false);
        /// <summary>
        /// Create a new one-time credit
        /// </summary>
        /// <param name="Coupon">The coupon parameters</param>
        /// <param name="ProductFamilyID">The ID of the product family to add this coupon to.</param>
        /// <returns>The object if successful, null otherwise.</returns>
        ICoupon CreateCoupon(ICoupon Coupon, int ProductFamilyID);
        /// <summary>
        /// Update an existing coupon
        /// </summary>
        /// <param name="Coupon">Coupon object</param>
        /// <returns>The updated coupon if successful, null otherwise.</returns>
        ICoupon UpdateCoupon(ICoupon Coupon);
        /// <summary>
        /// Create a new one-time credit
        /// </summary>
        /// <param name="SubscriptionID">The subscription that will be credited</param>
        /// <param name="Credit">The credit parameters</param>
        /// <returns>The object if successful, null otherwise.</returns>
        ICredit CreateCredit(int SubscriptionID, ICredit Credit);
        /// <summary>
        /// Create a new one-time credit
        /// </summary>
        /// <param name="SubscriptionID">The subscription that will be credited</param>
        /// <param name="amount">The amount to credit the customer</param>
        /// <param name="memo">A note regarding the reason for the credit</param>
        /// <returns>The object if successful, null otherwise.</returns>
        ICredit CreateCredit(int SubscriptionID, decimal amount, string memo);

        #region Customer
        /// <summary>
        /// Create a new chargify customer
        /// </summary>
        /// <param name="Customer">
        /// A customer object containing customer attributes.  The customer cannot be an existing saved chargify customer
        /// </param>
        /// <returns>The created chargify customer</returns>
        ICustomer CreateCustomer(ICustomer Customer);
        /// <summary>
        /// Create a new chargify customer
        /// </summary>
        /// <param name="FirstName">The first name of the customer</param>
        /// <param name="LastName">The last name of the customer</param>
        /// <param name="EmailAddress">The email address of the customer</param>
        /// <param name="Phone">The customers phone number</param>
        /// <param name="Organization">The organization of the customer</param>
        /// <param name="SystemID">The system ID for the customer</param>
        /// <returns>The created chargify customer</returns>
        ICustomer CreateCustomer(string FirstName, string LastName, string EmailAddress, string Phone, string Organization, string SystemID);
        /// <summary>
        /// Delete the specified customer
        /// </summary>
        /// <param name="ChargifyID">The integer identifier of the customer</param>
        /// <returns>True if the customer was deleted, false otherwise.</returns>
        /// <remarks>This method does not currently work, but it will once they open up the API. This will always return false, as Chargify will send a Http Forbidden everytime.</remarks>
        bool DeleteCustomer(int ChargifyID);
        /// <summary>
        /// Delete the specified customer
        /// </summary>
        /// <param name="SystemID">The system identifier of the customer.</param>
        /// <returns>True if the customer was deleted, false otherwise.</returns>
        /// <remarks>This method does not currently work, but it will once they open up the API. This will always return false, as Chargify will send a Http Forbidden everytime.</remarks>
        bool DeleteCustomer(string SystemID);
        #endregion

        #region Subscriptions
        /// <summary>
        /// Create a new subscription and a new customer at the same time without submitting PaymentProfile attributes
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="CustomerAttributes">The attributes for the new customer</param>
        /// <param name="PaymentCollectionMethod">The payment collection method to use, automatic by default</param>
        /// <returns>The xml describing the new subsscription</returns>
        ISubscription CreateSubscription(string ProductHandle, ICustomerAttributes CustomerAttributes, PaymentCollectionMethod? PaymentCollectionMethod = PaymentCollectionMethod.Automatic);
        /// <summary>
        /// Create a new subscription and a new customer at the same time and import the card data from a specific vault storage
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="CustomerAttributes">The attributes for the new customer</param>
        /// <param name="NextBillingAt">DateTime for this customer to be assessed at</param>
        /// <param name="ExistingProfile">Data concerning the existing profile in vault storage</param>
        /// <returns>The xml describing the new subscription</returns>
        ISubscription CreateSubscription(string ProductHandle, ICustomerAttributes CustomerAttributes, DateTime NextBillingAt, IPaymentProfileAttributes ExistingProfile);
        /// <summary>
        /// Create a new subscription and a new customer at the same time and use the card data from another payment profile (from the same customer).
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="CustomerAttributes">The attributes for the new customer</param>
        /// <param name="NextBillingAt">DateTime for this customer to be assessed at</param>
        /// <param name="ExistingProfileID">The ID of the existing payment profile to use when creating the new subscription.</param>
        /// <returns>The new subscription</returns>
        ISubscription CreateSubscription(string ProductHandle, ICustomerAttributes CustomerAttributes, DateTime NextBillingAt, int ExistingProfileID);
        /// <summary>
        /// Create a new subscription and a new customer at the same time
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="CustomerAttributes">The attributes for the new customer</param>
        /// <param name="CreditCardAttributes">The credit card attributes</param>
        /// <returns>The xml describing the new subsscription</returns>
        ISubscription CreateSubscription(string ProductHandle, ICustomerAttributes CustomerAttributes, ICreditCardAttributes CreditCardAttributes);
        /// <summary>
        /// Create a new subscription and a new customer at the same time
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="CustomerAttributes">The attributes for the new customer</param>
        /// <param name="CreditCardAttributes">The credit card attributes</param>
        /// /// <param name="NextBillingAt">DateTime for this customer to be assessed at</param>
        /// <returns>The xml describing the new subscription</returns>
        ISubscription CreateSubscription(string ProductHandle, ICustomerAttributes CustomerAttributes, ICreditCardAttributes CreditCardAttributes, DateTime NextBillingAt);
        /// <summary>
        /// Create a new subscription and a new customer at the same time
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="CustomerAttributes">The attributes for the new customer</param>
        /// <param name="CreditCardAttributes">The credit card attributes</param>
        /// <param name="AllocatedQuantity">The component allocation to send when creating the subscription</param>
        /// <param name="ComponentID">The component to reference when creating the subscription</param>
        /// <returns>The xml describing the new subsscription</returns>
        ISubscription CreateSubscription(string ProductHandle, ICustomerAttributes CustomerAttributes, ICreditCardAttributes CreditCardAttributes, int ComponentID, int AllocatedQuantity);
        /// <summary>
        /// Create a new subscription and a new customer at the same time
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="CustomerAttributes">The attributes for the new customer</param>
        /// <param name="CreditCardAttributes">The credit card attributes</param>
        /// <param name="ComponentsWithQuantity">The list of components and quantities to use</param>
        /// <returns>The xml describing the new subsscription</returns>
        ISubscription CreateSubscription(string ProductHandle, ICustomerAttributes CustomerAttributes, ICreditCardAttributes CreditCardAttributes, Dictionary<int, string> ComponentsWithQuantity);
        /// <summary>
        /// Create a new subscription without passing credit card information.
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="ChargifyID">The Chargify ID of the customer</param>
        /// <param name="PaymentCollectionMethod">Optional, type of payment collection method</param>
        /// <returns>The xml describing the new subsscription</returns>
        ISubscription CreateSubscription(string ProductHandle, int ChargifyID, PaymentCollectionMethod? PaymentCollectionMethod = PaymentCollectionMethod.Automatic);
        /// <summary>
        /// Create a new subscription 
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="ChargifyID">The Chargify ID of the customer</param>
        /// <param name="CreditCardAttributes">The credit card attributes</param>
        /// <returns>The xml describing the new subsscription</returns>
        ISubscription CreateSubscription(string ProductHandle, int ChargifyID, ICreditCardAttributes CreditCardAttributes);
        /// <summary>
        /// Create a subscription using a coupon for discounted rate, without using credit card information.
        /// </summary>
        /// <param name="ProductHandle">The product to subscribe to</param>
        /// <param name="ChargifyID">The ID of the Customer to add the subscription for</param>
        /// <param name="CouponCode">The discount coupon code</param>
        /// <param name="paymentCollectionMethod">Optional, type of payment collection method</param>
        /// <returns>If sucessful, the subscription object. Otherwise null.</returns>
        ISubscription CreateSubscription(string ProductHandle, int ChargifyID, string CouponCode, PaymentCollectionMethod? paymentCollectionMethod);

        /// <summary>
        /// Create a new subscription without requiring credit card information
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="SystemID">The System ID of the customer</param>
        /// <returns>The xml describing the new subsscription</returns>
        ISubscription CreateSubscription(string ProductHandle, string SystemID);
        /// <summary>
        /// Create a new subscription 
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="SystemID">The System ID of the customer</param>
        /// <param name="CreditCardAttributes">The credit card attributes</param>
        /// <returns>The xml describing the new subsscription</returns>
        ISubscription CreateSubscription(string ProductHandle, string SystemID, ICreditCardAttributes CreditCardAttributes);
        /// <summary>
        /// Create a new subscription and a new customer at the same time
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="CustomerAttributes">The attributes for the new customer</param>
        /// <param name="CreditCardAttributes">The credit card attributes</param>
        /// <param name="CouponCode">The discount coupon code</param>
        /// <returns>The xml describing the new subsscription</returns>
        ISubscription CreateSubscriptionUsingCoupon(string ProductHandle, ICustomerAttributes CustomerAttributes, ICreditCardAttributes CreditCardAttributes, string CouponCode);
        /// <summary>
        /// Create a new subscription and a new customer at the same time
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="CustomerAttributes">The attributes for the new customer</param>
        /// <param name="CreditCardAttributes">The credit card attributes</param>
        /// <param name="CouponCode">The discount coupon code</param>
        /// <param name="NextBillingAt">Specify the time of first assessment</param>
        /// <returns>The new subscription object</returns>
        ISubscription CreateSubscriptionUsingCoupon(string ProductHandle, ICustomerAttributes CustomerAttributes, ICreditCardAttributes CreditCardAttributes, DateTime NextBillingAt, string CouponCode);
        /// <summary>
        /// Create a new subscription and a new customer at the same time
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="CustomerAttributes">The attributes for the new customer</param>
        /// <param name="CreditCardAttributes">The credit card attributes</param>
        /// <param name="CouponCode">The discount coupon code</param>
        /// <param name="ComponentID">The component to allocate when creating the subscription</param>
        /// <param name="AllocatedQuantity">The quantity to allocate of the component when creating the subscription</param>
        /// <returns>The xml describing the new subsscription</returns>
        ISubscription CreateSubscriptionUsingCoupon(string ProductHandle, ICustomerAttributes CustomerAttributes, ICreditCardAttributes CreditCardAttributes, string CouponCode, int ComponentID, int AllocatedQuantity);
        /// <summary>
        /// Create a new subscription and a new customer at the same time
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="CustomerAttributes">The attributes for the new customer</param>
        /// <param name="CreditCardAttributes">The credit card attributes</param>
        /// <param name="CouponCode">The discount coupon code</param>
        /// <param name="ComponentsWithQuantity">The list of components and quantities to use</param>
        /// <returns>The xml describing the new subsscription</returns>
        ISubscription CreateSubscriptionUsingCoupon(string ProductHandle, ICustomerAttributes CustomerAttributes, ICreditCardAttributes CreditCardAttributes, string CouponCode, Dictionary<int, string> ComponentsWithQuantity);
        /// <summary>
        /// Create a new subscription and a new customer at the same time using no credit card information
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="CustomerAttributes">The attributes for the new customer</param>
        /// <param name="CouponCode">The discount coupon code</param>
        /// <returns>The xml describing the new subsscription</returns>
        ISubscription CreateSubscriptionUsingCoupon(string ProductHandle, ICustomerAttributes CustomerAttributes, string CouponCode);
        /// <summary>
        /// Create a new subscription and a new customer at the same time using no credit card information
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="CustomerAttributes">The attributes for the new customer</param>
        /// <param name="CouponCode">The discount coupon code</param>
        /// <param name="NextBillingAt">DateTime for this customer to be assessed at</param>
        /// <param name="ExistingProfile">Data concerning the existing profile in vault storage</param>
        /// <returns>The xml describing the new subsscription</returns>
        ISubscription CreateSubscriptionUsingCoupon(string ProductHandle, ICustomerAttributes CustomerAttributes, string CouponCode, DateTime NextBillingAt, IPaymentProfileAttributes ExistingProfile);
        /// <summary>
        /// Create a new subscription and a new customer at the same time using existing credit card information (from the same customer)
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="CustomerAttributes">The attributes for the new customer</param>
        /// <param name="CouponCode">The discount coupon code</param>
        /// <param name="NextBillingAt">DateTime for this customer to be assessed at</param>
        /// <param name="ExistingProfileID">The ID of the existing payment profile to use when creating the new subscription.</param>
        /// <returns>The new subsscription</returns>
        ISubscription CreateSubscriptionUsingCoupon(string ProductHandle, ICustomerAttributes CustomerAttributes, string CouponCode, DateTime NextBillingAt, int ExistingProfileID);
        /// <summary>
        /// Create a subscription using a coupon for discounted rate
        /// </summary>
        /// <param name="ProductHandle">The product to subscribe to</param>
        /// <param name="ChargifyID">The ID of the Customer to add the subscription for</param>
        /// <param name="CreditCardAttributes">The credit card attributes to use for this transaction</param>
        /// <param name="CouponCode">The discount coupon code</param>
        /// <returns></returns>
        ISubscription CreateSubscriptionUsingCoupon(string ProductHandle, int ChargifyID, ICreditCardAttributes CreditCardAttributes, string CouponCode);
        /// <summary>
        /// Create a new subscription 
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="ChargifyID">The Chargify ID of the customer</param>
        /// <param name="CouponCode">The discount coupon code</param>
        /// <returns>The xml describing the new subsscription</returns>
        ISubscription CreateSubscriptionUsingCoupon(string ProductHandle, int ChargifyID, string CouponCode);
        /// <summary>
        /// Create a new subscription 
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="SystemID">The System ID of the customer</param>
        /// <param name="CreditCardAttributes">The credit card attributes</param>
        /// <param name="CouponCode">The discount coupon code</param>
        /// <returns>The xml describing the new subsscription</returns>
        ISubscription CreateSubscriptionUsingCoupon(string ProductHandle, string SystemID, ICreditCardAttributes CreditCardAttributes, string CouponCode);
        /// <summary>
        /// Create a new subscription without passing credit card info
        /// </summary>
        /// <param name="ProductHandle">The handle to the product</param>
        /// <param name="SystemID">The System ID of the customer</param>
        /// <param name="CouponCode">The discount coupon code</param>
        /// <returns>The xml describing the new subscription</returns>
        ISubscription CreateSubscriptionUsingCoupon(string ProductHandle, string SystemID, string CouponCode);
        #endregion

        /// <summary>
        /// Delete a subscription
        /// </summary>
        /// <param name="SubscriptionID">The ID of the sucscription</param>
        /// <param name="CancellationMessage">The message to associate with the subscription</param>
        /// <returns></returns>
        bool DeleteSubscription(int SubscriptionID, string CancellationMessage);
        /// <summary>
        /// Update the product information for an existing subscription
        /// </summary>
        /// <remarks>Does NOT prorate. Use MigrateSubscriptionProduct to get proration to work.</remarks>
        /// <param name="Subscription">The suscription to update</param>
        /// <param name="Product">The new product</param>
        /// <returns>The new subscription resulting from the change</returns>
        ISubscription EditSubscriptionProduct(ISubscription Subscription, IProduct Product);
        /// <summary>
        /// Update the product information for an existing subscription
        /// </summary>
        /// <remarks>Does NOT prorate. Use MigrateSubscriptionProduct to get proration to work.</remarks>
        /// <param name="Subscription">The suscription to update</param>
        /// <param name="ProductHandle">The handle to the new product</param>
        /// <returns>The new subscription resulting from the change</returns>
        ISubscription EditSubscriptionProduct(ISubscription Subscription, string ProductHandle);
        /// <summary>
        /// Update the product information for an existing subscription
        /// </summary>
        /// <remarks>Does NOT prorate. Use MigrateSubscriptionProduct to get proration to work.</remarks>
        /// <param name="SubscriptionID">The ID of the suscription to update</param>
        /// <param name="Product">The new product</param>
        /// <returns>The new subscription resulting from the change</returns>
        ISubscription EditSubscriptionProduct(int SubscriptionID, IProduct Product);
        /// <summary>
        /// Update the product information for an existing subscription
        /// </summary>
        /// <remarks>Does NOT prorate. Use MigrateSubscriptionProduct to get proration to work.</remarks>
        /// <param name="SubscriptionID">The ID of the suscription to update</param>
        /// <param name="ProductHandle">The handle to the new product</param>
        /// <returns>The new subscription resulting from the change</returns>
        ISubscription EditSubscriptionProduct(int SubscriptionID, string ProductHandle);
        /// <summary>
        /// Method to allow users to change the next_assessment_at date
        /// </summary>
        /// <param name="SubscriptionID">The subscription to modify</param>
        /// <param name="NextBillingAt">The date to next bill the customer</param>
        /// <returns>Subscription if successful, null otherwise.</returns>
        ISubscription UpdateBillingDateForSubscription(int SubscriptionID, DateTime NextBillingAt);
        /// <summary>
        /// Method to allow users to change the cancel_at_end_of_period flag
        /// </summary>
        /// <param name="SubscriptionID">The subscription to modify</param>
        /// <param name="CancelAtEndOfPeriod">True if the subscription should cancel at the end of the current period</param>
        /// <param name="CancellationMessage">The reason for cancelling.</param>
        /// <returns>Subscription if successful, null otherwise.</returns>
        ISubscription UpdateDelayedCancelForSubscription(int SubscriptionID, bool CancelAtEndOfPeriod, string CancellationMessage);
        /// <summary>
        /// Retrieve the coupon corresponding to the coupon code, useful for coupon validation.
        /// </summary>
        /// <param name="ProductFamilyID">The ID of the product family the coupon belongs to</param>
        /// <param name="CouponCode">The code used to represent the coupon</param>
        /// <returns>The object if found, otherwise null.</returns>
        ICoupon FindCoupon(int ProductFamilyID, string CouponCode);
        /// <summary>
        /// Method for getting a list of component usages for a specific subscription
        /// </summary>
        /// <param name="SubscriptionID">The subscription ID to examine</param>
        /// <param name="ComponentID">The ID of the component to examine</param>
        /// <returns>A dictionary of usages if there are results, null otherwise.</returns>
        IDictionary<string, IComponent> GetComponentList(int SubscriptionID, int ComponentID);
        /// <summary>
        /// Method for getting a list of components for a specific product family
        /// </summary>
        /// <param name="ChargifyID">The product family ID</param>
        /// <returns>A dictionary of components if there are results, null otherwise.</returns>
        IDictionary<int, IComponentInfo> GetComponentsForProductFamily(int ChargifyID);
        /// <summary>
        /// Method for getting a list of components for a specific product family
        /// </summary>
        /// <param name="ChargifyID">The product family ID</param>
        /// <param name="includeArchived">Filter flag for archived components</param>
        /// <returns>A dictionary of components if there are results, null otherwise.</returns>
        IDictionary<int, IComponentInfo> GetComponentsForProductFamily(int ChargifyID, bool includeArchived);
        /// <summary>
        /// Returns all components "attached" to that subscription.
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to query about</param>
        /// <returns>A dictionary of components, if applicable.</returns>
        IDictionary<int, IComponentAttributes> GetComponentsForSubscription(int SubscriptionID);
        /// <summary>
        /// Get a list of all customers.  Be careful calling this method because a large number of
        /// customers will result in multiple calls to Chargify
        /// </summary>
        /// <returns>A list of customers</returns>
        IDictionary<string, ICustomer> GetCustomerList();
        /// <summary>
        /// Get a list of all customers.  Be careful calling this method because a large number of
        /// customers will result in multiple calls to Chargify
        /// </summary>
        /// <param name="keyByChargifyID">If true, the key will be the ChargifyID, otherwise it will be the reference value</param>
        /// <returns>A list of customers</returns>
        IDictionary<string, ICustomer> GetCustomerList(bool keyByChargifyID);
        /// <summary>
        /// Get a list of customers (will return 50 for each page)
        /// </summary>
        /// <param name="PageNumber">The page number to load</param>
        /// <returns>A list of customers for the specified page</returns>
        IDictionary<string, ICustomer> GetCustomerList(int PageNumber);
        /// <summary>
        /// Get a list of customers (will return 50 for each page)
        /// </summary>
        /// <param name="PageNumber">The page number to load</param>
        /// /// <param name="keyByChargifyID">If true, the key will be the ChargifyID, otherwise it will be the reference value</param>
        /// <returns>A list of customers for the specified page</returns>
        IDictionary<string, ICustomer> GetCustomerList(int PageNumber, bool keyByChargifyID);
        /// <summary>
        /// Method to get the secure URL (with pretty id) for updating the payment details for a subscription.
        /// </summary>
        /// <param name="FirstName">The first name of the customer to add to the pretty url</param>
        /// <param name="LastName">The last name of the customer to add to the pretty url</param>
        /// <param name="SubscriptionID">The ID of the subscription to update</param>
        /// <returns>The secure url of the update page</returns>
        string GetPrettySubscriptionUpdateURL(string FirstName, string LastName, int SubscriptionID);
        /// <summary>
        /// Get a list of products
        /// </summary>
        /// <returns>A list of products (keyed by product handle)</returns>
        IDictionary<int, IProduct> GetProductList();
        /// <summary>
        /// Get a list of all subscriptions for a customer.
        /// </summary>
        /// <param name="ChargifyID">The ChargifyID of the customer</param>
        /// <returns>A list of subscriptions</returns>
        IDictionary<int, ISubscription> GetSubscriptionListForCustomer(int ChargifyID);
        /// <summary>
        /// Method to get the secure URL for updating the payment details for a subscription.
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to update</param>
        /// <returns>The secure url of the update page</returns>
        string GetSubscriptionUpdateURL(int SubscriptionID);
        /// <summary>
        /// Method for getting a list of transactions
        /// </summary>
        /// <returns>The dictionary of transaction records if successful, otherwise null.</returns>
        IDictionary<int, ITransaction> GetTransactionList();
        /// <summary>
        /// Method for getting a list of transactions
        /// </summary>
        /// <param name="kinds">A list of the types of transactions to return.</param>
        /// <returns>The dictionary of transaction records if successful, otherwise null.</returns>
        IDictionary<int, ITransaction> GetTransactionList(List<TransactionType> kinds);
        /// <summary>
        /// Method for getting a list of transactions
        /// </summary>
        /// <param name="kinds">A list of transaction types to return.</param>
        /// <param name="since_id">Returns transactions with an ID greater than or equal to the one specified. Use int.MinValue to not specify a since_id.</param>
        /// <param name="max_id">Returns transactions with an id less than or equal to the one specified. Use int.MinValue to not specify a max_id.</param>
        /// <returns>The dictionary of transaction records if successful, otherwise null.</returns>
        IDictionary<int, ITransaction> GetTransactionList(List<TransactionType> kinds, int since_id, int max_id);
        /// <summary>
        /// Method for getting a list of transactions
        /// </summary>
        /// <param name="kinds">A list of transaction types to return.</param>
        /// <param name="since_id">Returns transactions with an ID greater than or equal to the one specified. Use int.MinValue to not specify a since_id.</param>
        /// <param name="max_id">Returns transactions with an id less than or equal to the one specified. Use int.MinValue to not specify a max_id.</param>
        /// <param name="since_date">Returns transactions with a created_at date greater than or equal to the one specified. Use DateTime.MinValue to not specify a since_date.</param>
        /// <param name="until_date">Returns transactions with a created_at date less than or equal to the one specified. Use DateTime.MinValue to not specify an until_date.</param>
        /// <returns>The dictionary of transaction records if successful, otherwise null.</returns>
        IDictionary<int, ITransaction> GetTransactionList(List<TransactionType> kinds, int since_id, int max_id, DateTime since_date, DateTime until_date);
        /// <summary>
        /// Method for getting a list of transactions
        /// </summary>
        /// <param name="page">The page number</param>
        /// <param name="per_page">The number of results per page (used for pagination)</param>
        /// <returns>The dictionary of transaction records if successful, otherwise null.</returns>
        IDictionary<int, ITransaction> GetTransactionList(int page, int per_page);
        /// <summary>
        /// Method for getting a list of transactions
        /// </summary>
        /// <param name="page">The page number</param>
        /// <param name="per_page">The number of results per page (used for pagination)</param>
        /// <param name="kinds">A list of the types of transactions to return.</param>
        /// <returns>The dictionary of transaction records if successful, otherwise null.</returns>
        IDictionary<int, ITransaction> GetTransactionList(int page, int per_page, List<TransactionType> kinds);
        /// <summary>
        /// Method for getting a list of transactions
        /// </summary>
        /// <param name="page">The page number</param>
        /// <param name="per_page">The number of results per page (used for pagination)</param>
        /// <param name="kinds">A list of transaction types to return.</param>
        /// <param name="since_id">Returns transactions with an ID greater than or equal to the one specified. Use int.MinValue to not specify a since_id.</param>
        /// <param name="max_id">Returns transactions with an id less than or equal to the one specified. Use int.MinValue to not specify a max_id.</param>
        /// <returns>The dictionary of transaction records if successful, otherwise null.</returns>
        IDictionary<int, ITransaction> GetTransactionList(int page, int per_page, List<TransactionType> kinds, int since_id, int max_id);
        /// <summary>
        /// Method for getting a list of transactions
        /// </summary>
        /// <param name="page">The page number</param>
        /// <param name="per_page">The number of results per page (used for pagination)</param>
        /// <param name="kinds">A list of transaction types to return.</param>
        /// <param name="since_id">Returns transactions with an ID greater than or equal to the one specified. Use int.MinValue to not specify a since_id.</param>
        /// <param name="max_id">Returns transactions with an id less than or equal to the one specified. Use int.MinValue to not specify a max_id.</param>
        /// <param name="since_date">Returns transactions with a created_at date greater than or equal to the one specified. Use DateTime.MinValue to not specify a since_date.</param>
        /// <param name="until_date">Returns transactions with a created_at date less than or equal to the one specified. Use DateTime.MinValue to not specify an until_date.</param>
        /// <returns>The dictionary of transaction records if successful, otherwise null.</returns>
        IDictionary<int, ITransaction> GetTransactionList(int page, int per_page, List<TransactionType> kinds, int since_id, int max_id, DateTime since_date, DateTime until_date);
        /// <summary>
        /// Method for getting the list of transactions for a subscription
        /// </summary>
        /// <param name="SubscriptionID">The subscriptionID to get a list of transactions for</param>
        /// <returns>A dictionary of transactions if successful, otherwise null.</returns>
        IDictionary<int, ITransaction> GetTransactionsForSubscription(int SubscriptionID);
        /// <summary>
        /// Method for getting the list of transactions for a subscription
        /// </summary>
        /// <param name="SubscriptionID">The subscriptionID to get a list of transactions for</param>
        /// <param name="kinds">A list of the types of transactions to return.</param>
        /// <returns>A dictionary of transactions if successful, otherwise null.</returns>
        IDictionary<int, ITransaction> GetTransactionsForSubscription(int SubscriptionID, List<TransactionType> kinds);
        /// <summary>
        /// Method for getting the list of transactions for a subscription
        /// </summary>
        /// <param name="SubscriptionID">The subscriptionID to get a list of transactions for</param>
        /// <param name="kinds">A list of the types of transactions to return.</param>
        /// <param name="since_id">Returns transactions with an ID greater than or equal to the one specified. Use int.MinValue to not specify a since_id.</param>
        /// <param name="max_id">Returns transactions with an id less than or equal to the one specified. Use int.MinValue to not specify a max_id.</param>
        /// <returns>A dictionary of transactions if successful, otherwise null.</returns>
        IDictionary<int, ITransaction> GetTransactionsForSubscription(int SubscriptionID, List<TransactionType> kinds, int since_id, int max_id);
        /// <summary>
        /// Method for getting the list of transactions for a subscription
        /// </summary>
        /// <param name="SubscriptionID">The subscriptionID to get a list of transactions for</param>
        /// <param name="kinds">A list of the types of transactions to return.</param>
        /// <param name="since_id">Returns transactions with an ID greater than or equal to the one specified. Use int.MinValue to not specify a since_id.</param>
        /// <param name="max_id">Returns transactions with an id less than or equal to the one specified. Use int.MinValue to not specify a max_id.</param>
        /// <param name="since_date">Returns transactions with a created_at date greater than or equal to the one specified. Use DateTime.MinValue to not specify a since_date.</param>
        /// <param name="until_date">Returns transactions with a created_at date less than or equal to the one specified. Use DateTime.MinValue to not specify an until_date.</param>
        /// <returns>A dictionary of transactions if successful, otherwise null.</returns>
        IDictionary<int, ITransaction> GetTransactionsForSubscription(int SubscriptionID, List<TransactionType> kinds, int since_id, int max_id, DateTime since_date, DateTime until_date);
        /// <summary>
        /// Method for getting the list of transactions for a subscription
        /// </summary>
        /// <param name="SubscriptionID">The subscriptionID to get a list of transactions for</param>
        /// <param name="page">The page number</param>
        /// <param name="per_page">The number of results per page (used for pagination)</param>
        /// <returns>A dictionary of transactions if successful, otherwise null.</returns>
        IDictionary<int, ITransaction> GetTransactionsForSubscription(int SubscriptionID, int page, int per_page);
        /// <summary>
        /// Method for getting the list of transactions for a subscription
        /// </summary>
        /// <param name="SubscriptionID">The subscriptionID to get a list of transactions for</param>
        /// <param name="page">The page number</param>
        /// <param name="per_page">The number of results per page (used for pagination)</param>
        /// <param name="kinds">A list of the types of transactions to return.</param>
        /// <returns>A dictionary of transactions if successful, otherwise null.</returns>
        IDictionary<int, ITransaction> GetTransactionsForSubscription(int SubscriptionID, int page, int per_page, List<TransactionType> kinds);
        /// <summary>
        /// Method for getting the list of transactions for a subscription
        /// </summary>
        /// <param name="SubscriptionID">The subscriptionID to get a list of transactions for</param>
        /// <param name="page">The page number</param>
        /// <param name="per_page">The number of results per page (used for pagination)</param>
        /// <param name="kinds">A list of the types of transactions to return.</param>
        /// <param name="since_id">Returns transactions with an ID greater than or equal to the one specified. Use int.MinValue to not specify a since_id.</param>
        /// <param name="max_id">Returns transactions with an id less than or equal to the one specified. Use int.MinValue to not specify a max_id.</param>
        /// <returns>A dictionary of transactions if successful, otherwise null.</returns>
        IDictionary<int, ITransaction> GetTransactionsForSubscription(int SubscriptionID, int page, int per_page, List<TransactionType> kinds, int since_id, int max_id);
        /// <summary>
        /// Method for getting the list of transactions for a subscription
        /// </summary>
        /// <param name="SubscriptionID">The subscriptionID to get a list of transactions for</param>
        /// <param name="page">The page number</param>
        /// <param name="per_page">The number of results per page (used for pagination)</param>
        /// <param name="kinds">A list of the types of transactions to return.</param>
        /// <param name="since_id">Returns transactions with an ID greater than or equal to the one specified. Use int.MinValue to not specify a since_id.</param>
        /// <param name="max_id">Returns transactions with an id less than or equal to the one specified. Use int.MinValue to not specify a max_id.</param>
        /// <param name="since_date">Returns transactions with a created_at date greater than or equal to the one specified. Use DateTime.MinValue to not specify a since_date.</param>
        /// <param name="until_date">Returns transactions with a created_at date less than or equal to the one specified. Use DateTime.MinValue to not specify an until_date.</param>
        /// <returns>A dictionary of transactions if successful, otherwise null.</returns>
        IDictionary<int, ITransaction> GetTransactionsForSubscription(int SubscriptionID, int page, int per_page, List<TransactionType> kinds, int since_id, int max_id, DateTime since_date, DateTime until_date);
        /// <summary>
        /// Method for determining if the properties have been set to allow this instance to connect correctly.
        /// </summary>
        bool HasConnected { get; }
        /// <summary>
        /// Get a reference to the last Http Response from the chargify server. This is set after every call to
        /// a Chargify Connect method
        /// </summary>
        HttpWebResponse LastResponse { get; }
        /// <summary>
        /// Method for retrieving information about a coupon using the ID of that coupon.
        /// </summary>
        /// <param name="ProductFamilyID">The ID of the product family that the coupon belongs to</param>
        /// <param name="CouponID">The ID of the coupon</param>
        /// <returns>The object if found, null otherwise.</returns>
        ICoupon LoadCoupon(int ProductFamilyID, int CouponID);
        /// <summary>
        /// Load the requested customer from chargify
        /// </summary>
        /// <param name="ChargifyID">The chargify ID of the customer</param>
        /// <returns>The customer with the specified chargify ID</returns>
        ICustomer LoadCustomer(int ChargifyID);
        /// <summary>
        /// Load the requested customer from chargify
        /// </summary>
        /// <param name="SystemID">The system ID of the customer</param>
        /// <returns>The customer with the specified chargify ID</returns>
        ICustomer LoadCustomer(string SystemID);
        /// <summary>
        /// Load the requested product from chargify by its handle
        /// </summary>
        /// <param name="Handle">The Chargify ID or handle of the product</param>
        /// <returns>The product with the specified chargify ID</returns>
        IProduct LoadProduct(string Handle);
        /// <summary>
        /// Load the requested product from chargify
        /// </summary>
        /// <param name="ProductID">The Chargify ID or handle of the product</param>
        /// <param name="IsHandle">If true, then the ProductID represents the handle, if false the ProductID represents the Chargify ID</param>
        /// <returns>The product with the specified chargify ID</returns>
        IProduct LoadProduct(string ProductID, bool IsHandle);
        /// <summary>
        /// Load the requested customer from chargify
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription</param>
        /// <returns>The subscription with the specified ID</returns>
        ISubscription LoadSubscription(int SubscriptionID);
        /// <summary>
        /// Load the requested transaction from Chargify
        /// </summary>
        /// <param name="ID">The ID of the transaction</param>
        /// <returns>The transaction with the specified ID</returns>
        /// <remarks>Unfortunately, this resource isn't yet available.</remarks>
        ITransaction LoadTransaction(int ID);
        /// <summary>
        /// Method that returns a list of subscriptions.
        /// </summary>
        /// <returns>Null if there are no results, object otherwise.</returns>
        IDictionary<int, ISubscription> GetSubscriptionList();
        /// <summary>
        /// Method that returns a list of subscriptions.
        /// </summary>
        /// <param name="states">A list of the states of subscriptions to return</param>
        /// <returns>Null if there are no results, object otherwise.</returns>
        IDictionary<int, ISubscription> GetSubscriptionList(List<SubscriptionState> states);
        /// <summary>
        /// Method that returns a list of subscriptions.
        /// </summary>
        /// <param name="page">The page number</param>
        /// <param name="per_page">The number of results per page (used for pagination)</param>
        /// <returns>Null if there are no results, object otherwise.</returns>
        IDictionary<int, ISubscription> GetSubscriptionList(int page, int per_page);
        /// <summary>
        /// Method to change the subscription product WITH proration.
        /// </summary>
        /// <param name="Subscription">The subscription to migrate</param>
        /// <param name="Product">The product to migrate the subscription to</param>
        /// <param name="IncludeTrial">Boolean, default false. If true, the customer will migrate to the new product
        /// if one is available. If false, the trial period will be ignored.</param>
        /// <param name="IncludeInitialCharge">Boolean, default false. If true, initial charges will be assessed.
        /// If false, initial charges will be ignored.</param>
        /// <returns></returns>
        ISubscription MigrateSubscriptionProduct(ISubscription Subscription, IProduct Product, bool IncludeTrial, bool IncludeInitialCharge);
        /// <summary>
        /// Method to change the subscription product WITH proration.
        /// </summary>
        /// <param name="Subscription">The subscription to migrate</param>
        /// <param name="ProductHandle">The product handle of the product to migrate to</param>
        /// <param name="IncludeTrial">Boolean, default false. If true, the customer will migrate to the new product
        /// if one is available. If false, the trial period will be ignored.</param>
        /// <param name="IncludeInitialCharge">Boolean, default false. If true, initial charges will be assessed.
        /// If false, initial charges will be ignored.</param>
        /// <returns>The completed subscription if migrated successfully, null otherwise.</returns>
        ISubscription MigrateSubscriptionProduct(ISubscription Subscription, string ProductHandle, bool IncludeTrial, bool IncludeInitialCharge);
        /// <summary>
        /// Method to change the subscription product WITH proration.
        /// </summary>
        /// <param name="SubscriptionID">The subscription to migrate</param>
        /// <param name="Product">The product to migrate to</param>
        /// <param name="IncludeTrial">Boolean, default false. If true, the customer will migrate to the new product
        /// if one is available. If false, the trial period will be ignored.</param>
        /// <param name="IncludeInitialCharge">Boolean, default false. If true, initial charges will be assessed.
        /// If false, initial charges will be ignored.</param>
        /// <returns>The completed subscription if migrated successfully, null otherwise.</returns>
        ISubscription MigrateSubscriptionProduct(int SubscriptionID, IProduct Product, bool IncludeTrial, bool IncludeInitialCharge);
        /// <summary>
        /// Method to change the subscription product WITH proration.
        /// </summary>
        /// <param name="SubscriptionID">The subscription to migrate</param>
        /// <param name="ProductHandle">The product handle of the product to migrate to</param>
        /// <param name="IncludeTrial">Boolean, default false. If true, the customer will migrate to the new product
        /// if one is available. If false, the trial period will be ignored.</param>
        /// <param name="IncludeInitialCharge">Boolean, default false. If true, initial charges will be assessed.
        /// If false, initial charges will be ignored.</param>
        /// <returns>The completed subscription if migrated successfully, null otherwise.</returns>
        ISubscription MigrateSubscriptionProduct(int SubscriptionID, string ProductHandle, bool IncludeTrial, bool IncludeInitialCharge);
        /// <summary>
        /// Get or set the password
        /// </summary>
        string Password { get; set; }
        /// <summary>
        /// Chargify offers the ability to reactivate a previously canceled subscription. For details
        /// on how reactivation works, and how to reactivate subscriptions through the Admin interface, see
        /// http://support.chargify.com/faqs/features/reactivation
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to reactivate</param>
        /// <returns>The newly reactivated subscription, or nothing.</returns>
        ISubscription ReactivateSubscription(int SubscriptionID);
        /// <summary>
        /// Chargify offers the ability to reactivate a previously canceled subscription. For details
        /// on how reactivation works, and how to reactivate subscriptions through the Admin interface, see
        /// http://support.chargify.com/faqs/features/reactivation
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to reactivate</param>
        /// <param name="includeTrial">If true, the reactivated subscription will include a trial if one is available.</param>
        /// <returns>The newly reactivated subscription, or nothing.</returns>
        ISubscription ReactivateSubscription(int SubscriptionID, bool includeTrial);
        /// <summary>
        /// Chargify offers the ability to reactivate a previously canceled subscription. For details
        /// on how reactivation works, and how to reactivate subscriptions through the Admin interface, see
        /// http://support.chargify.com/faqs/features/reactivation
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to reactivate</param>
        /// <param name="includeTrial">If true, the reactivated subscription will include a trial if one is available.</param>
        /// <param name="preserveBalance">If true, the existing subscription balance will NOT be cleared/reset before adding the additional reactivation charges.</param>
        /// <param name="couponCode">The coupon code to be applied during reactivation.</param>
        /// <returns>The newly reactivated subscription, or nothing.</returns>
        ISubscription ReactivateSubscription(int SubscriptionID, bool includeTrial, bool? preserveBalance, string couponCode);
        /// <summary>
        /// Method for reseting a subscription balance to zero (removes outstanding balance). 
        /// Useful when reactivating subscriptions, and making sure not to charge the user
        /// their existing balance when then cancelled.
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to modify.</param>
        /// <returns>True if successful, false otherwise.</returns>
        bool ResetSubscriptionBalance(int SubscriptionID);
        /// <summary>
        /// SharedKey used for url generation
        /// </summary>
        string SharedKey { get; set; }
        /// <summary>
        /// Update the specified chargify customer
        /// </summary>
        /// <param name="Customer">The customer to update</param>
        /// <returns>The updated customer, null otherwise.</returns>
        ICustomer UpdateCustomer(ICustomer Customer);
        /// <summary>
        /// Update the credit card information for an existing subscription
        /// </summary>
        /// <param name="Subscription">The subscription to update credit card info for</param>
        /// <param name="CreditCardAttributes">The attributes for the updated credit card</param>
        /// <returns>The new subscription resulting from the change</returns>
        ISubscription UpdateSubscriptionCreditCard(ISubscription Subscription, ICreditCardAttributes CreditCardAttributes);
        /// <summary>
        /// Update the credit card information for an existing subscription
        /// </summary>
        /// <param name="Subscription">The subscription to update credit card info for</param>
        /// <param name="FullNumber">The full number of the credit card (optional - set to null if not required)</param>
        /// <param name="ExpirationMonth">The expiration month of the credit card (optional - set to null if not required)</param>
        /// <param name="ExpirationYear">The expiration year of the credit card (optional - set to null if not required)</param>
        /// <param name="CVV">The CVV for the credit card (optional - set to null if not required)</param>
        /// <param name="BillingAddress">The billing address (optional - set to null if not required)</param>
        /// <param name="BillingCity">The billing city (optional - set to null if not required)</param>
        /// <param name="BillingState">The billing state or province (optional - set to null if not required)</param>
        /// <param name="BillingZip">The billing zip code or postal code (optional - set to null if not required)</param>
        /// <param name="BillingCountry">The billing country (optional - set to null if not required)</param>
        /// <returns>The new subscription resulting from the change</returns>
        ISubscription UpdateSubscriptionCreditCard(ISubscription Subscription, string FullNumber, int? ExpirationMonth, int? ExpirationYear, string CVV, string BillingAddress, string BillingCity, string BillingState, string BillingZip, string BillingCountry);
        /// <summary>
        /// Update the credit card information for an existing subscription
        /// </summary>
        /// <param name="SubscriptionID">The ID of the suscription to update</param>
        /// <param name="CreditCardAttributes">The attributes for the update credit card</param>
        /// <returns>The new subscription resulting from the change</returns>
        ISubscription UpdateSubscriptionCreditCard(int SubscriptionID, ICreditCardAttributes CreditCardAttributes);
        /// <summary>
        /// Update the credit card information for an existing subscription
        /// </summary>
        /// <param name="SubscriptionID">The ID of the suscription to update</param>
        /// <param name="FullNumber">The full number of the credit card (optional - set to null if not required)</param>
        /// <param name="ExpirationMonth">The expiration month of the credit card (optional - set to null if not required)</param>
        /// <param name="ExpirationYear">The expiration year of the credit card (optional - set to null if not required)</param>
        /// <param name="CVV">The CVV for the credit card (optional - set to null if not required)</param>
        /// <param name="BillingAddress">The billing address (optional - set to null if not required)</param>
        /// <param name="BillingCity">The billing city (optional - set to null if not required)</param>
        /// <param name="BillingState">The billing state or province (optional - set to null if not required)</param>
        /// <param name="BillingZip">The billing zip code or postal code (optional - set to null if not required)</param>
        /// <param name="BillingCountry">The billing country (optional - set to null if not required)</param>
        /// <returns>The new subscription resulting from the change</returns>
        ISubscription UpdateSubscriptionCreditCard(int SubscriptionID, string FullNumber, int? ExpirationMonth, int? ExpirationYear, string CVV, string BillingAddress, string BillingCity, string BillingState, string BillingZip, string BillingCountry);
        /// <summary>
        /// Update the credit card information for an existing subscription
        /// </summary>
        /// <param name="SubscriptionID">The ID of the suscription to update</param>
        /// <param name="FirstName">The first name on the credit card</param>
        /// <param name="LastName">The last name on the credit card</param>
        /// <param name="FullNumber">The full number of the credit card (optional - set to null if not required)</param>
        /// <param name="ExpirationMonth">The expiration month of the credit card (optional - set to null if not required)</param>
        /// <param name="ExpirationYear">The expiration year of the credit card (optional - set to null if not required)</param>
        /// <param name="CVV">The CVV for the credit card (optional - set to null if not required)</param>
        /// <param name="BillingAddress">The billing address (optional - set to null if not required)</param>
        /// <param name="BillingCity">The billing city (optional - set to null if not required)</param>
        /// <param name="BillingState">The billing state or province (optional - set to null if not required)</param>
        /// <param name="BillingZip">The billing zip code or postal code (optional - set to null if not required)</param>
        /// <param name="BillingCountry">The billing country (optional - set to null if not required)</param>
        /// <returns>The new subscription resulting from the change</returns>
        ISubscription UpdateSubscriptionCreditCard(int SubscriptionID, string FirstName, string LastName, string FullNumber, int? ExpirationMonth, int? ExpirationYear, string CVV, string BillingAddress, string BillingCity, string BillingState, string BillingZip, string BillingCountry);
        /// <summary>
        /// Update the specified chargify subscription
        /// </summary>
        /// <param name="Subscription">The subscription to update</param>
        /// <returns>The updated subscriptionn, null otherwise.</returns>
        ISubscription UpdateSubscription(ISubscription Subscription);
        /// <summary>
        /// Update the collection method of the subscription
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to update</param>
        /// <param name="PaymentCollectionMethod">The collection method to set</param>
        /// <returns>The full details of the updated subscription</returns>
        ISubscription UpdatePaymentCollectionMethod(int SubscriptionID, PaymentCollectionMethod PaymentCollectionMethod);
        /// <summary>
        /// Get or set the URL for chargify
        /// </summary>
        string URL { get; set; }
        /// <summary>
        /// Should Chargify.NET use JSON for output? XML by default.
        /// </summary>
        bool UseJSON { get; set; }
        /// <summary>
        /// The timeout (in milliseconds) for any call to Chargify. The default is 180000
        /// </summary>
        int Timeout { get; set; }
        /// <summary>
        /// Caller can plug in a delegate for logging raw Chargify requests.
        /// Method, URL, and Data are the parameters.
        /// </summary>
        Action<HttpRequestMethod, string, string> LogRequest { get; set; }
        /// <summary>
        /// Caller can plug in a delegate for logging raw Chargify responses (including errors)
        /// Http Status, URL, and response body are the parameters.
        /// </summary>
        Action<HttpStatusCode, string, string> LogResponse { get; set; }
        /// <summary>
        /// Method to update the allocated amount of a component for a subscription
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to modify the allocation for</param>
        /// <param name="ComponentID">The ID of the component</param>
        /// <param name="NewAllocatedQuantity">The quantity of component to allocate to the subscription</param>
        IComponentAttributes UpdateComponentAllocationForSubscription(int SubscriptionID, int ComponentID, int NewAllocatedQuantity);
        /// <summary>
        /// Method to retrieve the current information (including allocation) of a component against a subscription
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription in question</param>
        /// <param name="ComponentID">The ID of the component</param>
        /// <returns>The ComponentAttributes object, null otherwise.</returns>
        IComponentAttributes GetComponentInfoForSubscription(int SubscriptionID, int ComponentID);
        /// <summary>
        /// Create a refund
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to refund</param>
        /// <param name="PaymentID">The ID of the payment that the credit will be applied to</param>
        /// <param name="Amount">The amount (in dollars and cents) like 10.00 is $10.00</param>
        /// <param name="Memo">A helpful explanation for the refund.</param>
        /// <returns>The IRefund object indicating successful, or unsuccessful.</returns>
        IRefund CreateRefund(int SubscriptionID, int PaymentID, decimal Amount, string Memo);
        /// <summary>
        /// Create a refund
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to refund</param>
        /// <param name="PaymentID">The ID of the payment that the credit will be applied to</param>
        /// <param name="AmountInCents">The amount (in cents only) like 100 is $1.00</param>
        /// <param name="Memo">A helpful explanation for the refund.</param>
        /// <returns>The IRefund object indicating successful, or unsuccessful.</returns>
        IRefund CreateRefund(int SubscriptionID, int PaymentID, int AmountInCents, string Memo);
        /// <summary>
        /// Method for getting a specific statement
        /// </summary>
        /// <param name="StatementID">The ID of the statement to retrieve</param>
        /// <returns>The statement if found, null otherwise.</returns>
        IStatement LoadStatement(int StatementID);
        /// <summary>
        /// Individual PDF Statements can be retrieved by using the Accept/Content-Type header application/pdf or appending .pdf as the format portion of the URL:
        /// </summary>
        /// <param name="StatementID">The ID of the statement to retrieve the byte[] for</param>
        /// <returns>A byte[] of the PDF data, to be sent to the user in a download</returns>
        byte[] LoadStatementPDF(int StatementID);
        /// <summary>
        /// Method for getting a list of statment ids for a specific subscription
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to retrieve the statements for</param>
        /// <returns>The list of statements, an empty dictionary otherwise.</returns>
        IList<int> GetStatementIDs(int SubscriptionID);
        /// <summary>
        /// Method for getting a list of statment ids for a specific subscription
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to retrieve the statements for</param>
        /// <param name="page">The page number to return</param>
        /// <param name="per_page">The number of results to return per page</param>
        /// <returns>The list of statements, an empty dictionary otherwise.</returns>
        IList<int> GetStatementIDs(int SubscriptionID, int page, int per_page);
        /// <summary>
        /// Method for getting a list of statments for a specific subscription
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to retrieve the statements for</param>
        /// <returns>The list of statements, an empty dictionary otherwise.</returns>
        IDictionary<int, IStatement> GetStatementList(int SubscriptionID);
        /// <summary>
        /// Method for getting a list of statments for a specific subscription
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to retrieve the statements for</param>
        /// <param name="page">The page number to return</param>
        /// <param name="per_page">The number of results to return per page</param>
        /// <returns>The list of statements, an empty dictionary otherwise.</returns>
        IDictionary<int, IStatement> GetStatementList(int SubscriptionID, int page, int per_page);
        /// <summary>
        /// Returns the 50 most recent Allocations, ordered by most recent first.
        /// </summary>
        /// <param name="SubscriptionID">The subscriptionID to scope this request</param>
        /// <param name="ComponentID">The componentID to scope this request</param>
        /// <param name="Page">Pass an integer in the page parameter via the query string to access subsequent pages of 50 transactions</param>
        /// <returns>A dictionary of allocation objects keyed by ComponentID, or null.</returns>
        IDictionary<int, List<IComponentAllocation>> GetAllocationListForSubscriptionComponent(int SubscriptionID, int ComponentID, int? Page = 0);
        /// <summary>
        /// Creates a new Allocation, setting the current allocated quantity for the component and recording a memo.
        /// </summary>
        /// <param name="SubscriptionID"></param>
        /// <param name="ComponentID"></param>
        /// <param name="Allocation"></param>
        /// <returns></returns>
        IComponentAllocation CreateComponentAllocation(int SubscriptionID, int ComponentID, ComponentAllocation Allocation);

        /// <summary>
        /// Creates a new Allocation, setting the current allocated quantity for the component and recording a memo.
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to apply this quantity allocation to</param>
        /// <param name="ComponentID">The ID of the component to apply this quantity allocation to</param>
        /// <param name="Quantity">The allocated quantity to which to set the line-items allocated quantity. This should always be an integer. For On/Off components, use 1 for on and 0 for off.</param>
        /// <returns></returns>
        IComponentAllocation CreateComponentAllocation(int SubscriptionID, int ComponentID, int Quantity);

        /// <summary>
        /// Creates a new Allocation, setting the current allocated quantity for the component and recording a memo.
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to apply this quantity allocation to</param>
        /// <param name="ComponentID">The ID of the component to apply this quantity allocation to</param>
        /// <param name="Quantity">The allocated quantity to which to set the line-items allocated quantity. This should always be an integer. For On/Off components, use 1 for on and 0 for off.</param>
        /// <param name="Memo">(optional) A memo to record along with the allocation</param>
        /// <returns></returns>
        IComponentAllocation CreateComponentAllocation(int SubscriptionID, int ComponentID, int Quantity, string Memo);

        /// <summary>
        /// Creates a new Allocation, setting the current allocated quantity for the component and recording a memo.
        /// </summary>
        /// <param name="SubscriptionID">The ID of the subscription to apply this quantity allocation to</param>
        /// <param name="ComponentID">The ID of the component to apply this quantity allocation to</param>
        /// <param name="Quantity">The allocated quantity to which to set the line-items allocated quantity. This should always be an integer. For On/Off components, use 1 for on and 0 for off.</param>
        /// <param name="Memo">(optional) A memo to record along with the allocation</param>
        /// <param name="UpgradeScheme">(optional) The scheme used if the proration is an upgrade. Defaults to the site setting if one is not provided.</param>
        /// <param name="DowngradeScheme">(optional) The scheme used if the proration is a downgrade. Defaults to the site setting if one is not provided.</param>
        /// <returns>The component allocation object, null otherwise.</returns>
        IComponentAllocation CreateComponentAllocation(int SubscriptionID, int ComponentID, int Quantity, string Memo, ComponentUpgradeProrationScheme UpgradeScheme, ComponentDowngradeProrationScheme DowngradeScheme);
    }
}
