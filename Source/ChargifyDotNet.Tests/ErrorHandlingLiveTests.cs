using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace ChargifyDotNet.Tests
{
    /// <summary>
    /// Live tests (against the site in appsettings.json) for the 1.5.5 error diagnostics and HttpClient changes.
    /// None of these create a successful charge.
    /// </summary>
    [TestClass]
    public class ErrorHandlingLiveTests : ChargifyTestBase
    {
        private const int MissingId = 999999999;

        private ISubscription GetActiveSubscription()
        {
            var subscription = Chargify.GetSubscriptionList(SubscriptionState.Active).Values.FirstOrDefault();
            if (subscription == null) Assert.Inconclusive("An active subscription could not be found.");
            return subscription!;
        }

        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Live_Success_SetsLastStatusCodeAndBody(string method)
        {
            SetJson(method == "json");

            var products = Chargify.GetProductList();

            products.ShouldNotBeNull();
            Chargify.LastStatusCode.ShouldBe(HttpStatusCode.OK);
            Chargify.LastResponseBody.ShouldNotBeNullOrWhiteSpace();
            Chargify.LastResponseBody!.TrimStart().ShouldStartWith(method == "json" ? "[" : "<");
        }

        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Live_NotFound_SetsLastStatusCode(string method)
        {
            SetJson(method == "json");

            Chargify.LoadSubscription(MissingId).ShouldBeNull();

            Chargify.LastStatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Live_422_PopulatesErrorMessagesResponseBodyAndMessage(string method)
        {
            SetJson(method == "json");
            var subscription = GetActiveSubscription();
            var options = new ChargeCreateOptions { Amount = -1m, Memo = "chargify-dot-net negative amount test" };

            var ex = Should.Throw<ChargifyException>(() => Chargify.CreateCharge(subscription.SubscriptionID, options));
            TestContext.WriteLine(ex.ToString());

            ((int)ex.StatusCode).ShouldBe(422);
            ex.ErrorMessages.ShouldNotBeEmpty();
            ex.ResponseBody.ShouldNotBeNullOrWhiteSpace();
            ex.Message.ShouldContain(ex.ErrorMessages[0].Message);
            ex.ToString().ShouldContain(ex.ErrorMessages[0].Message);
            ex.ToString().ShouldContain("Response Body: ");
            ((int?)Chargify.LastStatusCode).ShouldBe(422);
            Chargify.LastResponseBody.ShouldNotBeNullOrWhiteSpace();
        }

        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Live_LogResponse_ReceivesBodyOnFailure(string method)
        {
            SetJson(method == "json");
            var subscription = GetActiveSubscription();
            string? loggedBody = null;
            HttpStatusCode? loggedStatus = null;
            Chargify.LogResponse = (status, _, body) => { loggedStatus = status; loggedBody = body; };
            try
            {
                var ex = Should.Throw<ChargifyException>(() =>
                    Chargify.CreateCharge(subscription.SubscriptionID, new ChargeCreateOptions { Amount = -1m, Memo = "log test" }));

                ((int?)loggedStatus).ShouldBe(422);
                loggedBody.ShouldNotBeNullOrWhiteSpace();
                loggedBody.ShouldContain(ex.ErrorMessages[0].Message);
                loggedBody.ShouldNotContain("Request: "); // body, not ChargifyException.ToString()
            }
            finally
            {
                Chargify.LogResponse = null;
            }
        }

        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Live_InvalidCard_MasksFullNumber(string method)
        {
            SetJson(method == "json");
            var product = Chargify.GetProductList().Values.FirstOrDefault();
            if (product == null) Assert.Inconclusive("No product found.");
            // Fails Luhn validation, so Chargify returns 422 and no subscription is created.
            const string invalidCard = "4111111111111112";
            var customer = new CustomerAttributes("Decline", "Test", $"decline.{Guid.NewGuid():N}@example.com", "Chargify.NET", Guid.NewGuid().ToString());
            var card = new CreditCardAttributes("Decline", "Test", invalidCard, DateTime.Now.Year + 2, 12, "123",
                "1 Street", "Ottawa", "ON", "K1A0A1", "CA");

            var ex = Should.Throw<ChargifyException>(() => Chargify.CreateSubscription(product!.Handle, customer, card));
            TestContext.WriteLine(ex.ToString());

            ((int)ex.StatusCode).ShouldBe(422);
            ex.ErrorMessages.ShouldNotBeEmpty();
            ex.LastDataPosted.ShouldNotBeNull();
            ex.LastDataPosted.ShouldNotContain(invalidCard);
            ex.LastDataPosted.ShouldContain("XXXXXXXXXXXX1112");
            ex.LastDataPosted.ShouldNotContain("<cvv>123</cvv>");
            ex.ToString().ShouldNotContain(invalidCard);
            ex.ResponseBody?.ShouldNotContain(invalidCard);
        }

        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Live_FileRequest_NotFound_ReturnsNullAndSetsStatus(string method)
        {
            SetJson(method == "json");

            Chargify.LoadStatementPDF(MissingId).ShouldBeNull();

            Chargify.LastStatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void Live_CreateCharge_DefaultValidation_ThrowsArgumentExceptionForMissingSubscription()
        {
            Should.Throw<ArgumentException>(() => Chargify.CreateCharge(MissingId, 1m, "should not post"));
            Chargify.LastStatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void Live_CreateCharge_WithoutValidation_PostsDirectlyAndSurfaces404()
        {
            var client = new ChargifyConnect(Chargify.URL, Chargify.apiKey, Chargify.Password)
            {
                ValidateSubscriptionBeforeCharge = false
            };
            var requests = 0;
            client.LogRequest = (verb, _, _) => { requests++; verb.ShouldBe(HttpRequestMethod.Post); };

            var ex = Should.Throw<ChargifyException>(() => client.CreateCharge(MissingId, 1m, "should not exist"));

            ex.StatusCode.ShouldBe(HttpStatusCode.NotFound);
            requests.ShouldBe(1);
        }

        [TestMethod]
        public void Live_HttpClient_IsReusedAcrossCalls()
        {
            Chargify.GetProductList();
            var first = Chargify.GetHttpClient();
            Chargify.LoadSubscription(MissingId);
            Chargify.GetProductFamilyList();

            Chargify.GetHttpClient().ShouldBeSameAs(first);
        }

        [TestMethod]
        public void Live_InjectedHttpClient_Works()
        {
            using var httpClient = new HttpClient();
            var client = new ChargifyConnect(Chargify.URL, Chargify.apiKey, Chargify.Password, null, httpClient);

            client.GetProductList().ShouldNotBeNull();

            client.LastStatusCode.ShouldBe(HttpStatusCode.OK);
            client.GetHttpClient().ShouldBeSameAs(httpClient);
        }

        [TestMethod]
        public void Live_InjectedHandler_Works()
        {
            using var handler = new HttpClientHandler();
            var client = new ChargifyConnect(Chargify.URL, Chargify.apiKey, Chargify.Password, null, handler);

            client.GetProductList().ShouldNotBeNull();
            client.LoadSubscription(MissingId).ShouldBeNull();

            client.LastStatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void Live_Timeout_CanChangeAfterFirstRequest()
        {
            var client = new ChargifyConnect(Chargify.URL, Chargify.apiKey, Chargify.Password);
            client.GetProductList();

            client.Timeout = 1; // would throw InvalidOperationException on a shared HttpClient.Timeout

            Should.Throw<OperationCanceledException>(() => client.GetProductList());
            client.Timeout = 180000;
            client.GetProductList().ShouldNotBeNull();
        }
    }
}
