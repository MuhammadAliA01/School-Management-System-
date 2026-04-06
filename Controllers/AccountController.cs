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

        // Add this so the page can actually load!
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var parent = await _context.Parents.FirstOrDefaultAsync(p => p.Email == email);

            if (parent != null)
            {
                string token = Guid.NewGuid().ToString();

                parent.ResetToken = token;
                parent.ResetTokenExpiry = DateTime.Now.AddHours(1);
                await _context.SaveChangesAsync();

                var resetLink = Url.Action("ResetPassword", "Account", new { token = token }, Request.Scheme);

                await SendResetEmail(parent.Email, resetLink);
            }

            ViewBag.Message = "If an account exists with that email, a reset link has been sent.";
            return View();
        }
        private async Task SendResetEmail(string email, string resetLink)
        {
            using (var client = new System.Net.Mail.SmtpClient("Smtp.gmail.com", 587))
            {
                client.EnableSsl = true;
                // Use the same credentials you used in the EmploymentController (call)
                client.Credentials = new System.Net.NetworkCredential("Hybridlearnacademy05@gmail.com", "utxw xbwc dsik yysx");

                var mailMessage = new System.Net.Mail.MailMessage();
                mailMessage.From = new System.Net.Mail.MailAddress("Hybridlearnacademy05@gmail.com");
                mailMessage.To.Add(email);
                mailMessage.Subject = "Reset Your Password - Hybrid Learning Academy";

                // Email msg to user with the reset link and instructions
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = $@"
            <h2>Password Reset Request</h2>
            <p>You requested to reset your password. Please click the link below to set a new one:</p>
            <p><a href='{resetLink}'>Click here to reset your password</a></p>
            <p>This link will expire in 1 hour.</p>
            <p>If you didn't request this, you can safely ignore this email.</p>";

                await client.SendMailAsync(mailMessage);
            }
        }
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login");

            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string token, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
                ViewBag.Token = token;
                return View();
            }

            if (!Regex.IsMatch(newPassword, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$"))
            {
                ModelState.AddModelError("", "Password must be 8+ chars with Upper, Lower, and Number.");
                ViewBag.Token = token;
                return View();
            }

            var parent = await _context.Parents.FirstOrDefaultAsync(p => p.ResetToken == token && p.ResetTokenExpiry > DateTime.Now);

            if (parent == null)
            {
                ModelState.AddModelError("", "Invalid or expired reset link.");
                return View();
            }

            byte[] newSalt = RandomNumberGenerator.GetBytes(128 / 8);
            parent.Salt = Convert.ToBase64String(newSalt);

            parent.Password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: newPassword,
                salt: newSalt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8
            ));

            parent.ResetToken = null;
            parent.ResetTokenExpiry = null;

            await _context.SaveChangesAsync();
            return RedirectToAction("Login");
        }
    }
}
