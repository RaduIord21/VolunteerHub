using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VolunteerHub.Backend.Models;
using VolunteerHub.DataAccessLayer.Interfaces;
using VolunteerHub.DataModels.Models;
using BCrypt.Net;
using System.Security.Cryptography;
using AutoMapper;
using VolunteerHub.Backend.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using VolunteerHub.DataAccessLayer.Repositories;
using System;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Diagnostics;

namespace VolunteerHub.Backend.Controllers
{
    [ApiController]
    [Route("api")]
    public class AuthController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly JwtService _jwtService;
        private readonly IServiceProvider _serviceProvider;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(
            IUserRepository userRepository,
            IMapper mapper,
            JwtService jwtService,
            IServiceProvider serviceProvider,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _jwtService = jwtService;
            _serviceProvider = serviceProvider;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles ="Admin")]
        [HttpGet("AllUsers")]
        public IActionResult GetAllUsers()
        {
            return Ok(_userManager.Users.ToList());
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto registerDto)
        {
            var u = new User
            {
                UserName = registerDto.Username,
                Email = registerDto.Email
            };
            var user = _userManager.FindByNameAsync(registerDto.Username);
            //_userRepository.Add(_mapper.Map<User>(registerDto));
            //_userRepository.Save();
            if (user.Result != null)
            {
                return BadRequest("User Already Exists");
            }
            var r = _userManager.CreateAsync(u, registerDto.Password);
            if (r.Result == null)
            {
                return BadRequest("Could not be created");
            }

            var myUser = _userManager.FindByNameAsync(u.UserName);
            if (myUser.Result != null)
            {
                var addRole = _userManager.AddToRoleAsync(myUser.Result, Constants.VolunteerRole);
                if (addRole.Result == null)
                {
                    return BadRequest("Nu s-a adaugat rolul");
                }
            }
            if(myUser.Result == null)
            {
                return BadRequest("Register user not found");
            }

            UserStat stats = new()
            {
                TasksCompleted = 0,
                TasksAsigned = 0,
                UserId = myUser.Result.Id
            };
            Console.WriteLine("S-a salvat ce a fost trimis din backend");
            return Ok("Success");
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginDto loginDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _signInManager.PasswordSignInAsync(loginDto.UserName, loginDto.Password, loginDto.RememberMe, false);
                    if (result.Succeeded)
                    {
                        var user = await _userManager.FindByNameAsync(loginDto.UserName);
                        if (user != null)
                        {
                            var roles = await _userManager.GetRolesAsync(user);
/*
                            var claims = new List<Claim>
                            {
                                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                                new(ClaimTypes.Name, user.UserName ?? "")
                            };
                            //for each role, add a claim
                            foreach (var claim in roles)
                            {
                                claims.Add(new Claim(ClaimTypes.Role, roles[0]));
                            }


                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);*/


                            /*await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);  // delete old cookie if exist

                            await HttpContext.SignInAsync(
                                CookieAuthenticationDefaults.AuthenticationScheme, 
                                new ClaimsPrincipal(claimsIdentity),
                                  new AuthenticationProperties { IsPersistent = true }                          // remember me
                            );*/

                            var tokenString = this._jwtService.Generate(user.Id, roles);
                            return Ok(new { token = tokenString });
                        }
                        return BadRequest("Unable to log in");
                    }
                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                    return BadRequest("Invalid user");
                }
                return BadRequest("Error :::");

            }
            catch (Exception e)
            {
                Console.Write(e + "EEERRROOOAAARRREEE");
                return BadRequest(e.Message);
            }
        }

        [Authorize]
        [HttpGet("user")]
        public async Task<IActionResult> GetUser()
        {
            try
            {

                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    return BadRequest("Invalid User !!!");
                }
                var userRoles = _userManager.GetRolesAsync(user).Result;
                var resp = new
                {
                    user = user.UserName,
                    roles = userRoles
                };
                return Ok(resp);
            }
            catch (Exception _)
            {
                return BadRequest("Crapa");
            }
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Ok("Success");
        }
    }
}
