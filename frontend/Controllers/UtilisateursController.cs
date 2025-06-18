//Ces instructions permettent d'acceder aux fonctionnalites necessaire
using Microsoft.AspNetCore.Mvc; //Fournit la classe Controller
using Frontend.Models; //Permet d'utiliser notre modele Utilisateur
using System.Net.Http.Json; //Pour faire de sappels HTTP vers l'API
using System.Threading.Tasks;   //Pouyr rendre l'appel a l'API asynchrone
using System.Collections.Generic;
using System.ComponentModel.Design;   //Pour utiliser des listes (List<>)

namespace Frontend.Controllers
        {
    //Creation d'un controlleur-gere la logique pour la page utilisateurs 
        public class UtilisateursController : Controller
                {
                    private readonly HttpClient _httpClient; // Objet qu va servir a appeller l'API

                    //Reception d'un client HTTP par le constructeur
                    public UtilisateursController(IHttpClientFactory httpClientFactory)
                            {
                                 _httpClient = httpClientFactory.CreateClient();
                            }
                    //Creation de la methode utilisee quand l'utilisateurs accede a /Utilisateurs 
                    public async Task<IActionResult> Index()
                            {
                                //Appel de l'API pour recuperer une liste d'utilisateurs 
                                var utilisateurs = await _httpClient.GetFromJsonAsync<List<Utilisateur>>
                                    (
                                       "http://localhost:5000/api/utilisateurs"
                                    );

                                    //On envoie cette liste a la vue Index.cshtml
                                    return View(utilisateurs);
                            }

                    }
}
