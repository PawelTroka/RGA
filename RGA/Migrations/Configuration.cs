using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RGA.Models;

namespace RGA.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            context.Configuration.LazyLoadingEnabled = true;


            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<User>(new UserStore<User>(context));


            var roles = new List<IdentityRole>
            {
                new IdentityRole {Name = "Admin"},
                new IdentityRole {Name = "Pracownik"},
                new IdentityRole {Name = "Kierowca"}
            };

            roles.ForEach(r => { if (!roleManager.RoleExists(r.Name)) roleManager.Create(r); });
            //       context.SaveChanges();

            var users = new List<User>
            {
                new User
                {
                    UserName = "Pawel",
                    Email = "pawtroka@student.pg.gda.pl",
                    PhoneNumber = "+48-725-656-424",
                    PasswordHash = "ABUJMtvUdcD84FMPwNWPeNSyRWRxZqTEsyTkeZCB6Ims2ga4ka5UXur0qu+0k1PL0Q==",
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new User
                {
                    UserName = "Lukasz",
                    Email = "lukadryc@student.pg.gda.pl",
                    PhoneNumber = "+48-518-133-434",
                    PasswordHash = "ABUJMtvUdcD84FMPwNWPeNSyRWRxZqTEsyTkeZCB6Ims2ga4ka5UXur0qu+0k1PL0Q==",
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new User
                {
                    UserName = "DarekKierowiec",
                    Email = "kierowczatirra@wp.pl",
                    PhoneNumber = "+48-642-411-111",
                    PasswordHash = "ABUJMtvUdcD84FMPwNWPeNSyRWRxZqTEsyTkeZCB6Ims2ga4ka5UXur0qu+0k1PL0Q==",
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new User
                {
                    UserName = "BillSzybki",
                    Email = "szybki_bill@gmail.com",
                    PhoneNumber = "+48-666-123-987",
                    PasswordHash = "ABUJMtvUdcD84FMPwNWPeNSyRWRxZqTEsyTkeZCB6Ims2ga4ka5UXur0qu+0k1PL0Q==",
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new User
                {
                    UserName = "JanTirowiec",
                    Email = "kierowczatirra@wp.pl",
                    PhoneNumber = "+48-612-132-246",
                    PasswordHash = "ABUJMtvUdcD84FMPwNWPeNSyRWRxZqTEsyTkeZCB6Ims2ga4ka5UXur0qu+0k1PL0Q==",
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new User
                {
                    UserName = "Zbychu",
                    Email = "zbychtoja@gmail.com",
                    PhoneNumber = "+48-756-111-111",
                    PasswordHash = "ABUJMtvUdcD84FMPwNWPeNSyRWRxZqTEsyTkeZCB6Ims2ga4ka5UXur0qu+0k1PL0Q==",
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new User
                {
                    UserName = "HonorataPracowita",
                    Email = "honorcia@amorki.pl",
                    PhoneNumber = "+48-777-222-333",
                    PasswordHash = "ABUJMtvUdcD84FMPwNWPeNSyRWRxZqTEsyTkeZCB6Ims2ga4ka5UXur0qu+0k1PL0Q==",
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new User
                {
                    UserName = "CichaKasia",
                    Email = "kasia@poczta.gda.pl",
                    PhoneNumber = "+48-717-123-111",
                    PasswordHash = "ABUJMtvUdcD84FMPwNWPeNSyRWRxZqTEsyTkeZCB6Ims2ga4ka5UXur0qu+0k1PL0Q==",
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new User
                {
                    UserName = "ZenonPiekny",
                    Email = "topmodel2014@poczta.onet.pl",
                    PhoneNumber = "+48-666-123-111",
                    PasswordHash = "ABUJMtvUdcD84FMPwNWPeNSyRWRxZqTEsyTkeZCB6Ims2ga4ka5UXur0qu+0k1PL0Q==",
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new User
                {
                    UserName = "Jur",
                    Email = "duzyjur@poczta.onet.pl",
                    PhoneNumber = "+48-666-666-666",
                    PasswordHash = "ABUJMtvUdcD84FMPwNWPeNSyRWRxZqTEsyTkeZCB6Ims2ga4ka5UXur0qu+0k1PL0Q==",
                    SecurityStamp = Guid.NewGuid().ToString()
                }
            };


            users.Find(u => u.UserName == "HonorataPracowita").Drivers = new List<User>
            {
                users.Find(u => u.UserName == "DarekKierowiec"),
                users.Find(u => u.UserName == "BillSzybki")
            };

            users.Find(u => u.UserName == "CichaKasia").Drivers = new List<User>
            {
                users.Find(u => u.UserName == "JanTirowiec")
            };
            users.ForEach(s => context.Users.AddOrUpdate(s));

            context.SaveChanges();


            userManager.AddToRole(users.Find(u => u.UserName == "Pawel").Id, "Admin");
            userManager.AddToRole(users.Find(u => u.UserName == "Lukasz").Id, "Admin");

            userManager.AddToRole(users.Find(u => u.UserName == "DarekKierowiec").Id, "Kierowca");
            userManager.AddToRole(users.Find(u => u.UserName == "BillSzybki").Id, "Kierowca");
            userManager.AddToRole(users.Find(u => u.UserName == "JanTirowiec").Id, "Kierowca");
            userManager.AddToRole(users.Find(u => u.UserName == "Zbychu").Id, "Kierowca");
            userManager.AddToRole(users.Find(u => u.UserName == "Jur").Id, "Kierowca");

            userManager.AddToRole(users.Find(u => u.UserName == "HonorataPracowita").Id, "Pracownik");
            userManager.AddToRole(users.Find(u => u.UserName == "ZenonPiekny").Id, "Pracownik");
            userManager.AddToRole(users.Find(u => u.UserName == "CichaKasia").Id, "Pracownik");

            context.SaveChanges();
        }
    }
}