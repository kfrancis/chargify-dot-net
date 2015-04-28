using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace Chargify
{
    public class ChargifyApiBase
    {
        #region Properties and Accessors
        protected string ApiKey { get; private set; }
        protected string ApiPassword { get; private set; }
        protected string SharedKey { get; private set; }
        protected string Subdomain { get; private set; }
        protected bool UseJSON { get; private set; }

        private static string _userAgent;
        private static string UserAgent
        {
            get
            {
                if (_userAgent == null)
                {
                    _userAgent = String.Format("Chargify .NET RestSharp Client v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
                }
                return _userAgent;
            }
        }

        private RestClient _client;
        #endregion

        #region Constructors
        protected ChargifyApiBase(string apiKey, string apiPassword)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentException("Api key is required. Details can be found at http://docs.chargify.com/api-authentication", "apiKey");

            this.ApiKey = apiKey;
            this.ApiPassword = apiPassword;
            this.UseJSON = false;

            _client = new RestClient();
            _client.Authenticator = new HttpBasicAuthenticator(this.ApiKey, this.ApiPassword);
            _client.UserAgent = ChargifyApiBase.UserAgent;
            _client.BaseUrl = Config.ApiBaseUrl;
        }

        protected ChargifyApiBase(string apiKey, string apiPassword, bool useJson)
            : this(apiKey, apiPassword)
        {
            this.UseJSON = useJson;
        }
        #endregion

        protected T GetRequest<T>(string path, params object[] args) where T : new()
        {
            return GetRequest<T>(path, string.Empty, args);
        }

        protected T GetRequest<T>(string path, string rootElement, params object[] args) where T : new()
        {
            RestRequest request = new RestRequest(BuildUrl(path, args));
            if (!string.IsNullOrWhiteSpace(rootElement)) request.RootElement = rootElement;
            SetupRequest(request);

            var response = _client.Execute<T>(request);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new ChargifyException("Not Found");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                throw new ChargifyException("Internal Server Error");
            }

            return response.Data;
        }

        protected TOutput PutRequest<TOutput, TInput>(TInput obj, string path, params object[] args) where TOutput : new()
        {
            return Request<TOutput, TInput>(Method.PUT, obj, path, args);
        }

        protected TOutput PostRequest<TOutput, TInput>(TInput obj, string path, params object[] args) where TOutput : new()
        {
            return Request<TOutput, TInput>(Method.POST, obj, path, args);
        }

        protected void DeleteRequest(string path, params object[] args)
        {
            Request<List<object>, object>(Method.DELETE, null, path, args);
        }

        private TOutput Request<TOutput, TInput>(Method method, TInput obj, string path, params object[] args) where TOutput : new()
        {            
            RestRequest request = new RestRequest(BuildUrl(path, args), method);
            SetupRequest(request);

            if (obj != null)
            {
                request.AddBody(obj);
            }

            var response = _client.Execute<TOutput>(request);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new ChargifyException("Not Found");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                throw new ChargifyException("Internal Server Error");
            }

            return response.Data;
        }

        protected string BuildUrl(string path, params object[] args)
        {
            // Assume for now that no querystring is added
            path = string.Format(path, args);

            if (this.UseJSON)
            {
                path += ".json";
            }
            else
            {
                path += ".xml";
            }

            return path;
        }

        protected void SetupRequest(RestRequest request)
        {
            if (request.Resource.ToLowerInvariant().Contains(".xml"))
            {
                request.RequestFormat = DataFormat.Xml;
                request.XmlSerializer = new RestSharp.Serializers.DotNetXmlSerializer();
            }
            else 
            { 
                request.RequestFormat = DataFormat.Json;
                request.JsonSerializer = new JsonSerializer();
            }
        }
    }
}
