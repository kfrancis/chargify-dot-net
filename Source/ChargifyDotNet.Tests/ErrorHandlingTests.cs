using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RichardSzalay.MockHttp;
using Shouldly;

namespace ChargifyDotNet.Tests
{
    /// <summary>
    /// Mocked-HTTP tests for error diagnostics and HttpClient handling. No live Chargify calls.
    /// </summary>
    [TestClass]
    public class ErrorHandlingTests
    {
        private const string Site = "https://example.chargify.test";
        private const string CardNumber = "4111111111111111";

        private static ChargifyConnect CreateClient(MockHttpMessageHandler handler, bool useJson = false)
        {
            return new ChargifyConnect(Site, "api-key", "X", null, handler) { UseJSON = useJson };
        }

        private static HttpResponseMessage Respond(HttpStatusCode statusCode, string body, string mediaType)
        {
            return new HttpResponseMessage(statusCode) { Content = new StringContent(body, System.Text.Encoding.UTF8, mediaType) };
        }

        [TestMethod]
        public void Status422_WithXmlErrorsBody_PopulatesErrorMessagesAndResponseBody()
        {
            const string body = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><errors><error>Memo: cannot be blank.</error><error>Amount: must be greater than 0.</error></errors>";
            var handler = new MockHttpMessageHandler();
            handler.When(HttpMethod.Post, $"{Site}/subscriptions/123/charges.xml")
                .Respond(_ => Respond((HttpStatusCode)422, body, "application/xml"));
            var client = CreateClient(handler);
            client.ValidateSubscriptionBeforeCharge = false;

            var ex = Should.Throw<ChargifyException>(() => client.CreateCharge(123, 1m, "memo"));

            ((int)ex.StatusCode).ShouldBe(422);
            ex.ErrorMessages.Select(e => e.Message).ShouldBe(new[] { "Memo: cannot be blank.", "Amount: must be greater than 0." });
            ex.ResponseBody.ShouldBe(body);
            ex.ToString().ShouldContain("Memo: cannot be blank.");
            ex.Message.ShouldContain("Errors: Memo: cannot be blank.; Amount: must be greater than 0.");
            ex.ToString().ShouldContain("Response Body: ");
            ((int?)client.LastStatusCode).ShouldBe(422);
            client.LastResponseBody.ShouldBe(body);
        }

        [TestMethod]
        public void Status422_WithJsonArrayErrorsBody_PopulatesErrorMessages()
        {
            const string body = "{\"errors\":[\"Memo: cannot be blank.\",\"Amount: must be greater than 0.\"]}";
            var handler = new MockHttpMessageHandler();
            handler.When(HttpMethod.Post, $"{Site}/subscriptions/123/charges.json")
                .Respond(_ => Respond((HttpStatusCode)422, body, "application/json"));
            var client = CreateClient(handler, useJson: true);
            client.ValidateSubscriptionBeforeCharge = false;

            var ex = Should.Throw<ChargifyException>(() => client.CreateCharge(123, 1m, "memo"));

            ex.ErrorMessages.Select(e => e.Message).ShouldBe(new[] { "Memo: cannot be blank.", "Amount: must be greater than 0." });
            ex.ResponseBody.ShouldBe(body);
            ex.ToString().ShouldContain("Amount: must be greater than 0.");
        }

        [TestMethod]
        public void Status422_WithJsonFieldErrorsBody_PopulatesErrorMessagesWithField()
        {
            const string body = "{\"errors\":{\"amount\":[\"must be greater than 0\"],\"base\":[\"Subscription is canceled\"]}}";
            var handler = new MockHttpMessageHandler();
            handler.When(HttpMethod.Post, $"{Site}/subscriptions/123/charges.json")
                .Respond(_ => Respond((HttpStatusCode)422, body, "application/json"));
            var client = CreateClient(handler, useJson: true);
            client.ValidateSubscriptionBeforeCharge = false;

            var ex = Should.Throw<ChargifyException>(() => client.CreateCharge(123, 1m, "memo"));

            ex.ErrorMessages.Select(e => e.Message).ShouldBe(new[] { "amount: must be greater than 0", "Subscription is canceled" });
            ex.ErrorMessages[0].Field.ShouldBe("amount");
            ex.ToString().ShouldContain("amount: must be greater than 0");
        }

