﻿
#region License, Terms and Conditions
//
// IPaymentProfileAttributes.cs
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

using System.Xml.Serialization;

namespace ChargifyNET
{
    #region Imports
    using System;
    #endregion

    /// <summary>
    /// The types of credit cards supported by Chargify internally
    /// </summary>
    public enum CardType
    {
        /// <summary>
        /// Visa
        /// </summary>
        Visa,
        /// <summary>
        /// Mastercard
        /// </summary>
        Master,
        /// <summary>
        /// Discover
        /// </summary>
        Discover,
        /// <summary>
        /// American Express
        /// </summary>
        American_Express,
        /// <summary>
        /// Diners Club
        /// </summary>
        Diners_Club,
        /// <summary>
        /// JCB
        /// </summary>
        JCB,
        /// <summary>
        /// Switch
        /// </summary>
        Switch,
        /// <summary>
        /// Solo
        /// </summary>
        Solo,
        /// <summary>
        /// Dankort
        /// </summary>
        Dankort,
        /// <summary>
        /// Maestro
        /// </summary>
        Maestro,
        /// <summary>
        /// Forbrugsforeningen
        /// </summary>
        Forbrugsforeningen,
        /// <summary>
        /// Laser
        /// </summary>
        Laser,
        /// <summary>
        /// Internal value used to determine if the field has been set.
        /// </summary>
        Unknown = -1
    }

    /// <summary>
    /// The vaults supported by Chargify for importing
    /// </summary>
    public enum VaultType 
    {
        /// <summary>
        /// Authorize.NET
        /// </summary>
        [XmlEnum(Name= "authorizenet")]
        AuthorizeNET,
        /// <summary>
        /// Trust Commerce
        /// </summary>
        [XmlEnum(Name= "trust_commerce")]
        Trust_Commerce,
        /// <summary>
        /// Payment Express
        /// </summary>
        [XmlEnum(Name = "payment_express")]
        Payment_Express,
        /// <summary>
        /// Beanstream
        /// </summary>
        [XmlEnum(Name = "cbeanstream")]
        Beanstream,
        /// <summary>
        /// Braintree Version 1 (Orange)
        /// </summary>
        [XmlEnum(Name = "braintree1")]
        Braintree1,
        /// <summary>
        /// Braintree Blue
        /// </summary>
        [XmlEnum(Name = "braintree_blue")]
        Braintree_Blue,
        /// <summary>
        /// PayPal
        /// </summary>
        [XmlEnum(Name = "paypal")]
        PayPal,
        /// <summary>
        /// QuickPay
        /// </summary>
        [XmlEnum(Name = "quickpay")]
        QuickPay,
        /// <summary>
        /// Eway 
        /// </summary>
        [XmlEnum(Name = "eway")]
        Eway,
        /// <summary>
        /// Samurai
        /// </summary>
        [XmlEnum(Name = "samurai")]
        Samurai,
        /// <summary>
        /// Stripe
        /// </summary>
        [XmlEnum(Name = "stripe")]
        Stripe,
        /// <summary>
        /// Pin
        /// </summary>
        [XmlEnum(Name = "pin")]
        Pin,
        /// <summary>
        /// Wirecard
        /// </summary>
        [XmlEnum(Name = "wirecard")]
        Wirecard,
        /// <summary>
        /// Bpoint
        /// </summary>
        [XmlEnum(Name = "bpoint")]
        Bpoint,
        /// <summary>
        /// FirstData
        /// </summary>
        [XmlEnum(Name = "firstdata")]
        FirstData,
        /// <summary>
        /// Elavon Virtual Merchant Gateway
        /// </summary>
        [XmlEnum(Name = "elavon")]
        Elavon,
        /// <summary>
        /// CyberSource
        /// </summary>
        [XmlEnum(Name = "cybersource")]
        CyberSource,
        /// <summary>
        /// PayMill
        /// </summary>
        [XmlEnum(Name = "paymill")]
        PayMill,
        /// <summary>
        /// Litle
        /// </summary>
        [XmlEnum(Name = "litle")]
        Litle,
        /// <summary>
        /// Moneris
        /// </summary>
        [XmlEnum(Name = "moneris")]
        Moneris,
        /// <summary>
        /// Fusebox
        /// </summary>
        [XmlEnum(Name = "fusebox")]
        Fusebox,
        /// <summary>
        /// Internal value used to determine if the field has been set.
        /// </summary>
        Unknown = -1
    }

    /// <summary>
    /// The type of bank account
    /// </summary>
    public enum BankAccountType
    {
        /// <summary>
        /// Checking
        /// </summary>
        Checking,
        /// <summary>
        /// Savings
        /// </summary>
        Savings,
        /// <summary>
        /// Internal value used to determine if the field has been set.
        /// </summary>
        Unknown = -1
    }

    /// <summary>
    /// The primary account purpose
    /// </summary>
    public enum BankAccountHolderType
    {
        /// <summary>
        /// Personal
        /// </summary>
        Personal,
        /// <summary>
        /// Buisiness
        /// </summary>
        Business,
        /// <summary>
        /// Internal value used to determine if the field has been set.
        /// </summary>
        Unknown = -1
    }

    /// <summary>
    /// Class which can be used to "import" subscriptions via the API into Chargify
    /// Info here: http://support.chargify.com/faqs/api/api-subscription-and-stored-card-token-imports
    /// </summary>
    public interface IPaymentProfileAttributes : IComparable<IPaymentProfileAttributes>
    {
        /// <summary>
        /// The "token" provided by your vault storage for an already stored payment profile
        /// </summary>
        string VaultToken { get; set; }
        /// <summary>
        /// (Only for Authorize.NET CIM storage) The "customerProfileId" for the owner of the
        /// "customerPaymentProfileId" provided as the VaultToken
        /// </summary>
        string CustomerVaultToken { get; set; }
        /// <summary>
        /// The vault that stores the payment profile with the provided VaultToken
        /// </summary>
        VaultType CurrentVault { get; set; }
        /// <summary>
        /// The year of expiration
        /// </summary>
        int ExpirationYear { get; set; }
        /// <summary>
        /// The month of expiration
        /// </summary>
        int ExpirationMonth { get; set; }
        /// <summary>
        /// (Optional) If you know the card type, you may supply it here so that we may display 
        /// the card type in the UI.
        /// </summary>
        CardType CardType { get; set; }
        /// <summary>
        /// (Optional) If you have the last 4 digits of the credit card number, you may supply
        /// them here so we may create a masked card number for display in the UI
        /// </summary>
        string LastFour { get; set; }
        /// <summary>
        /// The name of the bank where the customer's account resides
        /// </summary>
        string BankName { get; set; }
        /// <summary>
        /// The routing number of the bank
        /// </summary>
        string BankRoutingNumber { get; set; }
        /// <summary>
        /// The customer's bank account number
        /// </summary>
        string BankAccountNumber { get; set; }
        /// <summary>
        /// Either checking or savings
        /// </summary>
        BankAccountType BankAccountType { get; set; }
        /// <summary>
        /// Either personal or business
        /// </summary>
        BankAccountHolderType BankAccountHolderType { get; set; }
        /// <summary>
        /// Paypal payment method nonce
        /// </summary>
        string PaymentMethodNonce { get; set; }
        /// <summary>
        /// Paypal email
        /// </summary>
        string FirstName { get; set; }
        string LastName { get; set; }

        string PayPalEmail { get; set; }

        string BillingAddress { get; set; }

        string BillingAddress2 { get; set; }

        string BillingCity { get; set; }


        string BillingState { get; set; }

        string BillingZip { get; set; }

        string BillingCountry { get; set; }

    }
}
