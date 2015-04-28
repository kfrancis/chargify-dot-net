
#region License, Terms and Conditions
//
// MigrationPreview.cs
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
    using System.Linq;
    using System.Text;
    using System.Xml;
    using ChargifyNET.Json;
    #endregion

    /// <summary>
    /// One-time (or one-off) migration for arbitrary amount
    /// </summary>
    public class Migration : ChargifyBase, IMigration, IComparable<Migration>
    {
        #region Field Keys
        private const string ProratedAdjustmentInCentsKey = "prorated_adjustment_in_cents";
        private const string ChargeInCentsKey = "charge_in_cents";
        private const string PaymentDueInCentsKey = "payment_due_in_cents";
        private const string CreditAppliedInCentsKey = "credit_applied_in_cents";
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor.  Values set to default
        /// </summary>
        public Migration()
            : base()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="MigrationPreviewXML">XML containing migration info (in expected format)</param>
        public Migration(string MigrationPreviewXML)
            : base()
        {
            // get the XML into an XML document
            XmlDocument Doc = new XmlDocument();
            Doc.LoadXml(MigrationPreviewXML);
            if (Doc.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", "MigrationPreviewXML");
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in Doc.ChildNodes)
            {
                if (elementNode.Name == "migration")
                {
                    this.LoadFromNode(elementNode);
                    return;
                }
            }
            // if we get here, then no info was found
            throw new ArgumentException("XML does not contain migration preview information", "MigrationPreviewXML");
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="migrationNode">XML containing migration info (in expected format)</param>
        internal Migration(XmlNode migrationNode)
            : base()
        {
            if (migrationNode == null) throw new ArgumentNullException("migrationNode");
            if (migrationNode.Name != "migration") throw new ArgumentException("Not a vaild migration node", "migrationNode");
            if (migrationNode.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", "migrationNode");
            this.LoadFromNode(migrationNode);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="migrationObject">JsonObject containing migration info (in expected format)</param>
        public Migration(JsonObject migrationObject)
        {
            if (migrationObject == null) throw new ArgumentNullException("migrationObject");
            if (migrationObject.Keys.Count <= 0) throw new ArgumentException("Not a vaild migration object", "migrationObject");
            this.LoadFromJSON(migrationObject);
        }

        /// <summary>
        /// Load data from a JsonObject
        /// </summary>
        /// <param name="obj">The JsonObject with migration data</param>
        private void LoadFromJSON(JsonObject obj)
        {
            foreach (string key in obj.Keys)
            {
                switch (key)
                {
                    case ProratedAdjustmentInCentsKey:
                        _proratedAdjustmentInCents = obj.GetJSONContentAsInt(key);
                        break;
                    case ChargeInCentsKey:
                        _chargeInCents = obj.GetJSONContentAsInt(key);
                        break;
                    case PaymentDueInCentsKey:
                        _paymentDueInCents = obj.GetJSONContentAsInt(key);
                        break;
                    case CreditAppliedInCentsKey:
                        _creditAppliedInCents = obj.GetJSONContentAsInt(key);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Load data from a migration node
        /// </summary>
        /// <param name="migrationNode">The migration node</param>
        private void LoadFromNode(XmlNode migrationNode)
        {
            foreach (XmlNode dataNode in migrationNode.ChildNodes)
            {
                switch (dataNode.Name)
                {
                    case ProratedAdjustmentInCentsKey:
                        _proratedAdjustmentInCents = dataNode.GetNodeContentAsInt();
                        break;
                    case ChargeInCentsKey:
                        _chargeInCents = dataNode.GetNodeContentAsInt();
                        break;
                    case PaymentDueInCentsKey:
                        _paymentDueInCents = dataNode.GetNodeContentAsInt();
                        break;
                    case CreditAppliedInCentsKey:
                        _creditAppliedInCents = dataNode.GetNodeContentAsInt();
                        break;
                    default:
                        break;

                }
            }
        }
        #endregion

        #region IMigrationPreview Members

        /// <summary>
        /// 
        /// </summary>
        public int ProratedAdjustmentInCents
        {
            get { return _proratedAdjustmentInCents; }
        }
        private int _proratedAdjustmentInCents = int.MinValue;

        /// <summary>
        /// Get the charge, in cents
        /// </summary>
        public int ChargeInCents
        {
            get { return _chargeInCents; }
        }
        private int _chargeInCents = int.MinValue;


        /// <summary>
        /// Get the charge, in cents
        /// </summary>
        public int PaymentDueInCents
        {
            get { return _paymentDueInCents; }
        }
        private int _paymentDueInCents = int.MinValue;

        /// <summary>
        /// Credit applied in cents
        /// </summary>
        public int CreditAppliedInCents
        {
            get { return _creditAppliedInCents; }
        }
        private int _creditAppliedInCents = int.MinValue;

        #endregion

        #region IComparable<IMigrationPreview> Members

        /// <summary>
        /// Compare this migration to another
        /// </summary>
        public int CompareTo(IMigration other)
        {
            return -1;
        }

        #endregion

        #region IComparable<MigrationPreview> Members

        /// <summary>
        /// Compare this migration to another
        /// </summary>
        public int CompareTo(Migration other)
        {
            return -1;
        }

        #endregion
    }
}