        [DataRow("")]
        [DataRow(" ")]
        [DataRow("not xml or json")]
        [DataRow("<html><body>Bad Gateway")]
        [DataRow("{\"errors\":")]
        [DataRow("{\"something_else\":true}")]
        [TestMethod]
        public void Status4xx_WithEmptyOrUnparseableBody_HasEmptyErrorList(string body)
        {
            var handler = new MockHttpMessageHandler();
            handler.When($"{Site}/subscriptions/123.xml")
                .Respond(_ => Respond(HttpStatusCode.BadRequest, body, "text/plain"));
            var client = CreateClient(handler);

            var ex = Should.Throw<ChargifyException>(() => client.LoadSubscription(123));

            ex.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            ex.ErrorMessages.ShouldBeEmpty();
            Should.NotThrow(() => ex.ToString());
        }

        [DataRow(null)]
        [DataRow("")]
        [DataRow("<errors>")]
        [DataRow("[1,")]
        [TestMethod]
        public void ParseErrorsFromContent_NeverThrows(string? body)
        {
            ChargifyError.ParseErrorsFromContent(body).ShouldBeEmpty();
        }

        [TestMethod]
        public void Status429_WithRetryAfter_PopulatesRetryAfterAndRateLimitHeaders()
        {
            var handler = new MockHttpMessageHandler();
            handler.When($"{Site}/subscriptions/123.xml").Respond(_ =>
            {
                var response = Respond((HttpStatusCode)429, "<errors><error>Too many requests</error></errors>", "application/xml");
                response.Headers.RetryAfter = new System.Net.Http.Headers.RetryConditionHeaderValue(TimeSpan.FromSeconds(30));
                response.Headers.Add("X-RateLimit-Limit", "100");
                response.Headers.Add("X-RateLimit-Remaining", "0");
                response.Headers.Add("Set-Cookie", "session=secret");
                return response;
            });
            var client = CreateClient(handler);

            var ex = Should.Throw<ChargifyException>(() => client.LoadSubscription(123));

            ((int)ex.StatusCode).ShouldBe(429);
            ex.RetryAfter.ShouldBe(TimeSpan.FromSeconds(30));
            ex.RateLimitHeaders["X-RateLimit-Limit"].ShouldBe("100");
            ex.RateLimitHeaders["X-RateLimit-Remaining"].ShouldBe("0");
            ex.ResponseHeaders.ContainsKey("Retry-After").ShouldBeTrue();
            ex.ResponseHeaders.ContainsKey("Set-Cookie").ShouldBeFalse();
        }

        [TestMethod]
        public void ResponseBody_IsTruncated()
        {
            var body = "<errors><error>" + new string('a', 10000) + "</error></errors>";
            var handler = new MockHttpMessageHandler();
            handler.When($"{Site}/subscriptions/123.xml").Respond(_ => Respond(HttpStatusCode.BadRequest, body, "application/xml"));
            var client = CreateClient(handler);

            var ex = Should.Throw<ChargifyException>(() => client.LoadSubscription(123));

            ex.ResponseBody!.Length.ShouldBeLessThan(ChargifyException.MaxResponseBodyLength + 50);
            ex.ResponseBody.ShouldEndWith("[truncated]");
            ex.ErrorMessages.Count.ShouldBe(1); // parsed from the full body, not the truncated one
        }

        [TestMethod]
        public void LogResponse_ReceivesResponseBodyOnFailure()
        {
            const string body = "<errors><error>Memo: cannot be blank.</error></errors>";
            var handler = new MockHttpMessageHandler();
            handler.When($"{Site}/subscriptions/123.xml").Respond(_ => Respond((HttpStatusCode)422, body, "application/xml"));
            var client = CreateClient(handler);
            var logged = new List<(HttpStatusCode Status, string Body)>();
            client.LogResponse = (status, _, logBody) => logged.Add((status, logBody));

            Should.Throw<ChargifyException>(() => client.LoadSubscription(123));

            logged.Count.ShouldBe(1);
            ((int)logged[0].Status).ShouldBe(422);
            logged[0].Body.ShouldBe(body);
        }

