using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChargifySampleSite.MVC3.Models;
using ChargifyNET;
using ChargifyNET.Configuration;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Web.Security;
using System.Web.Routing;

namespace ChargifySampleSite.MVC3.Controllers
{
    public class HomeController : Controller
    {
        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }
        public IRoleService RoleService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }
            if (RoleService == null) { RoleService = new AccountRoleService(); }

            base.Initialize(requestContext);
        }

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

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Plans()
        {
            return View();
        }

        public ActionResult SubscribeRemote(string id)
        {
            // This determines if the _CreditCard partial gets rendered
            IProduct productResult = (from a in Chargify.GetProductList().Values
                                      where a.Handle == id
                                      select a).SingleOrDefault();

            // Shouldn't be here if there isn't any plan (ie. ID is nothing)
            if (productResult == null) { RedirectToAction("Plans"); }

            // Adjust the name of the plan for viewing (capitalize the first letter)
            ViewBag.PlanName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(id);

            return View();
        }

        [HttpPost]
        public ActionResult SubscribeRemote(string id, RemoteSubscriptionViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                MembershipUser user = MembershipService.CreateUser(model.Username, model.Password, model.Email, model.Question, model.Answer, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    IProduct productResult = (from a in Chargify.GetProductList().Values
                                              where a.Handle == id
                                              select a).SingleOrDefault();

                    string hostedPageUrl = Chargify.URL;
                    hostedPageUrl += "h/" + productResult.ID + "/subscriptions/new?reference=" + user.ProviderUserKey.ToString() + "&email=" + model.Email;
                    return Redirect(hostedPageUrl);
                }
                else
                {
                    ModelState.AddModelError("", AccountValidation.ErrorCodeToString(createStatus));
                }

                return RedirectToAction("SubscribeRemote", new { id = id });   
            }
            else
            {
                return RedirectToAction("SubscribeRemote", new { id = id });
            }
        }

        public ActionResult Subscribe(string id)
        {
            // This determines if the _CreditCard partial gets rendered
            IProduct productResult = (from a in Chargify.GetProductList().Values
                                      where a.Handle == id
                                      select a).SingleOrDefault();

            if (productResult == null)
            {
                RedirectToAction("Plans");
            }

            // Adjust the name of the plan for viewing (capitalize the first letter)
            ViewBag.PlanName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(id);

            List<SelectListItem> provinces = new List<SelectListItem>
            {
                new SelectListItem { Text = "Alberta", Value = "AB" },
                new SelectListItem { Text = "British Columbia", Value = "BC" },
                new SelectListItem { Text = "Manitoba", Value = "MB" },
                new SelectListItem { Text = "Newfoundland and Labrador", Value = "NL" },
                new SelectListItem { Text = "New Brunswick", Value = "NB" },
                new SelectListItem { Text = "Northwest Territories", Value = "NT" },
                new SelectListItem { Text = "Nova Scotia", Value = "NS" },
                new SelectListItem { Text = "Ontario", Value = "ON" },
                new SelectListItem { Text = "Prince Edward Island", Value = "PE" },
                new SelectListItem { Text = "Saskatchewan", Value = "SK" },
                new SelectListItem { Text = "Nunavut", Value = "NU" },
                new SelectListItem { Text = "Yukon", Value = "YT" }
            };
            ViewBag.Provinces = new SelectList(provinces, "Value", "Text");
            ViewBag.RequireCreditCard = productResult.RequireCreditCard;
            
            // Create the next 10 years for the credit card expiration
            List<SelectListItem> expYears = new List<SelectListItem>();
            for (int i = 0; i <= 10; i++)
            {
                string year = (DateTime.Today.Year + i).ToString();
                expYears.Add(new SelectListItem { Text = year, Value = year });
            }
            ViewBag.ExpYears = new SelectList(expYears, "Value", "Text");

            IEnumerable<SelectListItem> expMonths = DateTimeFormatInfo.InvariantInfo.MonthNames.Select((monthName, index) => new SelectListItem
                                                                                       {
                                                                                           Value = (index + 1).ToString(),
                                                                                           Text = monthName
                                                                                       });
            ViewBag.ExpMonths = new SelectList(expMonths, "Value", "Text");

            return View();
        }

        [HttpPost]
        public ActionResult Subscribe(string id, LocalSubscriptionViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                MembershipUser user = MembershipService.CreateUser(model.Username, model.Password, model.Email, model.Question, model.Answer, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    if (!RoleService.RoleExists("user")) {
                        RoleService.CreateRole("user");
                    }
                    RoleService.AddUsersToRoles(new string[] {user.UserName }, new string[] {"User"});

                    // Now that the user is created, attempt to create the corresponding subscription
                    CustomerAttributes cInfo = new CustomerAttributes();
                    cInfo.FirstName = model.FirstName;
                    cInfo.LastName = model.LastName;
                    cInfo.Email = model.Email;
                    cInfo.SystemID = user.ProviderUserKey.ToString();

                    CreditCardAttributes ccAttr = new CreditCardAttributes();
                    ccAttr.FullNumber = model.CardNumber.Trim();
                    ccAttr.CVV = model.CVV;
                    ccAttr.ExpirationMonth = model.ExpMonth;
                    ccAttr.ExpirationYear = model.ExpYear;

                    ccAttr.BillingAddress = model.Address;
                    ccAttr.BillingCity = model.City;
                    ccAttr.BillingZip = model.PostalCode;
                    ccAttr.BillingState = model.Province;
                    ccAttr.BillingCountry = "CA";

                    try
                    {
                        ISubscription newSubscription = Chargify.CreateSubscription(id, cInfo, ccAttr);

                        FormsService.SignIn(model.Username, false /* createPersistentCookie */);
                        return RedirectToAction("Index", "Site");
                    }
                    catch (ChargifyException ex)
                    {
                        if (ex.ErrorMessages.Count > 0)
                        {
                            ModelState.AddModelError("", ex.ErrorMessages.FirstOrDefault().Message);
                        }
                        else
                        {
                            ModelState.AddModelError("", ex.ToString());
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", AccountValidation.ErrorCodeToString(createStatus));
                }

                return RedirectToAction("Subscribe");
            }
            else
            {
                return RedirectToAction("Subscribe", new { id = id });
            }
        }
    }
}
