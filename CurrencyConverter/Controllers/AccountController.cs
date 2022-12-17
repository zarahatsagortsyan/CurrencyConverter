using CurrencyConverter.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using CurrencyConverter.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace CurrencyConverter.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext db;
        public AccountController(ApplicationDbContext context)
        {
            db = context;
        }

        #region SignInUser

        [HttpGet]
        public IActionResult SignInUser()
        {
            ViewData["Users"] = db.Users.ToList();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignInUser(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                Users users = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (users != null)
                {
                    bool match = BCrypt.Net.BCrypt.Verify(model.Password, users.Password);
                    if (match)
                    {
                        await Authenticate(model.Email);
                        return Redirect("/Home/Index");
                    }
                }
                ModelState.AddModelError("", "Wrong password or e-mail!");
            }
            ViewData["Users"] = db.Users.ToList();

            return View(model);
        }
        #endregion


        #region SignUpUser
        [HttpGet]
        public IActionResult SignUpUser()
        {
            ViewData["Users"] = db.Users.ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUpUser(RegisterViewModel model)
        {
            // bool checkPhone = PhoneNumberChecker.CheckPhoneNumber(model.Phone);
            ViewData["Users"] = db.Users.ToList();

            if (ModelState.IsValid)
            {
                if (model.ConfirmPassword != model.Password)
                {
                    ModelState.AddModelError("", "Wrong password!");
                }
                else
                {
                    Users users = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                    if (users == null)
                    {
                        db.Users.Add(new Users
                        {
                            LastName = model.FirstName,
                            FirstName = model.LastName,
                            Email = model.Email,
                            BaseCur = model.BaseCur,
                            Password = BCrypt.Net.BCrypt.HashPassword(model.Password)
                        });
                        await db.SaveChangesAsync();
                        await Authenticate(model.Email);

                        return Redirect("/Home/Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "User with this e-mail already exists!");
                    }
                }
            }
            return View(model);
        }
        #endregion


        private async Task Authenticate(string userName)
        {
            // ստեղծում ենք մեկ Claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            // ստեղծում ենք ClaimsIdentity օբյեկտ
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // ներբեռնում ենք աուտենտիֆիկացիոն քուքիները
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/Home/Index");
        }
        [Authorize]
        [HttpGet]
        public IActionResult ChangeUserInfo(Guid id)
        {
            ViewData["Users"] = db.Users.ToList();

            if (id == null)
                return BadRequest("Wrong input data!");
            Users users = db.Users.Find(id);
            if (users == null)
                return NotFound();
            return View(users);
        }
        [Authorize]

        [HttpPost]
        public IActionResult ChangeUserInfo(Users users)
        {
            ViewData["Users"] = db.Users.ToList();

            Users user1 = db.Users.FirstOrDefault(e => e.Id == users.Id);
            if (ModelState.IsValid)
            {
                bool match = BCrypt.Net.BCrypt.Verify(users.Password, user1.Password);
                if (match)
                {
                    user1.FirstName = users.FirstName;
                    user1.LastName = users.LastName;
                    user1.BaseCur = users.BaseCur;
                    user1.Email = users.Email;
                    user1.Password = BCrypt.Net.BCrypt.HashPassword(users.Password);
                    db.Entry(user1).State = EntityState.Modified;
                    db.SaveChanges();
                    return Redirect("/Home/Index");
                }
                else
                    ModelState.AddModelError("", "Wrong password!");
            }
            return View(users);
        }

    }
}