        [TestMethod]
        public void FileRequest_NonSuccess_PreservesStatusCodeAndBody()
        {
            const string body = "<errors><error>Statement is not available</error></errors>";
            var handler = new MockHttpMessageHandler();
            handler.When($"{Site}/statements/55.pdf").Respond(_ => Respond(HttpStatusCode.Forbidden, body, "application/xml"));
            var client = CreateClient(handler);

            var ex = Should.Throw<ChargifyException>(() => client.LoadStatementPDF(55));

            ex.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
            ex.ResponseBody.ShouldBe(body);
            ex.ErrorMessages.Single().Message.ShouldBe("Statement is not available");
            client.LastStatusCode.ShouldBe(HttpStatusCode.Forbidden);
        }

        [TestMethod]
        public void FileRequest_NotFound_StillReturnsNull()
        {
            var handler = new MockHttpMessageHandler();
            handler.When($"{Site}/statements/55.pdf").Respond(HttpStatusCode.NotFound);
            var client = CreateClient(handler);

            client.LoadStatementPDF(55).ShouldBeNull();
            client.LastStatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void LastStatusCodeAndBody_AreSetOnSuccess()
        {
            const string body = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><charge><id>1</id><amount_in_cents>100</amount_in_cents><success type=\"boolean\">true</success></charge>";
            var handler = new MockHttpMessageHandler();
            handler.When(HttpMethod.Post, $"{Site}/subscriptions/123/charges.xml").Respond(_ => Respond(HttpStatusCode.Created, body, "application/xml"));
            var client = CreateClient(handler);
            client.ValidateSubscriptionBeforeCharge = false;

            client.CreateCharge(123, 1m, "memo");

            client.LastStatusCode.ShouldBe(HttpStatusCode.Created);
            client.LastResponseBody.ShouldBe(body);
#pragma warning disable CS0618 // Type or member is obsolete
            client.LastResponse.ShouldBeNull();
#pragma warning restore CS0618
        }

        [TestMethod]
        public void InjectedHandler_IsUsed_WithAuthAndAcceptHeaders()
        {
            var handler = new MockHttpMessageHandler();
            var expectedAuth = "Basic " + Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes("api-key:X"));
            handler.Expect(HttpMethod.Get, $"{Site}/subscriptions/123.xml")
                .WithHeaders("Authorization", expectedAuth)
                .WithHeaders("Accept", "application/xml")
                .Respond(HttpStatusCode.NotFound);

            var client = CreateClient(handler);

            client.LoadSubscription(123).ShouldBeNull();
            handler.VerifyNoOutstandingExpectation();
        }

        [TestMethod]
        public void InjectedHttpClient_IsUsed()
        {
            var handler = new MockHttpMessageHandler();
            handler.Expect($"{Site}/subscriptions/123.xml").Respond(HttpStatusCode.NotFound);
            using var httpClient = handler.ToHttpClient();
            var client = new ChargifyConnect(Site, "api-key", "X", null, httpClient);

            client.LoadSubscription(123).ShouldBeNull();
            handler.VerifyNoOutstandingExpectation();
            client.GetHttpClient().ShouldBeSameAs(httpClient);
        }

        [TestMethod]
        public void HttpClient_IsReusedAcrossCalls()
        {
            var handler = new MockHttpMessageHandler();
            handler.When($"{Site}/subscriptions/*").Respond(HttpStatusCode.NotFound);
            var client = CreateClient(handler);

            client.LoadSubscription(1);
            var first = client.GetHttpClient();
            client.LoadSubscription(2);
            client.LoadSubscription(3);

            client.GetHttpClient().ShouldBeSameAs(first);
        }

