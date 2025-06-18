using Microsoft.AspNetCore.Mvc;                 // Pour créer un contrôleur MVC
using Frontend.Models;                          // Pour utiliser le modèle StatutFeedback
using System.Net.Http;                          // Pour faire des requêtes HTTP vers l’API
using System.Net.Http.Json;                     // Pour lire le JSON facilement
using System.Collections.Generic;               // Pour les listes
using System.Threading.Tasks;                   // Pour le async/await

namespace Frontend.Controllers
{
    public class StatutController : Controller
    {
        private readonly HttpClient _httpClient;

        // Constructeur : on récupère un client HTTP
        public StatutController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        // Action principale : affiche la liste des statuts
        public async Task<IActionResult> Index()
        {
            // On récupère la liste depuis l’API
            var statuts = await _httpClient.GetFromJsonAsync<List<StatutFeedback>>(
                "http://localhost:7100/api/statuts"
            );

            return View(statuts); // Envoie les données à la vue Views/Statut/Index.cshtml
        }
    }
}
