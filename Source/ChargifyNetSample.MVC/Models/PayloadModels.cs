using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChargifyNetSample.MVC.Models
{
    public class WebhookCustomer
    {
        public string last_name { get; set; }
        public string reference { get; set; }
        public string phone { get; set; }
        public string first_name { get; set; }
        public string zip { get; set; }
        public string address_2 { get; set; }
        public string address { get; set; }
        public string state { get; set; }
        public string updated_at { get; set; }
        public string created_at { get; set; }
        public string organization { get; set; }
        public string id { get; set; }
        public string email { get; set; }
        public string country { get; set; }
        public string city { get; set; }
    }

    public class WebhookProductFamily
    {
        public string handle { get; set; }
        public string accounting_code { get; set; }
        public string name { get; set; }
        public string id { get; set; }
        public string description { get; set; }
        public int version_number { get; set; }
    }

    public class WebhookProduct
    {
        public string handle { get; set; }
        public string expiration_interval_unit { get; set; }
        public string archived_at { get; set; }
        public string interval_unit { get; set; }
        public WebhookProductFamily product_family { get; set; }
        public string accounting_code { get; set; }
        public string price_in_cents { get; set; }
        public string expiration_interval { get; set; }
        public string name { get; set; }
        public string trial_interval_unit { get; set; }
        public string interval { get; set; }
        public string return_params { get; set; }
        public string updated_at { get; set; }
        public string created_at { get; set; }
        public string id { get; set; }
        public string return_url { get; set; }
        public string trial_interval { get; set; }
        public string description { get; set; }
        public string trial_price_in_cents { get; set; }
        public string update_return_url { get; set; }
        public string initial_charge_in_cents { get; set; }
        public string require_credit_card { get; set; }
        public string request_credit_card { get; set; }
    }

    public class WebhookCreditCard
    {
        public string last_name { get; set; }
        public string card_type { get; set; }
        public string vault_token { get; set; }
        public string first_name { get; set; }
        public string billing_state { get; set; }
        public string masked_card_number { get; set; }
        public string expiration_year { get; set; }
        public string billing_country { get; set; }
        public string customer_vault_token { get; set; }
        public string customer_id { get; set; }
        public string billing_address { get; set; }
        public string expiration_month { get; set; }
        public string current_vault { get; set; }
        public string billing_city { get; set; }
        public string id { get; set; }
        public string billing_address_2 { get; set; }
        public string billing_zip { get; set; }
    }

    public class WebhookSubscription
    {
        public string cancellation_message { get; set; }
        public WebhookCustomer customer { get; set; }
        public string trial_started_at { get; set; }
        public WebhookProduct product { get; set; }
        public string trial_ended_at { get; set; }
        public string previous_state { get; set; }
        public string expires_at { get; set; }
        public string coupon_code { get; set; }
        public string current_period_started_at { get; set; }
        public string canceled_at { get; set; }
        public string next_assessment_at { get; set; }
        public WebhookCreditCard credit_card { get; set; }
        public string signup_revenue { get; set; }
        public string signup_payment_id { get; set; }
        public string delayed_cancel_at { get; set; }
        public string state { get; set; }
        public string cancel_at_end_of_period { get; set; }
        public string updated_at { get; set; }
        public string created_at { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string activated_at { get; set; }
        public string current_period_ends_at { get; set; }
        public string balance_in_cents { get; set; }
        public string total_revenue_in_cents { get; set; }
    }

    public class WebhookComponent
    {
        public string kind { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string unit_name { get; set; }
    }

    public class WebhookTransaction
    {
        public string amount_in_cents { get; set; }
        public string created_at { get; set; }
        public string ending_balance_in_cents { get; set; }
        public string id { get; set; }
        public string kind { get; set; }
        public string memo { get; set; }
        public string payment_id { get; set; }
        public string product_id { get; set; }
        public string starting_balance_in_cents { get; set; }
        public string subscription_id { get; set; }
        public string success { get; set; }
        public string type { get; set; }
        public string transaction_type { get; set; }
        public string gateway_transaction_id { get; set; }
        public string gateway_order_id { get; set; }
        public string component_id { get; set; }
        public string tax_id { get; set; }
        public string statement_id { get; set; }
        public string card_number { get; set; }
        public string card_expiration { get; set; }
        public string card_type { get; set; }
    }

    public class WebhookEvent
    {
        public string id { get; set; }
        public string key { get; set; }
        public string message { get; set; }
    }

    public class WebhookStatement
    {
        public string closed_at { get; set; }
        public string created_at { get; set; }
        public string id { get; set; }
        public string opened_at { get; set; }
        public string settled_at { get; set; }
        public string subscription_id { get; set; }
        public string updated_at { get; set; }
        public string starting_balance_in_cents { get; set; }
        public string ending_balance_in_cents { get; set; }
        public string memo { get; set; }
        public List<WebhookEvent> events { get; set; }
        public List<WebhookTransaction> transactions { get; set; }
    }

    public class WebhookSite
    {
        public string subdomain { get; set; }
        public string id { get; set; }
    }

    public class WebhookPayload
    {
        public WebhookSubscription subscription { get; set; }
        public WebhookSite site { get; set; }
        public WebhookTransaction transaction { get; set; }
        public WebhookProduct previous_product { get; set; }
        public WebhookProduct product { get; set; }
        public WebhookProductFamily product_family { get; set; }
        public WebhookCreditCard previous_payment_profile { get; set; }
        public WebhookCreditCard updated_payment_profile { get; set; }
        public WebhookCustomer customer { get; set; }
        public WebhookStatement statement { get; set; }
        public WebhookCreditCard payment_profile { get; set; }
        public string email_sent { get; set; }
        public string estimated_renewal_amount_in_cents { get; set; }
        public string message { get; set; }
        public WebhookComponent component { get; set; }
        public string usage_quantity { get; set; }
        public string new_allocation { get; set; }
        public string new_unit_balance { get; set; }
        public string previous_allocation { get; set; }
        public string previous_unit_balance { get; set; }
        public string memo { get; set; }
        public string timestamp { get; set; }

        /// <summary>
        /// Used for test webhook events (only)
        /// </summary>
        public string chargify { get; set; }
    }

    public class WebhookEventData
    {
        public string @event { get; set; }
        public string id { get; set; }
        public WebhookPayload payload { get; set; }
    }
}