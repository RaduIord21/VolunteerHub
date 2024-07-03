using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using VolunteerHub.Backend.Helpers;
using VolunteerHub.DataModels.Models;


namespace VolunteerHub.Backend.Controllers
{
    [ApiController]
    [Route("api")]
    public class ImpersonationController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly JwtService _jwtService;

        public ImpersonationController(
            UserManager<User> userManager, 
            SignInManager<User> signInManager,
            JwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
        }

        // eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjQ2OTg2MTczLWU4MDQtNGE2ZC04ZWZjLWJmZWI2OWUwMTU1NCIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwiZXhwIjoxNzE1OTc5NjAwLCJpc3MiOiI0Njk4NjE3My1lODA0LTRhNmQtOGVmYy1iZmViNjllMDE1NTQiLCJhdWQiOiJhcGkifQ.UbrafkGbJS_vW2t138zIWcEj8XtjMJCPiEhH6uQ6EIQ
        
        [HttpGet("imp")]
        public async Task<IActionResult> Impersonate([FromQuery]string userId){ 
            try{
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null){
                    return NotFound("No such user");
                }
                var originalUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (originalUserId == null){
                    return BadRequest("Claim is null");
                }
                var roles = await _userManager.GetRolesAsync(user);
                roles.Add("impersionating:" + originalUserId);
                var tokenString = _jwtService.Generate(user.Id, roles);
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(tokenString);
                return Ok(new { token = tokenString });
            }catch (Exception ex){
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("revert")]
        public async Task<IActionResult> Revert()
        {
            var originalUserId = User.FindFirstValue(ClaimTypes.PrimarySid);
            if (originalUserId == null)
            {
                return BadRequest("Not impersonating");
            }

            var originalUser = await _userManager.FindByIdAsync(originalUserId);
            if (originalUser == null)
            {
                return NotFound();
            }

            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(originalUser, isPersistent: false);

            return Ok("success");
        }

        [Authorize]
        [HttpGet("userdata")]
        public async Task<IActionResult> GetImpersonatedUserData()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return BadRequest("Invalid User !!!");
            }

            var resp = new
            {
                user = user.UserName
            };
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(claims);
        }
    }

}
