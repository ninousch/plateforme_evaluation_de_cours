using System;
using System.Collections.Generic;

namespace Frontend.Models
{
    // Ce modèle sert à afficher le statut d'une session de feedback
    public class StatutFeedback
    {
        public int SessionId { get; set; }               // ID de la session de feedback
        public string CoursTitre { get; set; }           // Titre du cours concerné
        public DateTime DateSession { get; set; }        // Date et heure de la session
        public int NombreTotalEtudiants { get; set; }    // Nombre d'étudiants attendus
        public int NombreFeedbacksRecus { get; set; }    // Nombre de feedbacks déjà envoyés
        public List<string> NomsEtudiantsAyantRepondu { get; set; } // Liste des étudiants ayant répondu
    }
}
