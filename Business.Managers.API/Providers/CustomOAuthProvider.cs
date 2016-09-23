using Business.Entites;
using Business.Managers.API.App_Start;
using Microsoft.Owin.Security.OAuth;
using Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity.Owin;
using System.Security.Claims;
using Repositories;

namespace Business.Managers.API.Providers
{
    public class CustomOAuthProvider : OAuthAuthorizationServerProvider
    {
        IAuthClientRepository iauthClientRepository;

        public CustomOAuthProvider()
        {
            iauthClientRepository = new AuthClientRepository();
        }

        //Todo need to use DI
        //public CustomOAuthProvider(IAuthClientRepository _iauthClientRepository)
        //{
        //    iauthClientRepository = _iauthClientRepository;
        //}

        /// <summary>
        /// validating client id with DB
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {

            string clientId = string.Empty;
            string clientSecret = string.Empty;
            Client client = null;

            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }

            if (context.ClientId == null)
            {
                //Remove the comments from the below line context.SetError, and invalidate context 
                //if you want to force sending clientId/secrects once obtain access tokens. 
                context.Validated();
                //context.SetError("invalid_clientId", "ClientId should be sent.");
                return Task.FromResult<object>(null);
            }

            client = iauthClientRepository.FindClient(context.ClientId);


            if (client == null)
            {
                context.SetError("invalid_clientId", string.Format("Client '{0}' is not registered in the system.", context.ClientId));
                return Task.FromResult<object>(null);
            }

            if (!client.Active)
            {
                context.SetError("invalid_clientId", "Client is inactive.");
                return Task.FromResult<object>(null);
            }

            context.OwinContext.Set<string>("clientAllowedOrigin", client.AllowedOrigin);

            context.Validated();
            return Task.FromResult<object>(null);

        }

        /// <summary>
        /// The method “GrantResourceOwnerCredentials” is responsible for receiving the username and password from the request and validate them against our ASP.NET 2.1 Identity system, 
        /// if the credentials are valid and the email is confirmed we are building an identity for the logged in user, 
        /// this identity will contain all the roles and claims for the authenticated user, until now we didn’t cover roles and claims part of the tutorial,
        /// but for the mean time you can consider all users registered in our system without any roles or claims mapped to them.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            var allowedOrigin = context.OwinContext.Get<string>("clientAllowedOrigin");

            if (allowedOrigin == null) allowedOrigin = "*";

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();


            ApplicationUser user = await userManager.FindAsync(context.UserName.ToLower(), context.Password);

            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            if (!user.EmailConfirmed)
            {
                context.SetError("invalid_grant", "Your account is created, please check your email and confirm your email address");
                return;
            }

            //Generating access token SPA,Mobile apps
            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager, "JWT");
            var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    { 
                        "client_id", (context.ClientId == null) ? string.Empty : context.ClientId
                    },
                    { 
                        "userName", context.UserName
                    }
                });
            var ticket = new AuthenticationTicket(oAuthIdentity, props);

            context.Validated(ticket);

            //Generating cookie for web clients
            ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
                CookieAuthenticationDefaults.AuthenticationType);
            context.Request.Context.Authentication.SignIn(props, cookiesIdentity);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }
    }
}