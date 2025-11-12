using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChargifyDotNet;
using System;

namespace ChargifyDotNet.Tests
{
    [TestClass]
    public class HostedPageUrlTests : ChargifyTestBase
    {
        [TestMethod]
        public void GetPrettySubscriptionUpdateURL_WithNames_GeneratesExpectedUrl()
        {
            // Arrange
            var subscriptionId = 42;
            var firstName = "Jöhn";          // includes diacritic
            var lastName = "D'oe Smith!";    // includes punctuation and space
            var expectedToken = ($"update_payment--{subscriptionId}--{Chargify.SharedKey}").GetChargifyHostedToken();
            var expectedPrettyId = $"{subscriptionId}-john-d-oe-smith"; // slugged + lowercased
            var expectedBase = Chargify.URL.Replace("chargify.com", "chargifypay.com").TrimEnd('/');
            var expected = $"{expectedBase}/update_payment/{expectedPrettyId}/{expectedToken}";

            // Act
            var actual = Chargify.GetPrettySubscriptionUpdateURL(firstName, lastName, subscriptionId);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetPrettySubscriptionUpdateURL_TitleAndPunctuation_GeneratesExpectedUrl()
        {
            // Arrange
            var subscriptionId = 123;
            var firstName = "Dr. David J."; // title, spaces, periods
            var lastName = "Morrow";        // simple last name
            var expectedToken = ($"update_payment--{subscriptionId}--{Chargify.SharedKey}").GetChargifyHostedToken();
            // Slug steps: non-alnum -> '-', collapse, trim, lowercase
            // "Dr. David J." -> "dr-david-j"
            // "Morrow" -> "morrow"
            var expectedPrettyId = $"{subscriptionId}-dr-david-j-morrow";
            var expectedBase = Chargify.URL.Replace("chargify.com", "chargifypay.com").TrimEnd('/');
            var expected = $"{expectedBase}/update_payment/{expectedPrettyId}/{expectedToken}";

            // Act
            var actual = Chargify.GetPrettySubscriptionUpdateURL(firstName, lastName, subscriptionId);

            // Assert
            Assert.AreEqual(expected, actual);

            Log("URL: " + actual);
        }

        [TestMethod]
        public void GetPrettySubscriptionUpdateURL_EmptyNames_UsesOnlySubscriptionId()
        {
            // Arrange
            var subscriptionId = 7;
            var expectedToken = ($"update_payment--{subscriptionId}--{Chargify.SharedKey}").GetChargifyHostedToken();
            var expectedBase = Chargify.URL.Replace("chargify.com", "chargifypay.com").TrimEnd('/');
            var expected = $"{expectedBase}/update_payment/{subscriptionId}/{expectedToken}";

            // Act
            var actual = Chargify.GetPrettySubscriptionUpdateURL(string.Empty, null, subscriptionId);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetPrettySubscriptionUpdateURL_NoSharedKey_Throws()
        {
            // Arrange
            var connect = new ChargifyConnect { URL = Chargify.URL, SharedKey = null };

            // Act
            var threw = false;
            try
            {
                connect.GetPrettySubscriptionUpdateURL("A", "B", 1);
            }
            catch (ArgumentException ex)
            {
                threw = ex.Message.IndexOf("SharedKey is required", StringComparison.InvariantCultureIgnoreCase) >= 0;
            }

            // Assert
            Assert.IsTrue(threw, "Expected ArgumentException about missing SharedKey");
        }
    }
}
