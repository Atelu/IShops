using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using IShops.Data;
using Microsoft.AspNetCore.Authorization;
using IShops.Models;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using IShops.Services;

namespace IShops.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("AllowAllOrigins")]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUserService _userService;
        private readonly RoleManager<IdentityRole> _roleManager;


        private DateTime TokenExpire { get; set; }
        public AccountController(
           UserManager<IdentityUser> userManager,
           SignInManager<IdentityUser> signInManager,
           IConfiguration configuration,
           RoleManager<IdentityRole> roleManager,
           ApplicationDbContext applicationDbContext,
           IUserService userService
           )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _applicationDbContext = applicationDbContext;
            _userService = userService;
            _roleManager = roleManager;
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<object> Login([FromBody]LoginModel model)
        {
            var getEmp = _applicationDbContext.UserInfo.SingleOrDefault(u => u.Email == model.Email);
            if (getEmp != null)
            {
                if (getEmp.CanLogin == false)
                {
                    return StatusCode(200, new
                    {
                        status = 104,
                        message = "account has been blocked, please contact admin"
                    });
                }
              
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
            if (result.Succeeded)
            {
                var user = _userManager.Users.SingleOrDefault(r => r.Email == model.Email);
                var token = GenerateJwtToken(user.Email, user);
                var role = new object();
                var employee = new UserInfo();
                try
                {
                    var getrole = _applicationDbContext.UserRoles.SingleOrDefault(u => u.UserId == user.Id);
                    if (getrole != null)
                    {
                        role = _applicationDbContext.Roles.SingleOrDefault(u => u.Id == getrole.RoleId);
                    }

                  


                    var checkPrevious = _applicationDbContext.Tokens.AsNoTracking().SingleOrDefault(u => u.AccountId == user.Id);
                    if (checkPrevious == null)
                    {
                        var newToken = new UserTokens
                        {
                            Id = KeyGen.Generate(),
                            AccountId = user.Id,
                            Expiry = TokenExpire,
                            Token = token.ToString()
                        };

                        await _applicationDbContext.AddAsync(newToken);
                        await _applicationDbContext.SaveChangesAsync();
                    }
                    else
                    {
                        checkPrevious.Token = token.ToString();
                        checkPrevious.Expiry = TokenExpire;

                        _applicationDbContext.Entry(checkPrevious).State = EntityState.Modified;
                        _applicationDbContext.Update(checkPrevious);
                        await _applicationDbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        status = 500,
                        message = ex.Message
                    });
                }
                try
                {
                    if (!await _userManager.IsInRoleAsync(user, "System Administrator"))
                    {
                        return StatusCode(200, new
                        {
                            status = 102,
                            message = "Log in successful",
                            data = new
                            {
                                user = new
                                {
                                    id = user.Id,
                                    username = user.UserName,
                                    email = user.Email,
                                },
                                employee,
                                role,
                                accesstoken = new
                                {
                                    token,
                                    expire = TokenExpire
                                },
                                
                            }
                        });
                    }

                    return StatusCode(200, new
                    {
                        status = 102,
                        message = "Log in successful",
                        data = new
                        {
                            user = new
                            {
                                id = user.Id,
                                username = user.UserName,
                                email = user.Email,
                            },
                            employee,
                            role,
                            accesstoken = new
                            {
                                token,
                                expire = TokenExpire
                            },
                            
                        }
                    });

                }
                catch (Exception ex)
                {
                    return StatusCode(200, new
                    {
                        status = 104,
                        message = ex.Message
                    });
                }
            }
            else
            {
                return StatusCode(200, new
                {
                    status = "FAILED",
                    message = "email or password incorrect"
                });
            }
        }

        [HttpPost]
        public async Task<object> Register([FromBody] RegisterModel model)
        {
            var date = DateTime.UtcNow;
            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (await _roleManager.RoleExistsAsync("System Administrator") == false)
            {
                await _roleManager.CreateAsync(new IdentityRole { Id = Guid.NewGuid().ToString(), Name = "System Administrator" });
            }
            if (await _roleManager.RoleExistsAsync("User") == false)
            {
                await _roleManager.CreateAsync(new IdentityRole { Id = Guid.NewGuid().ToString(), Name = "User" });
            }
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");

                var Employee = new UserInfo
                {
                    Id = KeyGen.Generate(),
                    AccountId = user.Id,
                    CanLogin = true,
                    CreateTime = date,
                    FirstName = model.FirstName,
                    IsActive = true,
                    LastName = model.LastName,
                    CreateUserId = user.Id,
                    Phone = model.Phone,
                    Address = model.Address,

                };

                var employee = await _userService.Add(Employee);
                if (employee != "Ok")
                {
                    return NotFound(employee);
                }



                await _signInManager.SignInAsync(user, false);
                var employ = await _userService.GetUserDetails(user.Id);
                var id = _applicationDbContext.UserRoles.SingleOrDefault(u => u.UserId == user.Id);
                var role = _applicationDbContext.Roles.SingleOrDefault(u => u.Id == id.RoleId);
                return Ok(new
                {
                    user = new
                    {
                        id = user.Id,
                        username = user.UserName,
                        email = user.Email,
                        role = role.Name
                    },
                    employ,
                    accesstoken = new
                    {
                        token = GenerateJwtToken(user.Email, user),
                        expire = TokenExpire
                    }
                });
            }

            throw new ApplicationException("UNKNOWN_ERROR");
        }


        private object GenerateJwtToken(string email, IdentityUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            TokenExpire = DateTime.Now.AddDays(1);
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtIssuer"],
                audience: _configuration["JwtIssuer"],
                claims: claims,
                expires: TokenExpire,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
