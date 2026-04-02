using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagmentSystem.Models;

namespace SchoolManagmentSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDBContext _context;

        public AccountController(ApplicationDBContext context)
        {
            _context = context;
        }


        // GET: Account/Create
        public IActionResult CreateAccount()
        {
            return View();
        }

        // POST: Account/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAccount([Bind("IdNumber,FirstName,Surname,Username,Email,Password")] Parent parent)
        {
            if (ModelState.IsValid)
            {
                    if (Regex.IsMatch(parent.Email, "(?=.*[@])"))
                    {

                        if (Regex.IsMatch(parent.Password, "(?=.*[A-Z])"))
                        {
                            if (Regex.IsMatch(parent.Password, "(?=.*[a-z])"))
                            {
                                if (Regex.IsMatch(parent.Password, "(?=.*\\d)"))
                                {
                                    if (Regex.IsMatch(parent.Password, ".{8,}"))
                                    {
                                        _context.Add(parent);
                                        await _context.SaveChangesAsync();
                                        return RedirectToAction("HomePage", "Home");
                                    }
                                    else
                                    {
                                        ModelState.AddModelError("", "Password must be at least 8 characters long");
                                    }
                                }
                                else
                                {
                                    ModelState.AddModelError("", "Password must contain at least one digit");
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "Password must contain at least one lowercase character");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Password must contain at least one uppercase character");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Enter a valid email");
                    }

            }
            return View(parent);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Parent parent)
        {

            //var user = _context.Parents.FirstOrDefault(u => u.Username == parent.Username);

            /*if (user != null)
            {

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim("UserInternalID", user.Id.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("HomePage", "Home");


            }*/

            ModelState.AddModelError("", "Invalid username or password");

            return View(parent);
        }

        
        
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("HomePage", "Home");
        }


    }
}
