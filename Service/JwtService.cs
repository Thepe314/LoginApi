using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LoginApi.Models;
using Microsoft.IdentityModel.Tokens;

namespace LoginApi.Service
{
    public class JwtService
    {
        //read only
        private readonly IConfiguration _config;
        private readonly string _secretKey;

        private readonly string _issuer;

        private readonly string _audience;

        private readonly int _accessTokenExpiryMinutes;

        //Constructor: DI the Configuration (Reading values from appsettings)
        public JwtService(IConfiguration config)
        {
            _config = config;
            _secretKey = _config["JwtSettings:SecretKey"];
             _issuer = _config["JwtSettings:Issuer"];
            _audience = _config["JwtSettings:Audience"];
            _accessTokenExpiryMinutes = _config.GetValue<int>("JwtSettings:ExpiryMinutes",60);

        }

        //jwt creating function
        public string GenerateToken(User user)
        {
            {
                //Data that we are showing on payload of jwt
                var claims = new []
                {
                    new Claim("UserId",user.Id.ToString()), //Custom Claim
                    new Claim(ClaimTypes.Email,user.Email), //Standard Claim
                    new Claim(ClaimTypes.Role, user.Role) //Standard Claim
                };

                //Creating signing key and credentials
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
                var cred = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);

                //create the new token
                var token = new JwtSecurityToken(
                    issuer: _issuer,
                    audience: _audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(_accessTokenExpiryMinutes),
                    signingCredentials: cred
                );

                //returning the created token
                return new JwtSecurityTokenHandler().WriteToken(token);
                 
            }

        }
    }
}