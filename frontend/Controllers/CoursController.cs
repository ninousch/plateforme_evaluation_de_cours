using Microsoft.AspNetCore.Mvc;                // N�cessaire pour cr�er un contr�leur MVC
using Frontend.Models;                         // Permet d�utiliser la classe Cours
using System.Net.Http;                         // Pour appeler l�API HTTP
using System.Net.Http.Json;                    // Pour envoyer ou recevoir du JSON facilement
using System.Threading.Tasks;                  // Pour g�rer les m�thodes asynchrones

namespace Frontend.Controllers
{
    public class CoursController : Controller
    {
        private readonly HttpClient _httpClient; // D�clare le client HTTP utilis� pour contacter l'API

        // Constructeur : injecte le client HTTP quand le contr�leur est cr��
        public CoursController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(); // On cr�e une instance de client HTTP
        }

        //Formulaire de creation

        // Affiche un formulaire vide � l�utilisateur
        public IActionResult Create()
        {
            return View(); // Va chercher la vue Views/Cours/Create.cshtml
        }

        // Re�oit les donn�es du formulaire envoy� en POST
        [HttpPost]
        public async Task<IActionResult> Create(Cours cours)
        {
            if (ModelState.IsValid) // V�rifie que les donn�es du formulaire sont valides
            {
                // Envoie le cours � l�API via une requ�te POST
                var response = await _httpClient.PostAsJsonAsync("http://localhost:7100/api/cours", cours);

                if (response.IsSuccessStatusCode)
                {
                    // Redirige vers la page d�accueil ou de confirmation
                    return RedirectToAction("Index", "Utilisateurs");
                }

                ViewBag.Erreur = "Erreur lors de l�envoi des donn�es � l�API.";
            }

            return View(cours); // En cas d�erreur, on r�affiche le formulaire avec les donn�es
        }

        // Formulaire d emodification

        // R�cup�re les infos d�un cours � modifier
        public async Task<IActionResult> Edit(int id)
        {
            // Appelle l�API pour obtenir les donn�es d�un cours sp�cifique
            var cours = await _httpClient.GetFromJsonAsync<Cours>($"http://localhost:7100/api/cours/{id}");

            return View(cours); // Envoie les donn�es � la vue Edit.cshtml
        }

        // Re�oit le formulaire modifi� en POST
        [HttpPost]
        public async Task<IActionResult> Edit(Cours cours)
        {
            // Envoie les nouvelles donn�es � l�API via PUT
            var response = await _httpClient.PutAsJsonAsync($"http://localhost:7100/api/cours/{cours.Id}", cours);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Utilisateurs");
            }

            ViewBag.Erreur = "Erreur lors de la mise � jour du cours.";
            return View(cours);
        }
    }
}
