using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CourseFeedbackAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SecretController : ControllerBase
    {
        [Authorize(Roles = "Enseignant")]
        [HttpGet("secret-enseignant")]
        public IActionResult SecretEnseignant()
        {
            var nom = User.FindFirstValue("nom");
            var prenom = User.FindFirstValue("prenom");
            return Ok($"Bienvenue enseignant {prenom} {nom} !");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("secret-admin")]
        public IActionResult SecretAdmin()
        {
            var nom = User.FindFirstValue("nom");
            var prenom = User.FindFirstValue("prenom");
            return Ok($"Bienvenue admin {prenom} {nom} !");
        }
    }
}