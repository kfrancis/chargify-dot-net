namespace ChargifyDotNet
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Masks card data in request/response payloads before they are stored on exceptions or handed to log callbacks.
    /// </summary>
    internal static class SensitiveDataMasker
    {
        private static readonly Regex s_xmlFullNumber = new(@"(<full_number>)([^<]*)(</full_number>)", RegexOptions.Compiled);
        private static readonly Regex s_jsonFullNumber = new(@"(""full_number""\s*:\s*"")([^""]*)("")", RegexOptions.Compiled);
        private static readonly Regex s_xmlCvv = new(@"(<cvv>)([^<]*)(</cvv>)", RegexOptions.Compiled);
        private static readonly Regex s_jsonCvv = new(@"(""cvv""\s*:\s*"")([^""]*)("")", RegexOptions.Compiled);

        /// <summary>
        /// Masks every <c>full_number</c> (all but the last 4 characters) and <c>cvv</c> (all characters)
        /// value found in an XML or JSON payload. Returns the input unchanged if there is nothing to mask.
        /// </summary>
        internal static string? Mask(string? payload)
        {
            if (payload == null || payload.Length == 0) return payload;
            if (payload.IndexOf("full_number", System.StringComparison.Ordinal) < 0 &&
                payload.IndexOf("cvv", System.StringComparison.Ordinal) < 0)
            {
                return payload;
            }

            var result = s_xmlFullNumber.Replace(payload, m => m.Groups[1].Value + m.Groups[2].Value.Mask('X', 4) + m.Groups[3].Value);
            result = s_jsonFullNumber.Replace(result, m => m.Groups[1].Value + m.Groups[2].Value.Mask('X', 4) + m.Groups[3].Value);
            result = s_xmlCvv.Replace(result, m => m.Groups[1].Value + m.Groups[2].Value.Mask('X') + m.Groups[3].Value);
            result = s_jsonCvv.Replace(result, m => m.Groups[1].Value + m.Groups[2].Value.Mask('X') + m.Groups[3].Value);
            return result;
        }
    }
}
