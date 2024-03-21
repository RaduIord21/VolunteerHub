using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace VolunteerHub.Backend.Helpers
{
    public class JwtService
    {
        private readonly string secureKey = "Cheie de securitate (bmw) hello mfs";  
        public string Generate(string id, string role)
        {
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secureKey));
            var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);
            var claims = new List<Claim>
            {
            new Claim(ClaimTypes.Name, id),
            new Claim(ClaimTypes.Role, role)
    };
            var header  = new JwtHeader(credentials);
            var payload = new JwtPayload(id, null, claims, null, DateTime.Today.AddDays(1));
            var securityToken = new JwtSecurityToken(header, payload);
            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }
        public JwtSecurityToken Verify(string jwt)
        {
            var tokenHandler = new JwtSecurityTokenHandler();   
            var key = Encoding.ASCII.GetBytes(secureKey);
            tokenHandler.ValidateToken(jwt, new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false
            }, out SecurityToken validatedToken) ;
            return (JwtSecurityToken)validatedToken;
        }
    }
}
