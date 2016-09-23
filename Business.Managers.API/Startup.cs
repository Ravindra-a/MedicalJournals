using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.OAuth;
using System.Web.Http;
using System.Net.Http.Formatting;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using Business.Managers.API.Providers;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Web.Routing;
using Microsoft.Owin.Cors;
using Business.Managers.API.App_Start;
using Repositories;
using System.Data.Entity;

[assembly: OwinStartup(typeof(Business.Managers.API.Startup))]

namespace Business.Managers.API
{
    public class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthServerOptions { get; private set; }
             
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration httpConfig = new HttpConfiguration();

            ConfigureOAuthTokenGeneration(app);

            ConfigureOAuthTokenConsumption(app);
            
            ConfigureWebApi(httpConfig);

            app.UseCors(CorsOptions.AllowAll);

            app.UseWebApi(httpConfig);

        }

        private void ConfigureWebApi(HttpConfiguration config)
        {
            WebApiConfig.Register(config);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);            
        }
          
        /// <summary>
        /// Configures JWT for Web,SPA,Mobile apps
        /// </summary>
        /// <param name="app"></param>
        private void ConfigureOAuthTokenGeneration(IAppBuilder app)
        {
            // Configure the db context, user manager and role manager to use a single instance per request
            app.CreatePerOwinContext(MedicalJournalContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);

            //Cookie for old school MVC application
            var cookieOptions = new CookieAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                CookieHttpOnly = true, // JavaScript should use the Bearer
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/api/Account/Login"),
                CookieName = "AuthCookie"
            };
            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                //For Dev enviroment only (on production should be AllowInsecureHttp = false)
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/oauth/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(30),
                Provider = new CustomOAuthProvider(),
                AccessTokenFormat = new CustomJwtFormat(ConfigurationManager.AppSettings["JWTPath"])
            };

            // OAuth 2.0 Bearer Access Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
        }

        /// <summary>
        /// Consuming token for webAPI [Authorize] tag
        /// </summary>
        /// <param name="app"></param>
        private void ConfigureOAuthTokenConsumption(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions { CookieName = "AuthCookie", AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie });

            var issuer = ConfigurationManager.AppSettings["JWTPath"];
            string audienceId = ConfigurationManager.AppSettings["as:AudienceId"];
            byte[] audienceSecret = TextEncodings.Base64Url.Decode(ConfigurationManager.AppSettings["as:AudienceSecret"]);

            // Api controllers with an [Authorize] attribute will be validated with JWT
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,
                    AllowedAudiences = new[] { audienceId },
                    IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                    {
                        new SymmetricKeyIssuerSecurityTokenProvider(issuer, audienceSecret)
                    }
                });
        }
    }
}
