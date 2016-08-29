using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.DotNet.InternalAbstractions;
using System.Reflection;
using System.Net.Http;
using System.Text;

namespace ChargifyNET.DotCore
{
    public class ChargifyConnect
    {
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public ChargifyConnect() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="url">The Chargify URL</param>
        /// <param name="apiKey">Your Chargify api key</param>
        /// <param name="password">Your Chargify api password</param>
        public ChargifyConnect(string url, string apiKey, string password)
        {
            this.URL = url;
            this.apiKey = apiKey;
            this.Password = password;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="url">The Chargify URL</param>
        /// <param name="apiKey">Your Chargify api key</param>
        /// <param name="password">Your Chargify api password</param>
        /// <param name="sharedKey">Your Chargify hosted page shared key</param>
        public ChargifyConnect(string url, string apiKey, string password, string sharedKey)
        {
            this.URL = url;
            this.apiKey = apiKey;
            this.Password = password;
            this.SharedKey = sharedKey;
        }

        #endregion

        #region Properties
        private static string UserAgent
        {
            get
            {
                if (_userAgent == null)
                {
                    _userAgent = $"Chargify.NET Client v{typeof(ChargifyConnect).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}";
                }
                return _userAgent;
            }
        }
        private static string _userAgent;

        /// <summary>
        /// Get or set the API key
        /// </summary>
        public string apiKey { get; set; }

        /// <summary>
        /// Get or set the password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Get or set the URL for chargify
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// SharedKey used for url generation
        /// </summary>
        public string SharedKey { get; set; }

        /// <summary>
        /// Should Chargify.NET use JSON for output? XML by default, always XML for input.
        /// </summary>
        public bool UseJSON { get; set; }

        /// <summary>
        /// Should the library require a CVV?
        /// </summary>
        public bool CvvRequired { get { return this._cvvRequired; } set { this._cvvRequired = value; } }
        private bool _cvvRequired = true;


        /// <summary>
        /// The timeout (in milliseconds) for any call to Chargify. The default is 180000
        /// </summary>
        public int Timeout
        {
            get
            {
                return this._timeout;
            }
            set
            {
                this._timeout = value;
            }
        }
        private int _timeout = 180000;

        /// <summary>
        /// Method for determining if the properties have been set to allow this instance to connect correctly.
        /// </summary>
        public bool HasConnected
        {
            get
            {
                bool result = true;
                if (string.IsNullOrEmpty(this.apiKey)) result = false;
                if (string.IsNullOrEmpty(this.Password)) result = false;
                if (string.IsNullOrEmpty(this.URL)) result = false;
                return result;
            }
        }

        /// <summary>
        /// Caller can plug in a delegate for logging raw Chargify requests
        /// </summary>
        public Action<HttpRequestMethod, string, string> LogRequest { get; set; }

        /// <summary>
        /// Caller can plug in a delegate for logging raw Chargify responses
        /// </summary>
        public Action<HttpStatusCode, string, string> LogResponse { get; set; }

        /// <summary>
        /// Get a reference to the last Http Response from the chargify server. This is set after every call to
        /// a Chargify Connect method
        /// </summary>
        public HttpResponseMessage LastResponse
        {
            get
            {
                return _lastResponse;
            }
        }
        private HttpResponseMessage _lastResponse = null;

        #endregion

        public void Test()
        {
            Console.WriteLine(DoRequest("/subscriptions/12670109.xml", HttpRequestMethod.Get).Result);
        }

        private async Task<string> DoRequest(string methodString, HttpRequestMethod requestMethod, string postData = null)
        {
            // make sure values are set
            if (string.IsNullOrEmpty(this.URL)) throw new InvalidOperationException("URL not set");
            if (string.IsNullOrEmpty(this.apiKey)) throw new InvalidOperationException("apiKey not set");
            if (string.IsNullOrEmpty(this.Password)) throw new InvalidOperationException("Password not set");

            // create the URI
            string addressString = string.Format("{0}{1}", this.URL, (this.URL.EndsWith("/") ? string.Empty : "/"));

            var uriBuilder = new UriBuilder(addressString)
            {
                Scheme = "https",
                Port = -1 // default port for scheme
            };
            Uri address = uriBuilder.Uri;

            var handler = new HttpClientHandler();
            handler.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromMilliseconds(this._timeout),
                BaseAddress = address
            };

            var byteArray = Encoding.ASCII.GetBytes($"{this.apiKey}:{this.Password}");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            client.DefaultRequestHeaders.Add("User-Agent", UserAgent);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(this.UseJSON ? "application/json" : "application/xml"));
            client.DefaultRequestHeaders.TransferEncodingChunked = false;

            try
            {
                // Send the request for logging, if applicable.
                LogRequest?.Invoke(requestMethod, addressString+methodString, postData);

                switch (requestMethod)
                {
                    case HttpRequestMethod.Get:
                        _lastResponse = await client.GetAsync(methodString);
                        break;
                    case HttpRequestMethod.Post:
                        _lastResponse = await client.PostAsync(methodString, new StringContent(postData, Encoding.UTF8, this.UseJSON ? "application/json" : "text/xml"));
                        break;
                    case HttpRequestMethod.Put:
                        _lastResponse = await client.PutAsync(methodString, new StringContent(postData, Encoding.UTF8, this.UseJSON ? "application/json" : "text/xml"));
                        break;
                    case HttpRequestMethod.Delete:
                        _lastResponse = await client.DeleteAsync(methodString);
                        break;
                }

                var content = await _lastResponse?.Content?.ReadAsStringAsync();

                // Send the response for logging, if applicable.
                LogResponse?.Invoke(_lastResponse.StatusCode, addressString + methodString, content);

                return content;
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}

