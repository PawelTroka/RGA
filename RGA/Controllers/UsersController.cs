using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using RGA.Models;
using RGA.Models.ViewModels;

namespace RGA.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private EditUserViewModel editUserViewModel;

        private ApplicationDbContext db = new ApplicationDbContext();

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

        // GET: Users
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        // GET: Users/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            var model = new ManageUsersViewModel();
            model.Drivers = new List<User>();
            return View(model);
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create(ManageUsersViewModel usersViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = new User { UserName = usersViewModel.Username, Email = usersViewModel.Email, PhoneNumber = usersViewModel.Phone };



                var result = await UserManager.CreateAsync(user, usersViewModel.Password);


                if (result.Succeeded)
                {

                    var roleStore = new RoleStore<IdentityRole>(db);
                    var roleManager = new RoleManager<IdentityRole>(roleStore);

                    var userStore = new UserStore<User>(db);
                    var userManager = new UserManager<User>(userStore);
                    userManager.AddToRole(user.Id, usersViewModel.Role);


                    if (usersViewModel.Role == "Pracownik")
                    {
                        if (user.Drivers == null)
                            user.Drivers = new List<User>();
                        foreach (var driver in usersViewModel.Drivers)
                        {
                            user.Drivers.Add(userManager.FindById(driver.Id));
                        }

                    }

                    return View("Index", db.Users.ToList());
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(usersViewModel);
        }

        // GET: Users/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            editUserViewModel = new EditUserViewModel() { UserModel = user };
            return View(editUserViewModel);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditUserViewModel editUser)
        {
            var user = editUser.UserModel;
            if (ModelState.IsValid)
            {
                db.Users.Attach(user);
                user.Roles.Clear();

                var roleStore = new RoleStore<IdentityRole>(db);
                var roleManager = new RoleManager<IdentityRole>(roleStore);

                var userStore = new UserStore<User>(db);
                var userManager = new UserManager<User>(userStore);

                if (user.Role.Name != editUser.ManageUsersViewModel.Role)
                {
                    userManager.RemoveFromRole(user.Id, user.Role.Name);
                    userManager.AddToRole(user.Id, editUser.ManageUsersViewModel.Role);
                }


                if (editUser.ManageUsersViewModel.Role == "Pracownik")
                {
                    foreach (var driver in user.Drivers)
                    {
                        driver.SupervisorEmployee = null;
                    }
                    user.Drivers.Clear();
                    db.SaveChanges();

                    foreach (var driver in editUser.SelectedDriversItems)
                    {
                        var drv = userManager.FindByName(driver);
                        if (drv.SupervisorEmployee == null)
                        {
                            drv.SupervisorEmployee = user;
                            user.Drivers.Add(drv);
                        }

                    }

                }
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return View("Index", db.Users.ToList());
            }
            return View(editUser);
        }

        // GET: Users/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /* protected override void Dispose(bool disposing)
         {
             if (disposing)
             {
                 db.Dispose();
             }
             base.Dispose(disposing);
         }
 */

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}
