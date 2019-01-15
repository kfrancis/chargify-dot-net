
#region License, Terms and Conditions
//
// IInvoice.cs
//
// Authors: Chris Baxter
// Copyright (C) 2019 Loaded Reports Ltd. All rights reserved.
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

// ReSharper disable once CheckNamespace
namespace ChargifyNET
{
    #region Imports
    using System;
    using System.Collections.Generic;
    #endregion

    /// <summary>
    /// Represents an invoice from Chargify's relationship Invoicing system
    /// See https://reference.chargify.com/v1/relationship-invoicing-new
    /// </summary>
    public interface IRelationshipInvoice : IChargifyEntity
    {
        /// <summary>
        /// Unique identifier for the invoice. It is generated automatically by Chargify and has the prefix "inv_" followed by alphanumeric characters.
        /// </summary>
        string Uid { get; set; }
        /// <summary>
        /// ID of the site to which the invoice belongs.
        /// </summary>
        int SiteID { get; set; }
        /// <summary>
        /// ID of the customer to which the invoice belongs.
        /// </summary>
        int CustomerID { get; set; }
        /// <summary>
        /// The subscription unique id within Chargify
        /// </summary>
        int SubscriptionID { get; set; }
        /// <summary>
        /// A unique, identifying string that appears on the invoice and in places the invoice is referenced.
        /// While the UID is long and not appropriate to show to customers, the number is usually shorter and consumable by the customer and the merchant alike.
        /// </summary>
        string Number { get; set; }
        /// <summary>
        /// A monotonically increasing number assigned to invoices as they are created. This number is unique 
        /// within a site and can be used to sort and order invoices.
        /// </summary>
        int SequenceNumber { get; set; }
        /// <summary>
        /// Date the invoice was issued to the customer. This is the date that the invoice was made available for payment.
        /// </summary>
        DateTime IssueDate { get; set; }
        /// <summary>
        /// Date the invoice is due.
        /// </summary>
        DateTime DueDate { get; set; }
        /// <summary>
        /// Date the invoice became fully paid.
        /// </summary>
        DateTime PaidDate { get; set; }
        /// <summary>
        /// Current status of the invoice.
        /// </summary>
        RelationshipInvoiceStatus Status { get; set; }
        /// <summary>
        /// The collection method of the invoice, which is either "automatic" (tried and retried on an existing payment method by Chargify) 
        /// or "remittance" (payment must be remitted by the customer or keyed in by the merchant).
        /// </summary>
        PaymentCollectionMethod CollectionMethod { get; set; }
        /// <summary>
        /// A message that is printed on the invoice when it is marked for remittance collection.
        /// It is intended to describe to the customer how they may make payment, and is configured by the merchant.
        /// </summary>
        string PaymentInstructions { get; set; }
        /// <summary>
        /// The ISO 4217 currency code (3 character string) representing the currency of invoice transaction.
        /// </summary>
        string Currency { get; set; }
        /// <summary>
        /// Consolidation level of the invoice, which is applicable to invoice consolidation.
        /// </summary>
        RelationshipInvoiceCondolidationLevel ConsolidationLevel { get; set; }
        /// <summary>
        /// For invoices with consolidation_level of child, this specifies the UID of the parent (consolidated) invoice.
        /// </summary>
        string ParentInvoiceUid { get; set; }
        /// <summary>
        /// For invoices with consolidation_level of child, this specifies the number of the parent (consolidated) invoice.
        /// </summary>
        int ParentInvoiceNumber { get; set; }
        /// <summary>
        /// For invoices with consolidation_level of parent, this specifies the ID of the subscription which was the
        /// primary subscription of the subscription group that generated the invoice.
        /// </summary>
        string GroupPrimarySubscriptionId { get; set; }
        /// <summary>
        /// The name of the product subscribed when the invoice was generated.
        /// </summary>
        string ProductName { get; set; }
        /// <summary>
        /// The name of the product family subscribed when the invoice was generated.
        /// </summary>
        string ProductFamilyName { get; set; }
        /// <summary>
        /// Information about the seller (merchant) listed on the masthead of the invoice.
        /// </summary>
        IRelationshipInvoiceSeller Seller { get; set; }
        /// <summary>
        /// Information about the customer who is owner or recipient the invoiced subscription.
        /// </summary>
        IRelationshipInvoiceCustomer Customer { get; set; }
        /// <summary>
        /// The memo printed on invoices of any collection type. This message is in control of the merchant.
        /// </summary>
        string Memo { get; set; }
        /// <summary>
        /// The invoice billing address.
        /// </summary>
        IRelationshipInvoiceAddress BillingAddress { get; set; }
        /// <summary>
        /// The invoice shipping address.
        /// </summary>
        IRelationshipInvoiceAddress ShippingAddress { get; set; }
        /// <summary>
        /// Subtotal of the invoice, which is the sum of all line items before discounts or taxes.
        /// </summary>
        decimal SubtotalAmount { get; set; }
        /// <summary>
        /// Total discount applied to the invoice.
        /// </summary>
        decimal DiscountAmount { get; set; }
        /// <summary>
        /// Total tax on the invoice.
        /// </summary>
        decimal TaxAmount { get; set; }
        /// <summary>
        /// The invoice total, which is subtotal_amount - discount_amount + tax_amount.'
        /// </summary>
        decimal TotalAmount { get; set; }
        /// <summary>
        /// The amount of credit (from credit notes) applied to this invoice.
        /// Credits offset the amount due from the customer.
        /// </summary>
        decimal CreditAmount { get; set; }
        /// <summary>
        /// Refund amount
        /// </summary>
        decimal RefundAmount { get; set; }
        /// <summary>
        /// The amount paid on the invoice by the customer.
        /// </summary>
        decimal PaidAmount { get; set; }
        /// <summary>
        /// Amount due on the invoice, which is total_amount - credit_amount - paid_amount.
        /// </summary>
        decimal DueAmount { get; set; }
        /// <summary>
        /// Line items on the invoice.
        /// </summary>
        List<IRelationshipInvoiceLineItem> LineItems { get; set; }
        /// <summary>
        /// Discounts in the invoice
        /// </summary>
        List<IRelationshipInvoiceDiscount> Discounts { get; set; }
        /// <summary>
        /// Taxes on the invoice
        /// </summary>
        List<IRelationshipInvoiceTax> Taxes { get; set; }
        /// <summary>
        /// Credits on the invoice
        /// </summary>
        List<IRelationshipInvoiceCredit> Credits { get; set; }
        /// <summary>
        /// Refunds on the invoice
        /// </summary>
        List<IRelationshipInvoiceRefund> Refunds { get; set; }
        /// <summary>
        /// Payments on the invoice
        /// </summary>
        List<IRelationshipInvoicePayment> Payments { get; set; }
        /// <summary>
        /// Custom fields
        /// </summary>
        List<IRelationshipInvoiceCustomField> CustomFields { get; set; }
        /// <summary>
        /// The public URL of the invoice
        /// </summary>
        string PublicUrl { get; set; }
    }
}