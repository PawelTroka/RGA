using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace RGA.Models
{
    // You can add profile data for the user by adding more properties to your User class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class User : IdentityUser
    {
        public User()
        {
            //  userManager = new UserManager<User>(store);
            Drivers = new List<User>();
        }


        // public bool IsBanned { get { UserStore<User> store = new UserStore<User>(new ApplicationDbContext());
        //   UserManager<User> userManager; return userManager.IsLockedOut(this.Id); } }

        public virtual User SupervisorEmployee { get; set; } //jeżeli jest kierowcą to ma przełożonego pracownika
        public virtual ICollection<User> Drivers { get; set; }
        //jeżeli jest pracownikiem to ma podlegajcych mu kierowców 

        public IdentityRole Role
        {
            get
            {
                ApplicationDbContext context = ApplicationDbContext.Create();
                var userManager = new UserManager<User>(new UserStore<User>(context));
                context.Configuration.LazyLoadingEnabled = true;
                if (Roles.Count == 0)
                {
                    userManager.AddToRole(Id, "Kierowca");

                    context.SaveChanges();
                }

                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

                return roleManager.FindByName(userManager.GetRoles(Id).First());
            }
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            ClaimsIdentity userIdentity =
                await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public override string ToString()
        {
            return UserName;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", false)
        {
        }

        // public new System.Data.Entity.DbSet<RGA.Models.User> Users { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Note> Notes { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}