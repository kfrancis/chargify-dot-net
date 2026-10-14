using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace ChargifyDotNet.Tests
{
    /// <summary>
    /// Webhook signature verification (no network). Expected signatures are computed independently here.
    /// </summary>
    [TestClass]
    public class WebhookSignatureTests
    {
        private const string SharedKey = "test-shared-key";
        private const string Body = "id=123&event=signup_success&payload[subscription][id]=456";

        private static string Hex(byte[] bytes) => string.Concat(bytes.Select(b => b.ToString("x2")));

        private static string HmacUtf8(string text, string key)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            return Hex(hmac.ComputeHash(Encoding.UTF8.GetBytes(text)));
        }

        private static string HmacAscii(string text, string key)
        {
            using var hmac = new HMACSHA256(Encoding.ASCII.GetBytes(key));
            return Hex(hmac.ComputeHash(Encoding.ASCII.GetBytes(text)));
        }

        private static string Md5(string text, string key)
        {
            using var md5 = MD5.Create();
            return Hex(md5.ComputeHash(Encoding.UTF8.GetBytes(key + text)));
        }

        private static string FlipLastChar(string hex) => hex.Substring(0, hex.Length - 1) + (hex[hex.Length - 1] == '0' ? '1' : '0');

        // --- IsChargifyWebhookContentValidHMAC ---

        [TestMethod]
        public void Hmac_ValidSignature_LowerAndUpperCase_ReturnsTrue()
        {
            var signature = HmacUtf8(Body, SharedKey);

            UsefulExtensions.IsChargifyWebhookContentValidHMAC(Body, signature, SharedKey).ShouldBeTrue();
            UsefulExtensions.IsChargifyWebhookContentValidHMAC(Body, signature.ToUpperInvariant(), SharedKey).ShouldBeTrue();
        }

        [TestMethod]
        public void Hmac_NonAsciiBody_UsesUtf8()
        {
            const string body = "customer[first_name]=Zoë&customer[last_name]=Ångström";

            UsefulExtensions.IsChargifyWebhookContentValidHMAC(body, HmacUtf8(body, SharedKey), SharedKey).ShouldBeTrue();
        }

        [TestMethod]
        public void Hmac_WrongSignature_ReturnsFalse()
        {
            UsefulExtensions.IsChargifyWebhookContentValidHMAC(Body, FlipLastChar(HmacUtf8(Body, SharedKey)), SharedKey).ShouldBeFalse();
            UsefulExtensions.IsChargifyWebhookContentValidHMAC(Body, HmacUtf8(Body, "other-key"), SharedKey).ShouldBeFalse();
            UsefulExtensions.IsChargifyWebhookContentValidHMAC(Body + "x", HmacUtf8(Body, SharedKey), SharedKey).ShouldBeFalse();
        }

        [DataRow(null)]
        [DataRow("")]
        [DataRow("abc")]
        [DataRow("zz")]
        [TestMethod]
        public void Hmac_MissingOrMalformedSignature_ReturnsFalseWithoutThrowing(string? signature)
        {
            UsefulExtensions.IsChargifyWebhookContentValidHMAC(Body, signature!, SharedKey).ShouldBeFalse();
        }

        [TestMethod]
        public void Hmac_TruncatedOrPaddedOrNonHexSignature_ReturnsFalse()
        {
            var signature = HmacUtf8(Body, SharedKey);

            UsefulExtensions.IsChargifyWebhookContentValidHMAC(Body, signature.Substring(0, 63), SharedKey).ShouldBeFalse();
            UsefulExtensions.IsChargifyWebhookContentValidHMAC(Body, signature + "0", SharedKey).ShouldBeFalse();
            UsefulExtensions.IsChargifyWebhookContentValidHMAC(Body, signature + "00", SharedKey).ShouldBeFalse();
            UsefulExtensions.IsChargifyWebhookContentValidHMAC(Body, "g" + signature.Substring(1), SharedKey).ShouldBeFalse();
            UsefulExtensions.IsChargifyWebhookContentValidHMAC(Body, " " + signature.Substring(1), SharedKey).ShouldBeFalse();
        }

        // --- IsChargifyWebhookContentValid (legacy MD5) ---

        [TestMethod]
        public void Md5_ValidSignature_ReturnsTrue()
        {
            var signature = Md5(Body, SharedKey);

            Body.IsChargifyWebhookContentValid(signature, SharedKey).ShouldBeTrue();
            Body.IsChargifyWebhookContentValid(signature.ToUpperInvariant(), SharedKey).ShouldBeTrue();
        }

        [TestMethod]
        public void Md5_EmptySignature_ReturnsFalse()
        {
            // Regression: the old prefix comparison accepted "" for any body.
            Body.IsChargifyWebhookContentValid(string.Empty, SharedKey).ShouldBeFalse();
        }

        [TestMethod]
        public void Md5_TruncatedSignature_ReturnsFalse()
        {
            // Regression: the old prefix comparison accepted any prefix of the digest.
            var signature = Md5(Body, SharedKey);

            Body.IsChargifyWebhookContentValid(signature.Substring(0, 1), SharedKey).ShouldBeFalse();
            Body.IsChargifyWebhookContentValid(signature.Substring(0, 16), SharedKey).ShouldBeFalse();
            Body.IsChargifyWebhookContentValid(signature.Substring(0, 31), SharedKey).ShouldBeFalse();
        }

        [TestMethod]
        public void Md5_NullLongWrongOrNonHexSignature_ReturnsFalseWithoutThrowing()
        {
            var signature = Md5(Body, SharedKey);

            Body.IsChargifyWebhookContentValid(null!, SharedKey).ShouldBeFalse();
            Body.IsChargifyWebhookContentValid(signature + "00", SharedKey).ShouldBeFalse(); // used to throw ArgumentOutOfRangeException
            Body.IsChargifyWebhookContentValid(FlipLastChar(signature), SharedKey).ShouldBeFalse();
            Body.IsChargifyWebhookContentValid(new string('x', 32), SharedKey).ShouldBeFalse();
        }

        // --- IsWebhookRequestValid (stream) ---

        private static Stream StreamOf(string text) => new MemoryStream(Encoding.UTF8.GetBytes(text));

        [TestMethod]
        public void Stream_ValidSignature_EitherCase_ReturnsTrue()
        {
            var signature = HmacAscii(Body, SharedKey);

            StreamOf(Body).IsWebhookRequestValid(SharedKey, signature).ShouldBeTrue();
            StreamOf(Body).IsWebhookRequestValid(SharedKey, signature.ToUpperInvariant()).ShouldBeTrue();
        }

        [TestMethod]
        public void Stream_MatchesCalculateHMAC256Signature()
        {
            var signature = UsefulExtensions.CalculateHMAC256Signature(Body, SharedKey);

            signature.ShouldBe(HmacAscii(Body, SharedKey));
            StreamOf(Body).IsWebhookRequestValid(SharedKey, signature).ShouldBeTrue();
        }

        [DataRow(null)]
        [DataRow("")]
        [DataRow("not-hex")]
        [TestMethod]
        public void Stream_MissingOrMalformedSignature_ReturnsFalse(string? signature)
        {
            StreamOf(Body).IsWebhookRequestValid(SharedKey, signature!).ShouldBeFalse();
        }

        [TestMethod]
        public void Stream_WrongSignatureOrEmptyBody_ReturnsFalse()
        {
            StreamOf(Body).IsWebhookRequestValid(SharedKey, FlipLastChar(HmacAscii(Body, SharedKey))).ShouldBeFalse();
            StreamOf(string.Empty).IsWebhookRequestValid(SharedKey, HmacAscii(string.Empty, SharedKey)).ShouldBeFalse();
        }

        // --- FixedTimeEqualsHex ---

        [TestMethod]
        public void FixedTimeEqualsHex_Basics()
        {
            var bytes = new byte[] { 0x00, 0x0f, 0xa0, 0xff };

            UsefulExtensions.FixedTimeEqualsHex(bytes, "000fa0ff").ShouldBeTrue();
            UsefulExtensions.FixedTimeEqualsHex(bytes, "000FA0FF").ShouldBeTrue();
            UsefulExtensions.FixedTimeEqualsHex(bytes, "000fa0fe").ShouldBeFalse();
            UsefulExtensions.FixedTimeEqualsHex(bytes, "000fa0f").ShouldBeFalse();
            UsefulExtensions.FixedTimeEqualsHex(bytes, "000fa0ffff").ShouldBeFalse();
            UsefulExtensions.FixedTimeEqualsHex(bytes, "000fa0fg").ShouldBeFalse();
            UsefulExtensions.FixedTimeEqualsHex(bytes, null).ShouldBeFalse();
            UsefulExtensions.FixedTimeEqualsHex(Array.Empty<byte>(), string.Empty).ShouldBeTrue();
        }
    }
}
