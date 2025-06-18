namespace Frontend.Models
        {
            //Clase representant le cours de l'application 
            public class Cours
                {
                    public int Id { get; set; } //Id du cours, unique
                    public string Titre { get; set; }       //Nom ou titre du cours
                    public string Description { get; set; }     //Breve description du contenu
                }

        }