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
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

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
        public async Task<IActionResult> CreateAccount([Bind("IdNumber,FirstName,Surname,Password,Username,Email")] Parent parent)
        {
            bool emailExists = await _context.Parents.AnyAsync(e => e.Email == parent.Email);

            if (emailExists)
            {
                ModelState.AddModelError("", "An account with this email already exists.");
                return View(parent);
            }

            bool username = await _context.Parents.AnyAsync(e => e.Username == parent.Username);

            if (username)
            {
                ModelState.AddModelError("", "An account with this username already exists.");
                return View(parent);
            }

            



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
                                    string password = parent.Password;

                                    byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);

                                    parent.Salt = Convert.ToBase64String(salt);

                                    string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                                        password: password!,
                                        salt: salt,
                                        prf: KeyDerivationPrf.HMACSHA256,
                                        iterationCount: 100000,
                                        numBytesRequested: 256 / 8
                                        ));


                                    parent.Password = hashedPassword;
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
        public async Task<IActionResult> Login([Bind("Username,Password")] Parent parent, Staff staff)
        {
            var selectedRole = Request.Form["role"].ToString();


            
                if (selectedRole == "Teacher" && staff != null)
                {
                    var staffUser = _context.Staffs.FirstOrDefault(s => s.Username == staff.Username);

                    if (staffUser != null)
                    {

                        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, staffUser.Username),
                    new Claim("UserInternalID", staffUser.id.ToString())
                };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                        return RedirectToAction("StaffDashboard", "Dashboards");


                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid username or password");
                        return View(parent);

                    }

                }
                else if (selectedRole == "Parent")
                {

                    var user = _context.Parents.FirstOrDefault(u => u.Username == parent.Username);

                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid username or password");
                    return View(parent);
                }



                    string password = parent.Password;

                    string dbPassword = user.Password;

                    byte[] salt = Convert.FromBase64String(user.Salt);

                    string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: password!,
                        salt: salt,
                        prf: KeyDerivationPrf.HMACSHA256,
                        iterationCount: 100000,
                        numBytesRequested: 256 / 8
                        ));




                    if (hashedPassword == dbPassword)
                    {

                        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim("UserInternalID", user.Id.ToString())
                };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                        return RedirectToAction("ParentDashboard", "Dashboards", new {parentID = user.Id});

                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid username or password");
                        return View(parent);

                    }
                }
            
            ModelState.AddModelError("", "Invalid username or passwords");

            return View(parent);
        }

        
        
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("HomePage", "Home");
        }


    }
}
