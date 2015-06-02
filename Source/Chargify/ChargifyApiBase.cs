
#region License, Terms and Conditions
//
// ChargifyApiBase.cs
//
// Authors: Kori Francis <twitter.com/djbyter>, David Ball
// Copyright (C) 2010 Clinical Support Systems, Inc. All rights reserved.
// 
//  THIS FILE IS LICENSED UNDER THE MIT LICENSE AS OUTLINED IMMEDIATELY BELOW:
//
//  Permission is hereby granted, free of charge, to any person obtaining a
//  copy of this software and associated documentation files (the "Software"),
//  to deal in the Software without restriction, including without limitation
//  the rights to use, copy, modify, merge, publish, distribute, sublicense,
//  and/or sell copies of the Software, and to permit persons to whom the
//  Software is furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
//  FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
//  DEALINGS IN THE SOFTWARE.
//
#endregion

namespace Chargify
{
    #region Imports
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using RestSharp;
    #endregion

    public class BaseService<TObject> where TObject : class
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
        public BaseService()
        {
        }

        public BaseService(string apiKey, string apiPassword)
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

        public BaseService(string apiKey, string apiPassword, bool useJson)
            : this(apiKey, apiPassword)
        {
            UseJSON = useJson;
        }

        public BaseService(RestClient client)
        {
            _client = client;
        }
        #endregion

        #region Sync
        internal TObject GetRequest<TObject>(string path, params object[] args) where TObject : new()
        {
            return GetRequest<TObject>(path, string.Empty, args);
        }

        internal TObject GetRequest<TObject>(string path, string rootElement, params object[] args) where TObject : new()
        {
            RestRequest request = new RestRequest(BuildUrl(path, args));
            if (!string.IsNullOrWhiteSpace(rootElement)) request.RootElement = rootElement;
            SetupRequest(request);

            var response = _client.Execute<TObject>(request);

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

        internal TObject PutRequest<TObject, TInput>(TInput obj, string path, params object[] args) where TObject : new()
        {
            return Request<TObject, TInput>(Method.PUT, obj, path, args);
        }

        internal TObject PostRequest<TObject, TInput>(TInput obj, string path, params object[] args) where TObject : new()
        {
            return Request<TObject, TInput>(Method.POST, obj, path, args);
        }

        internal void DeleteRequest(string path, params object[] args)
        {
            Request<List<object>, object>(method: Method.DELETE, obj: null, path: path, args: args);
        }

        private TObject Request<TObject, TInput>(Method method, TInput obj, string path, params object[] args) where TObject : new()
        {
            RestRequest request = new RestRequest(BuildUrl(path, args), method);
            SetupRequest(request);

            if (obj != null)
            {
                request.AddBody(obj);
            }

            var response = _client.Execute<TObject>(request);

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
        internal async Task<TObject> GetRequestAsync<TObject>(string path, params object[] args) where TObject : new()
        {
            return await GetRequestAsync<TObject>(path, string.Empty, args).ConfigureAwait(continueOnCapturedContext: false);
        }

        internal Task<TObject> GetRequestAsync<TObject>(string path, string rootElement, params object[] args) where TObject : new()
        {
            RestRequest request = new RestRequest(BuildUrl(path, args));
            if (!string.IsNullOrWhiteSpace(rootElement)) request.RootElement = rootElement;
            SetupRequest(request);

            var tcs = new TaskCompletionSource<TObject>();
            _client.ExecuteAsync<TObject>(request, (response) =>
            {
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
        internal async Task<TObject> PutRequestAsync<TObject, TInput>(TInput obj, string path, params object[] args) where TObject : new()
        {
            return await RequestAsync<TObject, TInput>(Method.PUT, obj, path, args).ConfigureAwait(continueOnCapturedContext: false);
        }

        internal async Task<TObject> PostRequestAsync<TObject, TInput>(TInput obj, string path, params object[] args) where TObject : new()
        {
            return await RequestAsync<TObject, TInput>(Method.POST, obj, path, args).ConfigureAwait(continueOnCapturedContext: false);
        }

        internal async Task DeleteRequestAsync(string path, params object[] args)
        {
            await RequestAsync<List<object>, object>(method: Method.DELETE, obj: null, path: path, args: args).ConfigureAwait(continueOnCapturedContext: false);
        }

        private Task<TObject> RequestAsync<TObject, TInput>(Method method, TInput obj, string path, params object[] args) where TObject : new()
        {
            RestRequest request = new RestRequest(BuildUrl(path, args), method);
            SetupRequest(request);

            if (obj != null)
            {
                request.AddBody(obj);
            }

            var tcs = new TaskCompletionSource<TObject>();
            _client.ExecuteAsync<TObject>(request, (response) =>
            {
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
