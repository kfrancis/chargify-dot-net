﻿#if NETFULL
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using ChargifyNET.Data;

// ReSharper disable once CheckNamespace
namespace ChargifyNET.Controls
{
    /// <summary>
    /// Custom control
    /// </summary>
    [ToolboxData("<{0}:CountryDropDownList runat=\"server\"></{0}:CountryDropDownList>")]
    public class CountryDropDownList : DropDownList
    {
        /// <summary>
        /// Overloading the OnLoad method to inject the countries from the ISO 3166-1 Alpha 2 list
        /// </summary>
        /// <param name="e">The event args of the OnLoad</param>
        protected override void OnLoad(EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                CountryNameLookup lookup = new();
                foreach (var country in lookup.GetData())
                {
                    // Since the data comes in as Unicode, we need to ASCII encode it for HTML.                    
                    var countryName = Encoding.ASCII.GetString(Encoding.Unicode.GetBytes(country.Value));
                    Items.Add(new ListItem(countryName, country.Key));
                }
            }
            base.OnLoad(e);
        }
    }
}
#endif
