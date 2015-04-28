using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using ChargifyNET.Data;
using System.Web;

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
                CountryNameLookup lookup = new CountryNameLookup();
                foreach (KeyValuePair<string, string> country in lookup.GetData())
                {
                    // Since the data comes in as Unicode, we need to ASCII encode it for HTML.                    
                    string countryName = Encoding.ASCII.GetString(Encoding.Unicode.GetBytes(country.Value));
                    this.Items.Add(new ListItem(countryName, country.Key));
                }
            }
            base.OnLoad(e);
        }
    }
}
