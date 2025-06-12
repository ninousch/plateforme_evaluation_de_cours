using CourseFeedbackAPI.DTOs;
using CourseFeedbackAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CourseFeedbackAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly string[] rolesValides = { "Admin", "Etudiant", "Enseignant" };

        public AuthService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
        }

        public async Task<string> RegisterAsync(RegisterDTO dto)
        {
            if (!rolesValides.Contains(dto.Role))
                throw new Exception("Rôle invalide");

            if (!await _roleManager.RoleExistsAsync(dto.Role))
                await _roleManager.CreateAsync(new IdentityRole(dto.Role));

            var user = new User
            {
                UserName = dto.Email,
                Email = dto.Email,
                Nom = dto.Nom,
                Prenom = dto.Prenom,
                Role = dto.Role
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, dto.Role);
            return await GenerateToken(user);
        }

        public async Task<string?> LoginAsync(LoginDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return null;

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!isPasswordValid) return null;

            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Email, user.Email!),
        new Claim(ClaimTypes.Role, roles.First()) // Utilise le rôle réel attribué
    };

            var keyString = _config["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(keyString))
            {
                Console.WriteLine("[ERREUR JWT] Jwt:Key est null ou vide !");
                return null;
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            if (string.IsNullOrWhiteSpace(tokenString))
            {
                Console.WriteLine("[ERREUR JWT] Le token généré est null !");
            }

            return tokenString;
        }


        private async Task<string> GenerateToken(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim("id", user.Id),
                new Claim(ClaimTypes.Role, roles.First())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
