using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BSE = Business.Security;
using ESE = Entities.Security;

namespace Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(AuthenticationController));

        public IConfiguration Configuration { get; }

        public AuthenticationController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [HttpPost]
        public ActionResult Post([FromBody] Login value)
        {
            try
            {
                BSE.User bUser = new BSE.User();
                ESE.User result = bUser.Login(value.login, value.password);
                if (result != null && result.Id.Length > 0)
                {
                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, result.Id),
                        new Claim("UserId",result.Id)
                    };

                    var key = new SymmetricSecurityKey(Convert.FromBase64String(Configuration["Token:SigningKey"]));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                            issuer: Configuration["Token:Issuer"],
                            audience: Configuration["Token:Audience"],
                            expires: DateTime.UtcNow.AddDays(365),
                            signingCredentials: creds,
                            claims: claims,
                            notBefore: DateTime.UtcNow
                        );

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    });
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }
    }
}