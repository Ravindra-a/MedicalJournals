using Business.Entites;
using Business.Entites.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Configuration;
using Microsoft.Owin.Security;
using System.Web;
using System.Security.Claims;
using Newtonsoft.Json.Linq;
using Microsoft.Owin.Security.OAuth;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.Infrastructure;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Text;

namespace Business.Managers.API.Controllers
{
    [RoutePrefix("api/account")]
    public class AccountController : BaseAPIController
    {

        public AccountController()
        {

        }
        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        #region Profile info CRUD

        /// <summary>
        /// Method will be called to register a user in local DB
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string[] host = (model.RegisterEmail.Split('@'));
            string hostname = host[1];
            try
            {
                IPHostEntry IPhst = Dns.GetHostEntry(hostname);
            }
            catch
            {
                return BadRequest("invalid Email");
            }

            var user = new ApplicationUser { UserName = model.RegisterEmail.ToLower(), Email = model.RegisterEmail.ToLower(), PhoneNumber = model.RegisterPhoneNumber, EmailConfirmed = true, PhoneNumberConfirmed = true };
            var addUserResult = await this.UserManager.CreateAsync(user, model.RegisterPassword);

            
            if (!addUserResult.Succeeded)
            {
                return BadRequest(addUserResult.Errors.FirstOrDefault());
            }

            //assign role
            var currentUser = this.UserManager.FindByEmail(user.Email);
            if (await AssignRolesToUser(currentUser.Id, model.SelectedRole) == false)
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                return BadRequest(errors[0].ToString());
            }

            return Ok();
        }

        

        /// <summary>
        /// Get all Users - for dashboard and only admin role
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [Route("users")]
        public IHttpActionResult GetUsers()
        {
            return Ok(this.UserManager.Users.ToList());
        }
        
        
        /// <summary>
        /// Change Password after logging in. This shouldn't allow anonymous
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("ChangePassword")]
        public IHttpActionResult ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var changePasswordResult = UserManager.ChangePassword(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                if (changePasswordResult.Succeeded)
                {
                    var user = UserManager.FindById(User.Identity.GetUserId());

                    return Ok();
                }

                //reached this far something went wrong
                GetErrorResult(changePasswordResult);
                return BadRequest(ModelState);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Forgot Password call, will trigger an email and should allow anonymous
        /// </summary>
        /// <param name="model"></param>
        /// <param name="forgotPasswordSubmit"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("forgotpassword")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            var user = await UserManager.FindByNameAsync(model.ForgotPasswordEmail);
            if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return BadRequest("Invalid Email");
            }

            var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var callbackUrl = new Uri(model.ForgotPasswordClientURL + "/Account/ResetPassword?userId=" + user.Id + "&ResetPasswordCode=" + HttpUtility.UrlEncode(code));
            await UserManager.SendEmailAsync(user.Id, "ResetPassWord", "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");

            return Ok("Please check your mail to reset password.");
        }

        /// <summary>
        /// This is the submit call post email link
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("resetpassword")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await UserManager.FindByNameAsync(model.ResetPasswordEmail);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return BadRequest("Something went wrong.Please try again.");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.ResetPasswordCode, model.ResetPasswordPassword);
            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest(result.Errors.FirstOrDefault());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateUser")]
        [Authorize]
        public IHttpActionResult UpdateUser(ProfileViewModel model)
        {
            ApplicationUser user = UserManager.FindById(HttpContext.Current.User.Identity.GetUserId());
            user.ProfileName = model.ProfileName;
            var result = UserManager.Update(user);
            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result.Errors.FirstOrDefault());
            }
        }

        /// <summary>
        /// GEt profile information of authenticated user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetProfileDetails")]
        [Authorize]
        public IHttpActionResult GetProfileDetails()
        {
            ApplicationUser user = UserManager.FindById(HttpContext.Current.User.Identity.GetUserId());

            if (user != null)
            {   
                return Ok(user);
            }
            else
            {
                return NotFound();
            }
        }

        
        
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rolesToAssign"></param>
        /// <returns></returns>
        private async Task<bool> AssignRolesToUser(string id, ApplicationRoles roleToAssign)
        {
            var role = await this.AppRoleManager.FindByNameAsync(roleToAssign.ToString());
            if (role == null)
            {
                ModelState.AddModelError("", "Role does not exist");
                return false;
            }

            var appUser = await this.UserManager.FindByIdAsync(id);

            if (appUser == null)
            {
                ModelState.AddModelError("", String.Format("User: {0} does not exists", id));
                return false;
            }

            if (!this.UserManager.IsInRole(id, role.Name))
            {
                IdentityResult result = await this.UserManager.AddToRoleAsync(id , role.Name);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", String.Format("User: {0} could not be added to role", id));
                    return false;
                }
                return true;
            }
            return false;

        }        
        

        #region helper

        // This method helps to get the error information from the MVC "ModelState".
        // We can not directly send the ModelState to the client in Json. The "ModelState"
        // object has some circular reference that prevents it to be serialized to Json.
        public Dictionary<string, object> GetErrorsFromModelState()
        {
            var errors = new Dictionary<string, object>();
            foreach (var key in ModelState.Keys)
            {
                // Only send the errors to the client.
                if (ModelState[key].Errors.Count > 0)
                {
                    errors[key] = ModelState[key].Errors;
                }
            }

            return errors;
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return Request.GetOwinContext().Authentication;
            }
        }

        #endregion

    }
}
