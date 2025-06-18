using Microsoft.AspNetCore.Mvc;                // Nécessaire pour créer un contrôleur MVC
using Frontend.Models;                         // Permet d’utiliser la classe Cours
using System.Net.Http;                         // Pour appeler l’API HTTP
using System.Net.Http.Json;                    // Pour envoyer ou recevoir du JSON facilement
using System.Threading.Tasks;                  // Pour gérer les méthodes asynchrones

namespace Frontend.Controllers
{
    public class CoursController : Controller
    {
        private readonly HttpClient _httpClient; // Déclare le client HTTP utilisé pour contacter l'API

        // Constructeur : injecte le client HTTP quand le contrôleur est créé
        public CoursController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(); // On crée une instance de client HTTP
        }

        //Formulaire de creation

        // Affiche un formulaire vide à l’utilisateur
        public IActionResult Create()
        {
            return View(); // Va chercher la vue Views/Cours/Create.cshtml
        }

        // Reçoit les données du formulaire envoyé en POST
        [HttpPost]
        public async Task<IActionResult> Create(Cours cours)
        {
            if (ModelState.IsValid) // Vérifie que les données du formulaire sont valides
            {
                // Envoie le cours à l’API via une requête POST
                var response = await _httpClient.PostAsJsonAsync("http://localhost:7100/api/cours", cours);

                if (response.IsSuccessStatusCode)
                {
                    // Redirige vers la page d’accueil ou de confirmation
                    return RedirectToAction("Index", "Utilisateurs");
                }

                ViewBag.Erreur = "Erreur lors de l’envoi des données à l’API.";
            }

            return View(cours); // En cas d’erreur, on réaffiche le formulaire avec les données
        }

        // Formulaire d emodification

        // Récupère les infos d’un cours à modifier
        public async Task<IActionResult> Edit(int id)
        {
            // Appelle l’API pour obtenir les données d’un cours spécifique
            var cours = await _httpClient.GetFromJsonAsync<Cours>($"http://localhost:7100/api/cours/{id}");

            return View(cours); // Envoie les données à la vue Edit.cshtml
        }

        // Reçoit le formulaire modifié en POST
        [HttpPost]
        public async Task<IActionResult> Edit(Cours cours)
        {
            // Envoie les nouvelles données à l’API via PUT
            var response = await _httpClient.PutAsJsonAsync($"http://localhost:7100/api/cours/{cours.Id}", cours);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Utilisateurs");
            }

            ViewBag.Erreur = "Erreur lors de la mise à jour du cours.";
            return View(cours);
        }
    }
}
