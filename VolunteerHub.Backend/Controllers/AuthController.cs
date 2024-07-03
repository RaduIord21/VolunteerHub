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

//        [Authorize(Roles ="Admin")]
    
        [HttpGet("AllUsers")]
        public IActionResult GetAllUsers()
        {
            return Ok(_userManager.Users.ToList());
        }


        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) {
                return BadRequest("No user found");
            }
            
            if(changePasswordDto.oldPassword == null)
            {
                return BadRequest("Campurile nu pot fi goale");
            }

            if (changePasswordDto.newPassword == null)
            {
                return BadRequest("Campurile nu pot fi goale");
            }
            if(changePasswordDto.newPassword != changePasswordDto.confirmPassword)
            {
                return BadRequest("Parolele nu corespund");
            }
            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.oldPassword, changePasswordDto.newPassword);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                return Ok("Success");
            }
            return BadRequest("Parola trebuie sa contina minim 8 caractere,o litera mare, o litera mica, o cifra si un simbol");
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto registerDto)
        {
            //preluarea datelor intr-un obiect user
            var u = new User
            {
                UserName = registerDto.Username,
                Email = registerDto.Email
            };
            //verificarea existentei in baza de date
            var user = _userManager.FindByEmailAsync(registerDto.Email);
          
            if (user.Result != null)
            {
                return BadRequest("User Already Exists");
            }
            //adaugarea utilizatorui nou
            var r = _userManager.CreateAsync(u, registerDto.Password);
            if (r.Result == null)
            {
                return BadRequest("Could not be created");
            }
            //incadrarea in rolul de voluntar
            var myUser = _userManager.FindByNameAsync(u.UserName);
            if (myUser.Result != null)
            {   
                var addRole = _userManager.AddToRoleAsync(myUser.Result, Constants.VolunteerRole);
                if (addRole.Result == null)
                {
                    return BadRequest("Nu s-a adaugat rolul");
                }
            }
            // intitializare statistici

            if (myUser.Result == null)
            {
                return BadRequest("Register user not found");
            }
            UserStat stats = new()
            {
                TasksCompleted = 0,
                TasksAsigned = 0,
                UserId = myUser.Result.Id
            };
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
                    var result = await _signInManager.PasswordSignInAsync(loginDto.UserName, loginDto.Password, false, false);
                    if (result.Succeeded)
                    {
                        var user = await _userManager.FindByNameAsync(loginDto.UserName);
                        if (user != null)
                        {
                            var roles = await _userManager.GetRolesAsync(user);

                            var tokenString = this._jwtService.Generate(user.Id, roles);
                            return Ok(new { token = tokenString });     
                        }
                        return BadRequest("Unable to log in");
                    }
                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                    return BadRequest("Invalid user");
                }
                throw new BadHttpRequestException("Invalid data");

            }
            catch (Exception e)
            {
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
                    email = user.Email,
                    roles = userRoles
                };
                return Ok(resp);
            }
            catch (Exception _)
            {
                return BadRequest("Crapa");
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return Ok("Success");
        }
    }
}
