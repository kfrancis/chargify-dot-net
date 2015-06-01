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
                    _userAgent = string.Format(format: "Chargify .NET RestSharp Client v{0}", arg0: System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
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
                throw new ArgumentException(message: "Api key is required. Details can be found at http://docs.chargify.com/api-authentication", paramName: "apiKey");

            ApiKey = apiKey;
            ApiPassword = apiPassword;
            UseJSON = false;

            _client = new RestClient();
            _client.Authenticator = new HttpBasicAuthenticator(ApiKey, ApiPassword);
            _client.UserAgent = UserAgent;
            _client.BaseUrl = new Uri(Config.ApiBaseUrl);
        }

        protected ChargifyApiBase(string apiKey, string apiPassword, bool useJson)
            : this(apiKey, apiPassword)
        {
            UseJSON = useJson;
        }
        #endregion

        #region Sync
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
                throw new ChargifyException(message: "Not Found");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                throw new ChargifyException(message: "Internal Server Error");
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
            Request<List<object>, object>(method: Method.DELETE, obj: null, path: path, args: args);
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
                throw new ChargifyException(message: "Not Found");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                throw new ChargifyException(message: "Internal Server Error");
            }

            return response.Data;
        }
        #endregion

        #region Async
        protected async Task<T> GetRequestAsync<T>(string path, params object[] args) where T : new()
        {
            return await GetRequestAsync<T>(path, string.Empty, args).ConfigureAwait(continueOnCapturedContext: false);
        }

        protected Task<T> GetRequestAsync<T>(string path, string rootElement, params object[] args) where T : new()
        {
            RestRequest request = new RestRequest(BuildUrl(path, args));
            if (!string.IsNullOrWhiteSpace(rootElement)) request.RootElement = rootElement;
            SetupRequest(request);

            var tcs = new TaskCompletionSource<T>();
            _client.ExecuteAsync<T>(request, (response)=>{
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    tcs.SetException(new ChargifyException(message: "Not Found"));
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    tcs.SetException(new ChargifyException(message: "Internal Server Error"));
                }
                else
                {
                    tcs.SetResult(response.Data);
                }
            });

            return tcs.Task;
        }

        protected async Task<TOutput> PutRequestAsync<TOutput, TInput>(TInput obj, string path, params object[] args) where TOutput : new()
        {
            return await RequestAsync<TOutput, TInput>(Method.PUT, obj, path, args).ConfigureAwait(continueOnCapturedContext: false);
        }

        protected async Task<TOutput> PostRequestAsync<TOutput, TInput>(TInput obj, string path, params object[] args) where TOutput : new()
        {
            return await RequestAsync<TOutput, TInput>(Method.POST, obj, path, args).ConfigureAwait(continueOnCapturedContext: false);
        }

        protected async Task DeleteRequestAsync(string path, params object[] args)
        {
            await RequestAsync<List<object>, object>(method: Method.DELETE, obj: null, path: path, args: args).ConfigureAwait(continueOnCapturedContext: false);
        }

        private Task<TOutput> RequestAsync<TOutput, TInput>(Method method, TInput obj, string path, params object[] args) where TOutput : new()
        {
            RestRequest request = new RestRequest(BuildUrl(path, args), method);
            SetupRequest(request);

            if (obj != null)
            {
                request.AddBody(obj);
            }

            var tcs = new TaskCompletionSource<TOutput>();
            _client.ExecuteAsync<TOutput>(request, (response) => {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    tcs.SetException(new ChargifyException(message: "Not Found"));
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    tcs.SetException(new ChargifyException(message: "Internal Server Error"));
                }
                else
                {
                    tcs.SetResult(response.Data);
                }
            });

            return tcs.Task;
        }
        #endregion

        #region Helper Methods
        protected string BuildUrl(string path, params object[] args)
        {
            // Assume for now that no querystring is added
            path = string.Format(path, args);

            if (UseJSON)
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
            if (request.Resource.ToLowerInvariant().Contains(value: ".xml"))
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
        #endregion
    }
}
