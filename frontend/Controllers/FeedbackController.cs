using Microsoft.AspNetCore.Mvc;                  // Pour les contr�leurs MVC
using Frontend.Models;                           // Pour utiliser les mod�les comme Cours ou SessionFeedback
using System.Net.Http;                           // Pour effectuer des requ�tes HTTP
using System.Net.Http.Json;                      // Pour envoyer et lire du JSON
using System.Collections.Generic;                // Pour les listes
using System.Threading.Tasks;                    // Pour le async/await

namespace Frontend.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly HttpClient _httpClient;

        public FeedbackController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();  // Cr�e un client HTTP pour appeler l'API
        }

        // Affiche le formulaire pour cr�er une session
        public async Task<IActionResult> Create()
        {
            // On r�cup�re la liste des cours et des �tudiants depuis l�API
            var coursList = await _httpClient.GetFromJsonAsync<List<Cours>>("http://localhost:7100/api/cours");
            var etudiantsList = await _httpClient.GetFromJsonAsync<List<Utilisateur>>("http://localhost:7100/api/utilisateurs?role=etudiant");

            // On envoie les listes � la vue via ViewBag (simple � ce stade)
            ViewBag.Cours = coursList;
            ViewBag.Etudiants = etudiantsList;

            return View();
        }

        // Envoie du formulaire rempli � l'API pour cr�er une session
        [HttpPost]
        public async Task<IActionResult> Create(SessionFeedback session)
        {
            if (ModelState.IsValid)
            {
                var response = await _httpClient.PostAsJsonAsync("http://localhost:7100/api/sessions", session);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Utilisateurs"); // Redirection apr�s succ�s
                }

                ViewBag.Erreur = "Erreur lors de la cr�ation de la session.";
            }

            // En cas d'erreur ou si ModelState invalide, recharger les listes
            ViewBag.Cours = await _httpClient.GetFromJsonAsync<List<Cours>>("http://localhost:7100/api/cours");
            ViewBag.Etudiants = await _httpClient.GetFromJsonAsync<List<Utilisateur>>("http://localhost:7100/api/utilisateurs?role=etudiant");

            return View(session);
        }
    }
}
