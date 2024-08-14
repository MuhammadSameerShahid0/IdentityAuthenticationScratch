using IdentityAuthWithScratch.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityAuthWithScratch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _UserManager;
        private readonly RoleManager<IdentityRole> _RoleManager;
        private readonly IConfiguration _Configuration;

        public AccountController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _UserManager = userManager;
            _RoleManager = roleManager;
            _Configuration = configuration;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO register)
        {
            var user = new IdentityUser { UserName = register.UserName };
            user.EmailConfirmed = true;
            var result = await _UserManager.CreateAsync(user, register.Password);
            if (result.Succeeded)
            {
                return Ok(new { message = "User Regsiter Successfully" });
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            var user = await _UserManager.FindByNameAsync(model.UserName);
            if (user != null && await _UserManager.CheckPasswordAsync(user, model.Password))
            {
                var UserRoles = await _UserManager.GetRolesAsync(user);
                var authclaims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                authclaims.AddRange(UserRoles.Select(role => new Claim(ClaimTypes.Role, role)));

                var token = new JwtSecurityToken(
                    issuer : _Configuration["Jwt:Issuer"],
                    expires : DateTime.Now.AddDays(1),
                    claims : authclaims,
                     signingCredentials: new SigningCredentials(
                         new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Configuration["Jwt:Key"]!)),
                    SecurityAlgorithms.HmacSha256));
                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            }
            return Unauthorized();
        }

        [HttpPost("Add-Role")]
        public async Task<IActionResult> AddRole([FromBody] string role)
        {
            if (!await _RoleManager.RoleExistsAsync(role))
            {
                var result = await _RoleManager.CreateAsync(new IdentityRole(role));
                if (result.Succeeded)
                {
                    return Ok(new { message = "Role added successfully" });
                }

                return BadRequest(result.Errors);
            }
            return BadRequest("Role already exists");
        }

        [HttpPost("Assign-Role")]
        public async Task<IActionResult> AssignRole([FromBody] UserRoleDTO role)
        {
            var user = await _UserManager.FindByNameAsync(role.Username);
            if (user == null)
            {
                return BadRequest("User Not Found");
            }
            var result = await _UserManager.AddToRoleAsync(user, role.Role); 
            if (result.Succeeded)
            {
                return Ok(new { message = "Role assigned successfully" });
            }

            return BadRequest(result.Errors);

        }
    }
}
