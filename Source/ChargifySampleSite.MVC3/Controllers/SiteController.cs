using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChargifyNET;
using ChargifyNET.Configuration;
using System.Configuration;
using ChargifySampleSite.MVC3.Models;
using System.Web.Routing;
using System.Web.Security;

namespace ChargifySampleSite.MVC3.Controllers
{
    public class SiteController : Controller
    {

        public IMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {            
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        #region Helpers

        private ChargifyConnect Chargify
        {
            get
            {
                if (HttpContext.Cache["Chargify"] == null)
                {
                    ChargifyAccountRetrieverSection config = ConfigurationManager.GetSection("chargify") as ChargifyAccountRetrieverSection;
                    ChargifyAccountElement accountInfo = config.GetDefaultOrFirst();
                    ChargifyConnect chargify = new ChargifyConnect();
                    chargify.apiKey = accountInfo.ApiKey;
                    chargify.Password = accountInfo.ApiPassword;
                    chargify.URL = accountInfo.Site;
                    chargify.SharedKey = accountInfo.SharedKey;
                    chargify.UseJSON = config.UseJSON;

                    HttpContext.Cache.Add("Chargify", chargify, null, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, null);
                }

                return HttpContext.Cache["Chargify"] as ChargifyConnect;
            }
        }

        #endregion

        //
        // GET: /Site/
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.HasSubscription = false;
            ISubscription model = null;
            ICustomer c = Chargify.Find<Customer>(MembershipService.GetUser().ProviderUserKey.ToString());
            if (c != null)
            { 
                IDictionary<int, ISubscription> sList = Chargify.GetSubscriptionListForCustomer(c.ChargifyID);
                if (sList.Count > 0)
                {
                    ViewBag.HasSubscription = true;
                    model = sList.FirstOrDefault().Value;
                }
            }

            return View(model);
        }

        public ActionResult Settings()
        {
            return View();
        }
        
        public ActionResult Stats()
        {
            var model = Chargify.GetSiteStatistics();
            return View(model);
        }
    }
}
