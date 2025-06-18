namespace Frontend.Models
{
    public class Utilisateur
        /*Creation du modele utilkisateurs compose d'un idnetifiant
         * unique , du nom soit le nom complet de l'user, 
         * de son adresse mail, et de son role respectivement 
         * de type int,string,string et string */
        {
            public int Id { get; set; }
            public string Nom { get; set; }
            public string Email { get; set; } 
            public string Role { get; set; }

    }

}