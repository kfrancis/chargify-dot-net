using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ChargifyNET.Configuration;
using System.Configuration;
using ChargifyNET;

namespace ChargifyAdminApp
{
    public partial class SiteSettings : Form
    {
        #region Chargify
        /// <summary>
        /// The Chargify config section
        /// </summary>
        private ChargifyAccountRetrieverSection _config = ConfigurationManager.GetSection("chargify") as ChargifyAccountRetrieverSection;

        /// <summary>
        /// The ChargifyConnect object that allows you to make API calls via Chargify.NET
        /// </summary>
        protected ChargifyConnect Chargify
        {
            get
            {
                if (this._chargify == null)
                {
                    // Get the account info ..
                    ChargifyAccountElement accountInfo = this._config.GetDefaultOrFirst();
                    this._chargify = new ChargifyConnect();
                    this._chargify.apiKey = accountInfo.ApiKey;
                    this._chargify.Password = accountInfo.ApiPassword;
                    this._chargify.URL = accountInfo.Site;
                    this._chargify.SharedKey = accountInfo.SharedKey;
                }
                return this._chargify;
            }
        }
        private ChargifyConnect _chargify = null;
        #endregion


        public SiteSettings()
        {
            InitializeComponent();
        }
    }
}
