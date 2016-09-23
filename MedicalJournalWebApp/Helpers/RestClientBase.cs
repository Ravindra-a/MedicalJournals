using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace MedicalJournalWebApp.Helpers
{
    public class RestClientBase<T> where T : class,new()
    {
        private readonly RestClient _client;
        private readonly string _url = ConfigurationManager.AppSettings["webapibaseurl"];
        private string _apicontrollername;
        public RestClientBase(string baseapicontrollername)
        {
            _client = new RestClient(_url);
            _apicontrollername = "api/" + baseapicontrollername;
        }

        public IEnumerable<T> GetAll()
        {
            var request = new RestRequest(_apicontrollername, Method.GET) { RequestFormat = DataFormat.Json };
            if (HttpContext.Current.Request.Cookies["AuthCookie"] != null)
                request.AddCookie("AuthCookie", HttpContext.Current.Request.Cookies["AuthCookie"].Value);
            return ExecuteAsList<T>(request);
        }

        public T GetById(int id)
        {
            var request = new RestRequest(_apicontrollername + "/{id}", Method.GET) { RequestFormat = DataFormat.Json };

            request.AddParameter("id", id, ParameterType.UrlSegment);

            if (HttpContext.Current.Request.Cookies["AuthCookie"] != null)
                request.AddCookie("AuthCookie", HttpContext.Current.Request.Cookies["AuthCookie"].Value);

            var response = _client.Execute<T>(request);
            

            if (response.StatusCode == HttpStatusCode.InternalServerError && response.ErrorMessage != null)
                throw new Exception(response.ErrorMessage);

            return JsonConvert.DeserializeObject<T>(response.Content);
        }

        private List<TResult> ExecuteAsList<TResult>(IRestRequest request) where TResult : new()
        {
            var response = _client.Execute(request);
            if (response.ResponseStatus == ResponseStatus.Error)
                throw new Exception(response.ErrorMessage);
            return ConvertToList<TResult>(response.Content);
        }

        private List<TResult> ConvertToList<TResult>(string response) where TResult : new()
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<TResult>>(response);
        }

        public void Add(T serverData)
        {

            var request = new RestRequest(_apicontrollername, Method.POST) { RequestFormat = DataFormat.Json };
            request.JsonSerializer = new RestSharpJsonNetSerializer(new Newtonsoft.Json.JsonSerializer());
            request.AddBody(serverData);
            request.AddCookie("AuthCookie", HttpContext.Current.Request.Cookies["AuthCookie"].Value);
            var response = _client.Execute<T>(request);

            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
                return;
            else
                throw new Exception(response.ErrorMessage);
        }

    }

    /// <summary>
    /// Default JSON serializer for request bodies
    /// Doesn't currently use the SerializeAs attribute, defers to Newtonsoft's attributes
    /// </summary>
    public class RestSharpJsonNetSerializer : ISerializer
    {
        private readonly Newtonsoft.Json.JsonSerializer _serializer;

        /// <summary>
        /// Default serializer
        /// </summary>
        public RestSharpJsonNetSerializer()
        {
            ContentType = "application/json";
            _serializer = new Newtonsoft.Json.JsonSerializer
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Include,
                DefaultValueHandling = DefaultValueHandling.Include
            };
        }

        /// <summary>
        /// Default serializer with overload for allowing custom Json.NET settings
        /// </summary>
        public RestSharpJsonNetSerializer(Newtonsoft.Json.JsonSerializer serializer)
        {
            ContentType = "application/json";
            _serializer = serializer;
        }

        /// <summary>
        /// Serialize the object as JSON
        /// </summary>
        /// <param name="obj">Object to serialize
        /// <returns>JSON as String</returns>
        public string Serialize(object obj)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var jsonTextWriter = new JsonTextWriter(stringWriter))
                {
                    jsonTextWriter.Formatting = Formatting.Indented;
                    jsonTextWriter.QuoteChar = '"';

                    _serializer.Serialize(jsonTextWriter, obj);

                    var result = stringWriter.ToString();
                    return result;
                }
            }
        }

        /// <summary>
        /// Unused for JSON Serialization
        /// </summary>
        public string DateFormat { get; set; }
        /// <summary>
        /// Unused for JSON Serialization
        /// </summary>
        public string RootElement { get; set; }
        /// <summary>
        /// Unused for JSON Serialization
        /// </summary>
        public string Namespace { get; set; }
        /// <summary>
        /// Content type for serialized content
        /// </summary>
        public string ContentType { get; set; }
    }
}