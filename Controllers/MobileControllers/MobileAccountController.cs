using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchoolManagmentSystem.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SchoolManagmentSystem.Controllers.MobileControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobileAccountController : ControllerBase
    {

        private readonly ApplicationDBContext _context;

        public MobileAccountController(ApplicationDBContext context)
        {
            _context = context;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]Models.LoginRequest request)
        {

            string secretKey = "your_super_secret_key_with_at_least_256_bits_for_security";
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


            if (request.Role == "Teacher")
            {

                var staffUser = _context.Staffs.FirstOrDefault(s => s.Username == request.Username);

                if (staffUser == null)
                {
                    return Unauthorized("Invalid username or Password");
                }


                if (staffUser != null)
                {

                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, staffUser.Username),
                    new Claim("UserInternalID", staffUser.id.ToString()),
                    new Claim(ClaimTypes.Role, "Staff")
                };

                    var token = new JwtSecurityToken(
                        issuer: "https://localhost:7205",
                        audience: "https://localhost:7205",
                        claims: claims,
                        notBefore: DateTime.UtcNow,
                        expires: DateTime.UtcNow.AddDays(30),
                        signingCredentials: credentials

                        );

                    var handler = new JwtSecurityTokenHandler();
                    var tokenString = handler.WriteToken(token);

                    return Ok(new
                    {
                        token = tokenString,
                        role = "teacher",
                        staffId = staffUser.id,
                        staffName = staffUser.FirstName
                    });


                }
                

            }
            else if (request.Role == "Parent")
            {

                var user = _context.Parents.FirstOrDefault(s => s.Username == request.Username);


                if (user == null)
                {
                    return Unauthorized("Invalid username or Password");

                }



                string password = request.Password;

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
                    new Claim("UserInternalID", user.Id.ToString()),
                    new Claim(ClaimTypes.Role, "Staff")
                };

                    var token = new JwtSecurityToken(
                        issuer: "https://localhost:7205",
                        audience: "https://localhost:7205",
                        claims: claims,
                        notBefore: DateTime.UtcNow,
                        expires: DateTime.UtcNow.AddDays(30),
                        signingCredentials: credentials

                        );

                    var handler = new JwtSecurityTokenHandler();
                    var tokenString = handler.WriteToken(token);

                    return Ok(new
                    {
                        token = tokenString,
                        role = "parent",
                        staffId = user.Id,
                        staffName = user.FirstName
                    });

                }
                
            }

            return Unauthorized("An error occurred");
        }



        [HttpPost("register")]
        public async Task<IActionResult> CreateAccount([FromBody]Models.RegisterRequest request)
        {
            bool emailExists = await _context.Parents.AnyAsync(e => e.Email == request.email);

            if (emailExists)
            {
                return Unauthorized("An account with this email already exists.");

            }

            bool username = await _context.Parents.AnyAsync(e => e.Username == request.userName);

            if (username)
            {
                return Unauthorized("An account with this username already exists.");
            }





            if (ModelState.IsValid)
            {
                if (Regex.IsMatch(request.email, "(?=.*[@])"))
                {

                    if (Regex.IsMatch(request.password, "(?=.*[A-Z])"))
                    {
                        if (Regex.IsMatch(request.password, "(?=.*[a-z])"))
                        {
                            if (Regex.IsMatch(request.password, "(?=.*\\d)"))
                            {
                                if (Regex.IsMatch(request.password, ".{8,}"))
                                {
                                    string password = request.password;

                                    byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);

                                    request.salt = Convert.ToBase64String(salt);

                                    string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                                        password: password!,
                                        salt: salt,
                                        prf: KeyDerivationPrf.HMACSHA256,
                                        iterationCount: 100000,
                                        numBytesRequested: 256 / 8
                                        ));


                                    request.password = hashedPassword;

                                    var parent = new Parent
                                    {
                                        IdNumber = request.idNumber,
                                        FirstName = request.firstName,
                                        Surname = request.lastName,
                                        Email = request.email,
                                        Username = request.userName,
                                        Password = hashedPassword,
                                        Salt = request.salt
                                    };

                                    _context.Add(parent);
                                    await _context.SaveChangesAsync();
                                    return Ok();
                                }
                                else
                                {
                                    return Unauthorized("Password must be at least 8 characters long");
                                }
                            }
                            else
                            {
                                return Unauthorized("Password must contain at least one digit");
                            }
                        }
                        else
                        {
                            return Unauthorized("Password must contain at least one lowercase character");
                        }
                    }
                    else
                    {
                        return Unauthorized("Password must contain at least one uppercase character");
                    }
                }
                else
                {
                    return Unauthorized("Enter a valid email");
                }

            }
            return Unauthorized("An error occurred");
        }



    }
}
