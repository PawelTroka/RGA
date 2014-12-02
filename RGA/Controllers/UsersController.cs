using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
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
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        private ApplicationUserManager _userManager;
        private EditUserViewModel editUserViewModel;

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
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
            var model = new CreateUserViewModel();
            return View(model);
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create(CreateUserViewModel usersViewModel)
        {
            if (usersViewModel.SelectedDrivers.Any() && usersViewModel.Role != "Pracownik")
            {
                ModelState.AddModelError("SelectedDrivers",
                    "Użytkownik nie może mieć przydzielonych kierowców jeżeli nie jest pracownikiem");
                ModelState.AddModelError("Role",
                    "Użytkownik nie może mieć przydzielonych kierowców jeżeli nie jest pracownikiem");
            }


            var roleStore = new RoleStore<IdentityRole>(db);
            var roleManager = new RoleManager<IdentityRole>(roleStore);

            var userStore = new UserStore<User>(db);
            var userManager = new UserManager<User>(userStore);


            if (usersViewModel.SelectedDrivers.Any(n => userManager.FindByName(n).SupervisorEmployee != null))
                ModelState.AddModelError("SelectedDrivers",
                    "Użytkownik nie może mieć przydzielonych kierowców, którzy już są do kogoś innego przydzieleni");


            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = usersViewModel.Username,
                    Email = usersViewModel.Email,
                    PhoneNumber = usersViewModel.Phone
                };

                IdentityResult result = await UserManager.CreateAsync(user, usersViewModel.Password);

                if (result.Succeeded)
                {
                    user = userManager.FindByName(usersViewModel.Username);
                    userManager.AddToRole(user.Id, usersViewModel.Role);


                    if (usersViewModel.Role == "Pracownik")
                    {
                        user.Drivers = new List<User>();
                        foreach (string driver in usersViewModel.SelectedDrivers)
                        {
                            User drv = userManager.FindByName(driver);
                            if (drv.SupervisorEmployee == null)
                            {
                                drv.SupervisorEmployee = user;
                                user.Drivers.Add(drv);
                            }
                        }
                        //db.Entry(user).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                    return RedirectToAction("Index", db.Users.ToList());
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

            editUserViewModel = new EditUserViewModel {User = user};
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
            if (editUser.SelectedDrivers.Any() && editUser.Role != "Pracownik")
            {
                ModelState.AddModelError("SelectedDrivers",
                    "Użytkownik nie może mieć przydzielonych kierowców jeżeli nie jest pracownikiem");
                ModelState.AddModelError("Role",
                    "Użytkownik nie może mieć przydzielonych kierowców jeżeli nie jest pracownikiem");
            }


            var roleStore = new RoleStore<IdentityRole>(db);
            var roleManager = new RoleManager<IdentityRole>(roleStore);

            var userStore = new UserStore<User>(db);
            var userManager = new UserManager<User>(userStore);

            User user = userManager.FindById(editUser.UserId); //db.Users.Find(editUser.UserId);


            if (
                editUser.SelectedDrivers.Any(
                    n =>
                        userManager.FindByName(n).SupervisorEmployee != null &&
                        userManager.FindByName(n).SupervisorEmployee != user))
                ModelState.AddModelError("SelectedDrivers",
                    "Użytkownik nie może mieć przydzielonych kierowców, którzy już są do kogoś innego przydzieleni");


            if (ModelState.IsValid)
            {
                db.Users.Attach(user);

                string currentRole = userManager.GetRoles(user.Id).First();

                if (currentRole != editUser.Role)
                {
                    userManager.RemoveFromRole(user.Id, currentRole);
                    userManager.AddToRole(user.Id, editUser.Role);
                }


                if (user.Drivers != null)
                {
                    foreach (User driver in user.Drivers)
                    {
                        driver.SupervisorEmployee = null;
                    }
                    user.Drivers.Clear();
                    db.SaveChanges();
                }
                if (editUser.Role == "Pracownik")
                {
                    user.Drivers = new List<User>();
                    foreach (string driver in editUser.SelectedDrivers)
                    {
                        User drv = userManager.FindByName(driver);
                        if (drv.SupervisorEmployee == null)
                        {
                            drv.SupervisorEmployee = user;
                            user.Drivers.Add(drv);
                        }
                    }
                }
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", db.Users.ToList());
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
            user.SupervisorEmployee = null;
            user.Drivers.Clear();
            db.SaveChanges();
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
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}