        [TestMethod]
        public void OwnedHttpClient_IsReused_AndRebuiltWhenProtocolTypeChanges()
        {
            var client = new ChargifyConnect(Site, "api-key", "X");

            var first = client.GetHttpClient();
            client.GetHttpClient().ShouldBeSameAs(first);

            client.ProtocolType = SecurityProtocolType.Tls12;
            var second = client.GetHttpClient();
            second.ShouldNotBeSameAs(first);
            client.GetHttpClient().ShouldBeSameAs(second);

            client.ProtocolType = SecurityProtocolType.Tls12; // unchanged value keeps the client
            client.GetHttpClient().ShouldBeSameAs(second);
        }

        [TestMethod]
        public void FullNumber_IsMasked_InLastDataPostedAndToString()
        {
            var handler = new MockHttpMessageHandler();
            handler.When(HttpMethod.Get, $"{Site}/customers/42.xml")
                .Respond(_ => Respond(HttpStatusCode.OK, "<?xml version=\"1.0\" encoding=\"UTF-8\"?><customer><id>42</id></customer>", "application/xml"));
            handler.When(HttpMethod.Post, $"{Site}/subscriptions.xml")
                .Respond(_ => Respond((HttpStatusCode)422,
                    $"<errors><error>Credit card number is invalid</error><full_number>{CardNumber}</full_number></errors>", "application/xml"));
            var client = CreateClient(handler);
            var card = new CreditCardAttributes("First", "Last", CardNumber, DateTime.Now.Year + 2, 12, "123",
                "1 Street", "City", "ON", "K1A0A1", "CA");

            var ex = Should.Throw<ChargifyException>(() => client.CreateSubscription("basic", 42L, card));

            ex.LastDataPosted.ShouldNotBeNull();
            ex.LastDataPosted.ShouldNotContain(CardNumber);
            ex.LastDataPosted.ShouldContain("<full_number>XXXXXXXXXXXX1111</full_number>");
            ex.LastDataPosted.ShouldNotContain("<cvv>123</cvv>");
            ex.ToString().ShouldNotContain(CardNumber);
            ex.ResponseBody.ShouldNotBeNull();
            ex.ResponseBody.ShouldNotContain(CardNumber);
        }

        [TestMethod]
        public void CreateCharge_ByDefault_LoadsSubscriptionFirst_AndKeepsUseNegativeBalanceWireValue()
        {
            var handler = new MockHttpMessageHandler();
            handler.Expect(HttpMethod.Get, $"{Site}/subscriptions/123.xml")
                .Respond(_ => Respond(HttpStatusCode.OK, "<?xml version=\"1.0\" encoding=\"UTF-8\"?><subscription><id>123</id></subscription>", "application/xml"));
            handler.Expect(HttpMethod.Post, $"{Site}/subscriptions/123/charges.xml")
                .WithPartialContent("<use_negative_balance>1</use_negative_balance>")
                .Respond(_ => Respond(HttpStatusCode.Created, "<?xml version=\"1.0\" encoding=\"UTF-8\"?><charge><id>1</id></charge>", "application/xml"));
            var client = CreateClient(handler);

            client.CreateCharge(123, 1m, "memo");

            handler.VerifyNoOutstandingExpectation();
        }

        [TestMethod]
        public void CreateCharge_WithValidationDisabled_SkipsSubscriptionLookup()
        {
            var handler = new MockHttpMessageHandler();
            var get = handler.When(HttpMethod.Get, $"{Site}/subscriptions/123.xml").Respond(HttpStatusCode.OK);
            handler.Expect(HttpMethod.Post, $"{Site}/subscriptions/123/charges.xml")
                .WithPartialContent("<use_negative_balance>0</use_negative_balance>")
                .Respond(_ => Respond(HttpStatusCode.Created, "<?xml version=\"1.0\" encoding=\"UTF-8\"?><charge><id>1</id></charge>", "application/xml"));
            var client = CreateClient(handler);
            client.ValidateSubscriptionBeforeCharge = false;

            client.CreateCharge(123, 1m, "memo", useNegativeBalance: true);

            handler.VerifyNoOutstandingExpectation();
            handler.GetMatchCount(get).ShouldBe(0);
        }
    }
}
