using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VolunteerHub.DataModels.Models;

namespace VolunteerHub.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public TestController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet("test")]
        public IActionResult Testone([FromQuery] string userId)
        {
            try
            {
                var user = _userManager.FindByIdAsync(userId);
                if (user.Result == null)
                {
                    return NotFound();
                }
                var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (claim == null)
                {
                    return BadRequest("Claim is null");
                }
                var impersonationClaims = new List<Claim>
                {

                    new("ImpersonatorId", claim)
                };

                var impersonationIdentity = new ClaimsIdentity(impersonationClaims);
                var userPrincipal = _signInManager.CreateUserPrincipalAsync(user.Result);
                userPrincipal.Result.AddIdentity(impersonationIdentity);

                _signInManager.SignOutAsync();
                _signInManager.Context.SignInAsync(IdentityConstants.ApplicationScheme, userPrincipal.Result);

                return Ok("Index");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
