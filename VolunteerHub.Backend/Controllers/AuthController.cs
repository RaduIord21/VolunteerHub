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

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto registerDto)
        {
            Console.WriteLine("Apel register");
            _userRepository.Add(_mapper.Map<User>(registerDto));
            _userRepository.Save();
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
                            var role = await _userManager.GetRolesAsync(user);
                            var jwt = _jwtService.Generate(user.Id, role[0]);
                            Response.Cookies.Append("jwt", jwt, new CookieOptions
                            {
                                HttpOnly = true
                            });
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                        return BadRequest("Invalid user");
                    }
                }
            }
            catch (Exception e)
            {

                Console.Write(e + "EEERRROOOAAARRREEE");
            }
            return Ok("Success");
        }
        [HttpGet("user")]
        public new IActionResult User()
        {
            try
            {
                var jwt = Request.Cookies["jwt"];
                var token = _jwtService.Verify(jwt);
                var issuer = token.Issuer;
                var user = _userManager.FindByIdAsync(issuer);
                if (user == null)
                {
                    return BadRequest("Invalid User !!!");
                }
                var userRoles = _userManager.GetRolesAsync(user.Result).Result[0];
                return Ok(user.Result);
            }
            catch (Exception _)
            {
                return BadRequest("Crapa");
            }
        }
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return Ok("Success");
        }
    }
}
