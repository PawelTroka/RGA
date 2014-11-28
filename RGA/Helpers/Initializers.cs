
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RGA.Models;

namespace RGA.Helpers
{
    public class UsersInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
                var roles = new List<IdentityRole>()
                {
                    new IdentityRole() {Name = "Admin"},
                    new IdentityRole() {Name = "Pracownik"},
                    new IdentityRole() {Name = "Kierowca"}
                };


                var users = new List<User>
                {
                    new User() { UserName = "Pawel",   Email = "pawtroka@student.pg.gda.pl",PhoneNumber = "+48-725-656-424",PasswordHash = "ABUJMtvUdcD84FMPwNWPeNSyRWRxZqTEsyTkeZCB6Ims2ga4ka5UXur0qu+0k1PL0Q=="},
                    new User() { UserName = "Lukasz",   Email = "lukadryc@student.pg.gda.pl",PhoneNumber = "+48-518-133-434",PasswordHash = "ABUJMtvUdcD84FMPwNWPeNSyRWRxZqTEsyTkeZCB6Ims2ga4ka5UXur0qu+0k1PL0Q=="},

                    new User() { UserName = "DarekKierowiec",   Email = "kierowczatirra@wp.pl",PhoneNumber = "+48-642-411-111",PasswordHash = "ABUJMtvUdcD84FMPwNWPeNSyRWRxZqTEsyTkeZCB6Ims2ga4ka5UXur0qu+0k1PL0Q=="},
                    new User() { UserName = "BillSzybki",   Email = "szybki_bill@gmail.com",PhoneNumber = "+48-666-123-987",PasswordHash = "ABUJMtvUdcD84FMPwNWPeNSyRWRxZqTEsyTkeZCB6Ims2ga4ka5UXur0qu+0k1PL0Q=="},
                    new User() { UserName = "JanTirowiec",   Email = "kierowczatirra@wp.pl",PhoneNumber = "+48-612-132-246",PasswordHash = "ABUJMtvUdcD84FMPwNWPeNSyRWRxZqTEsyTkeZCB6Ims2ga4ka5UXur0qu+0k1PL0Q=="},
                    new User() { UserName = "Zbychu",   Email = "zbychtoja@gmail.com",PhoneNumber = "+48-756-111-111",PasswordHash = "ABUJMtvUdcD84FMPwNWPeNSyRWRxZqTEsyTkeZCB6Ims2ga4ka5UXur0qu+0k1PL0Q=="},

                    new User() { UserName = "HonorataPracowita",   Email = "honorcia@amorki.pl",PhoneNumber = "+48-777-222-333",PasswordHash = "ABUJMtvUdcD84FMPwNWPeNSyRWRxZqTEsyTkeZCB6Ims2ga4ka5UXur0qu+0k1PL0Q=="},
                    new User() { UserName = "CichaKasia",   Email = "kasia@poczta.gda.pl",PhoneNumber = "+48-717-123-111",PasswordHash = "ABUJMtvUdcD84FMPwNWPeNSyRWRxZqTEsyTkeZCB6Ims2ga4ka5UXur0qu+0k1PL0Q=="},
                    new User() { UserName = "ZenonPiekny",   Email = "topmodel2014@poczta.onet.pl",PhoneNumber = "+48-666-123-111",PasswordHash = "ABUJMtvUdcD84FMPwNWPeNSyRWRxZqTEsyTkeZCB6Ims2ga4ka5UXur0qu+0k1PL0Q=="},
                                        new User() { UserName = "Jur",   Email = "duzyjur@poczta.onet.pl",PhoneNumber = "+48-666-666-666",PasswordHash = "ABUJMtvUdcD84FMPwNWPeNSyRWRxZqTEsyTkeZCB6Ims2ga4ka5UXur0qu+0k1PL0Q=="}
                };


                users.Find((u) => u.UserName == "HonorataPracowita").Drivers = new List<User>() { users.Find((u) => u.UserName == "DarekKierowiec"), users.Find((u) => u.UserName == "BillSzybki") };

                users.Find((u) => u.UserName == "CichaKasia").Drivers = new List<User>() { users.Find((u) => u.UserName == "JanTirowiec") };

                roles.ForEach(r => context.Roles.Add(r));
                users.ForEach(s => context.Users.Add(s));
                context.SaveChanges();

                Roles.AddUserToRole("Pawel", "Admin");
                Roles.AddUserToRole("Lukasz", "Admin");

                Roles.AddUserToRole("DarekKierowiec", "Kierowca");
                Roles.AddUserToRole("BillSzybki", "Kierowca");
                Roles.AddUserToRole("JanTirowiec", "Kierowca");
                Roles.AddUserToRole("Zbychu", "Kierowca");

                Roles.AddUserToRole("HonorataPracowita", "Pracownik");
                Roles.AddUserToRole("CichaKasia", "Pracownik");
                Roles.AddUserToRole("ZenonPiekny", "Pracownik");

                context.SaveChanges();
        }
    }
}