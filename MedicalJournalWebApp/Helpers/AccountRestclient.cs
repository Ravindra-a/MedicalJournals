using Business.Entites.Auth;
using MedicalJournalWebApp.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace MedicalJournalWebApp.Helpers
{
    public class AccountRestclient
    {
        private readonly RestClient _client;
        private string _apicontrollername;
        private readonly string _url = ConfigurationManager.AppSettings["webapibaseurl"];

        public AccountRestclient(string apiControllerName)
        {
            _client = new RestClient(_url);
            _apicontrollername = apiControllerName;
        }

        /// <summary>
        /// call the Register API
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IRestResponse Register(MedicalJournalWebApp.Models.RegisterViewModel model)
        {
            var request = new RestRequest(_apicontrollername, Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(model);
            return _client.Execute(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IRestResponse Login(LoginViewModel model)
        {
            var request = new RestRequest(_apicontrollername, Method.POST) { RequestFormat = DataFormat.Json };
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("username", model.LoginEmail);
            request.AddParameter("password", model.LoginPassword);
            //request.AddParameter("client_id", ConfigurationManager.AppSettings["ClientKey"]);
            //request.AddParameter("client_secret", ConfigurationManager.AppSettings["ClientSecret"]);
            request.AddParameter("grant_type", "password");
            var response = _client.Execute(request);
            return response;
        }
    }
}