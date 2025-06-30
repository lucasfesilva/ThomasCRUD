using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ThomasCRUD.DTOs.Auth;
using ThomasCRUD.Models;

namespace ThomasCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(CadastrarDTO dto)
        {
            var user = new IdentityUser
            {
                UserName = dto.NomeUsuario,
                Email = dto.EmailUsuario,
                PasswordHash = dto.SenhaUsuario
            };

            var resultado = await _userManager.CreateAsync(user, dto.SenhaUsuario);
            if (!resultado.Succeeded)
                return BadRequest(resultado.Errors);

            return Ok("Usuário cadastrado com sucesso!");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var usuario = await _userManager.FindByEmailAsync(dto.Email);
            if (usuario == null || !await _userManager.CheckPasswordAsync(usuario, dto.Senha))
                return Unauthorized("Usuario ou senha inválidos!");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Email, usuario.Email),
                }),

                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new { token = tokenHandler.WriteToken(token) });
        }
    }
}
