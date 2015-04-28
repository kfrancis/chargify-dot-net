
#region License, Terms and Conditions
//
// SiteStatistics.cs
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
    using ChargifyNET;
    using ChargifyNET.Json;
    using System;
    #endregion

    /// <summary>
    /// The statistics object for a site
    /// </summary>
    public class SiteStatistics : ChargifyBase, ISiteStatistics
    {
        #region Field Keys
        private const string SellerNameKey = "seller_name";
        private const string SiteNameKey = "site_name";
        private const string StatsKey = "stats";
        private const string TotalSubscriptionsKey = "total_subscriptions";
        private const string SubscriptionsTodayKey = "subscriptions_today";
        private const string TotalRevenueKey = "total_revenue";
        private const string RevenueTodayKey = "revenue_today";
        private const string RevenueThisMonthKey = "revenue_this_month";
        private const string RevenueThisYearKey = "revenue_this_year";
        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public SiteStatistics()
            : base()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="statsObject">An JsonObject with stats information</param>
        public SiteStatistics(JsonObject statsObject)
            : base()
        {
            if (statsObject == null) throw new ArgumentNullException("statsObject");
            if (statsObject.Keys.Count <= 0) throw new ArgumentException("Not a vaild stats object", "statsObject");
            this.LoadFromJSON(statsObject);
        }

        private void LoadFromJSON(JsonObject obj)
        {
            // loop through the keys of this JsonObject to get stats info, and parse it out
            foreach (string key in obj.Keys)
            {
                switch (key)
                {
                    case SellerNameKey:
                        _sellerName = obj.GetJSONContentAsString(key);
                        break;
                    case SiteNameKey:
                        _siteName = obj.GetJSONContentAsString(key);
                        break;
                    case StatsKey:
                        JsonObject statsObj = obj[key] as JsonObject;
                        foreach (string innerKey in statsObj.Keys)
                        {
                            switch (innerKey)
                            {
                                case TotalSubscriptionsKey:
                                    _totalSubscriptions = statsObj.GetJSONContentAsInt(innerKey);
                                    break;
                                case SubscriptionsTodayKey:
                                    _subscriptionsToday = statsObj.GetJSONContentAsInt(innerKey);
                                    break;
                                case TotalRevenueKey:
                                    _totalRevenue = statsObj.GetJSONContentAsString(innerKey);
                                    break;
                                case RevenueTodayKey:
                                    _revenueToday = statsObj.GetJSONContentAsString(innerKey);
                                    break;
                                case RevenueThisMonthKey:
                                    _revenueThisMonth = statsObj.GetJSONContentAsString(innerKey);
                                    break;
                                case RevenueThisYearKey:
                                    _revenueThisYear = statsObj.GetJSONContentAsString(innerKey);
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #region ISiteStatistics Members

        /// <summary>
        /// The name of the seller who "owns" the site.
        /// </summary>
        public string SellerName { get { return _sellerName; } }
        private string _sellerName = string.Empty;

        /// <summary>
        /// The name of the site
        /// </summary>
        public string SiteName { get { return _siteName; } }
        private string _siteName = string.Empty;

        /// <summary>
        /// The total number of active subscriptions
        /// </summary>
        public int TotalSubscriptions { get { return _totalSubscriptions; } }
        private int _totalSubscriptions = int.MinValue;

        /// <summary>
        /// The total number of signups today
        /// </summary>
        public int SubscriptionsToday { get { return _subscriptionsToday; } }
        private int _subscriptionsToday = int.MinValue;

        /// <summary>
        /// Total site revenue
        /// </summary>
        public string TotalRevenue { get { return _totalRevenue; } }
        private string _totalRevenue = string.Empty;

        /// <summary>
        /// Today's revenue
        /// </summary>
        public string RevenueToday { get { return _revenueToday; } }
        private string _revenueToday = string.Empty;

        /// <summary>
        /// The site revenue for this month
        /// </summary>
        public string RevenueThisMonth { get { return _revenueThisMonth; } }
        private string _revenueThisMonth = string.Empty;

        /// <summary>
        /// The site revenue for this year
        /// </summary>
        public string RevenueThisYear { get { return _revenueThisYear; } }
        private string _revenueThisYear = string.Empty;

        #endregion
    }
}
