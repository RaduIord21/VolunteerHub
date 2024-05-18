using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VolunteerHub.DataModels.Models;

namespace VolunteerHub.Backend.Helpers
{
    public class JwtService
    {
        private IConfiguration _config;

        public JwtService(IConfiguration config) 
        {
            _config = config;
        }
        public string Generate(string userId, IList<string> roles, IDictionary<string, string>? additionalItems = null)
        {
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId),
            };

            if(additionalItems != null)
            {
                //claims.AddRange(additionalClaims);
                foreach (var keyValuePair in additionalItems)
                {
                    claims.Add(new Claim(ClaimTypes.Role, keyValuePair.Value));
                }
            }
            
            //for each role, add a claim
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var header  = new JwtHeader(credentials);
            var payload = new JwtPayload(userId, "api", claims, null, DateTime.UtcNow.AddDays(1));
            var securityToken = new JwtSecurityToken(header, payload);
            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }

        public bool Verify(string jwt)
        {
            var tokenHandler = new JwtSecurityTokenHandler();   
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
            var cp = tokenHandler.ValidateToken(jwt, new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false
            }, out SecurityToken validatedToken) ;
            //return ((JwtSecurityToken)validatedToken);
            return cp.HasClaim(ClaimTypes.Role, "coordonator");
        }
    }
}
