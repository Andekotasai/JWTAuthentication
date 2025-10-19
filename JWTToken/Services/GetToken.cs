using JWTToken.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace JWTToken.Services
{
    public class GetToken
    {
        public IConfiguration _config;
       
        public GetToken(IConfiguration _config)
        {
            this._config = _config;
            
           
        }

        public IEnumerable<User> GetAllUsers()
        {
            List<User> user = new List<User>()
            {
            new User{ UserName="Admin",Password="Admin@123",Role="Admin"},
            new User{ UserName="User",Password="User@123",Role="User"}
            };
            return user;
        }



        public string GenerateToken()
        {

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config["JWT:Key"]));

            JwtHeader header = new JwtHeader(new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
            JwtPayload payload = new JwtPayload(issuer:this._config["JWT:issuer"],audience:"",claims:new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub,"Sai"),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            }, expires: DateTime.UtcNow.AddMinutes(20),notBefore:DateTime.UtcNow);

            var token = new JwtSecurityToken(header, payload);
            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
