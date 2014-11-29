using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using RGA.Models;

namespace RGA.Controllers
{
    public class ManageUsersController : Controller
    {
        private CreateUserViewModel model = new CreateUserViewModel();
        private ApplicationDbContext context = ApplicationDbContext.Create();


        // GET: ManageUsers
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            
            return View("Index", model);
        }



        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User { UserName = model.Username, Email = model.Email, PhoneNumber = model.Phone };
                var result = await UserManager.CreateAsync(user, model.Password);


               



                if (result.Succeeded)
                {

                    var roleStore = new RoleStore<IdentityRole>(context);
                    var roleManager = new RoleManager<IdentityRole>(roleStore);

                    var userStore = new UserStore<User>(context);
                    var userManager = new UserManager<User>(userStore);
                    userManager.AddToRole(user.Id, model.Role);
                    // await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    //return RedirectToAction("Index", "Home");
                 ///   model = new CreateUserViewModel();
                    return View("Index", model);
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View("Index", model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }


        [Authorize(Roles = "Admin")]
        // GET: ManageUsers/Delete/5
        public ActionResult Delete(string id)
        {
            //var model = new CreateUserViewModel();
           // var dbContext = ApplicationDbContext.Create();
         //   dbContext.Users.Remove(dbContext.Users.Find(id));
            var user = UserManager.FindById(id);
            var result = UserManager.Delete(user);

           // model = new CreateUserViewModel();
            if (result.Succeeded)
            {

                return View("Index", model);
            }
            AddErrors(result);
            
            return View("Index",model);
        }
    }
}
