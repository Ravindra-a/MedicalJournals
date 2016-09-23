using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Business.Entites.Auth;
using MedicalJournalWebApp.Models;
using MedicalJournalWebApp.Helpers;
using RestSharp;
using System.Net;
using Newtonsoft.Json;
using Business.Entites;

namespace MedicalJournalWebApp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        AccountRestclient accountRestClient;
        IRestResponse response;

        public AccountController()
        {
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (TempData["PreviousUrl"] != null)
            {
                switch(TempData["PreviousUrl"].ToString())
                {
                    case "Register":
                        ViewBag.Message = "Successfully registered please log in";
                        break;
                    default :
                        ViewBag.Message = "Please Log in";
                        break;
                }
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                accountRestClient = new AccountRestclient("OAuth/token");
                IRestResponse loginRestClient = new RestResponse();
                response = accountRestClient.Login(model);
                if(response.StatusCode == HttpStatusCode.OK)
                {
                    AccessClaims claimsToken = new AccessClaims();
                    claimsToken = JsonConvert.DeserializeObject<AccessClaims>(response.Content);
                    claimsToken.Cookie = response.Cookies[0].Value;
                    Request.Headers.Add("Authorization", "bearer " + claimsToken.access_token);
                    var ctx = Request.GetOwinContext();
                    var authenticateResult = await ctx.Authentication.AuthenticateAsync("JWT");
                    ctx.Authentication.SignOut("JWT");
                    var applicationCookieIdentity = new ClaimsIdentity(authenticateResult.Identity.Claims, DefaultAuthenticationTypes.ApplicationCookie);
                    ctx.Authentication.SignIn(applicationCookieIdentity);

                    var principal = new ClaimsPrincipal(applicationCookieIdentity);
                    System.Threading.Thread.CurrentPrincipal = principal;
                    if (System.Web.HttpContext.Current != null)
                        System.Web.HttpContext.Current.User = principal;
                    if(User.IsInRole(ApplicationRoles.Publisher.ToString()))
                        return RedirectToAction("Index", "Publish");
                    else
                        return RedirectToAction("Index", "Subscribe");
                }
                else
                {
                    OauthTokenMessage errorsfromAPI = new OauthTokenMessage();
                    errorsfromAPI = JsonConvert.DeserializeObject<OauthTokenMessage>(response.Content);
                    ModelState.AddModelError("", errorsfromAPI.error_description);
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
            



        }
        
        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(MedicalJournalWebApp.Models.RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if(model.IsPublisher)
                {
                    model.SelectedRole = ApplicationRoles.Publisher;
                }
                accountRestClient = new AccountRestclient("api/account/register");
                response = accountRestClient.Register(model);
                if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK)
                {
                    TempData["PreviousUrl"] = "Register";
                    return RedirectToAction("Login");
                }
                else
                {
                    APIMessage errorsfromAPI = new APIMessage();
                    errorsfromAPI = JsonConvert.DeserializeObject<APIMessage>(response.Content);
                    ModelState.AddModelError("", errorsfromAPI.Message);
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
       
        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Login", "Account");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {

            }

            base.Dispose(disposing);
        }

        #region Helpers
        
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        #endregion
    }
}