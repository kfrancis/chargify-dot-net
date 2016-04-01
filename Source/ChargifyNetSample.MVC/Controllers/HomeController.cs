using ChargifyNetSample.MVC.Models;
using System.Net;
using System.Web.Mvc;
using ChargifyNET;
using ChargifyNET.Configuration;
using System.Configuration;

namespace ChargifyNetSample.MVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// POST: /home/webhook
        /// </summary>
        /// <param name="model">The webhook event payload</param>
        /// <param name="signature_hmac_sha_256"></param>
        /// <returns>Applicable HttpStatusCode result</returns>
        [HttpPost]
        public ActionResult Webhook(WebhookEventData model, string signature_hmac_sha_256)
        {
            ChargifyAccountRetrieverSection config = ConfigurationManager.GetSection("chargify") as ChargifyAccountRetrieverSection;
            ChargifyAccountElement accountInfo = config.GetDefaultOrFirst();
            var sharedKey = accountInfo.SharedKey;
            
            var signatureHeaderHandle = "X-Chargify-Webhook-Signature-Hmac-Sha-256";
            string signature = !string.IsNullOrEmpty(signature_hmac_sha_256) ? signature_hmac_sha_256 : this.Request.Headers[signatureHeaderHandle]; // Try and get the signature passed in the request header
            var isRequestValid = this.Request.InputStream.IsWebhookRequestValid(sharedKey, signature);

            if (!isRequestValid) { return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable, "Signature mismatch"); }

            switch (model.@event)
            {
                case "signup_success":
                    break;
                case "signup_failure":
                    break;
                case "renewal_success":
                    break;
                case "renewal_failure":
                    break;
                case "payment_success":
                    break;
                case "payment_failure":
                    break;
                case "billing_date_change":
                    break;
                case "subscription_state_change":
                    break;
                case "subscription_product_change":
                    break;
                case "subscription_card_update":
                    break;
                case "expiring_card":
                    break;
                case "customer_update":
                    break;
                case "component_allocation_change":
                    break;
                case "metered_usage":
                    break;
                case "upgrade_downgrade_sucess":
                    break;
                case "upgrade_downgrade_failure":
                    break;
                case "refund_success":
                    break;
                case "refund_failure":
                    break;
                case "upcoming_renewal_notice":
                    break;
                case "end_of_trial_notice":
                    break;
                case "statement_closed":
                    break;
                case "statement_settled":
                    break;
                case "expiration_date_change":
                    break;
                default:
                    break;
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}