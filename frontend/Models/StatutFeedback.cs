using System;
using System.Collections.Generic;

namespace Frontend.Models
{
    // Ce mod�le sert � afficher le statut d'une session de feedback
    public class StatutFeedback
    {
        public int SessionId { get; set; }               // ID de la session de feedback
        public string CoursTitre { get; set; }           // Titre du cours concern�
        public DateTime DateSession { get; set; }        // Date et heure de la session
        public int NombreTotalEtudiants { get; set; }    // Nombre d'�tudiants attendus
        public int NombreFeedbacksRecus { get; set; }    // Nombre de feedbacks d�j� envoy�s
        public List<string> NomsEtudiantsAyantRepondu { get; set; } // Liste des �tudiants ayant r�pondu
    }
}
