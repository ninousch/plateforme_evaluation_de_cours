using System;
using System.Collections.Generic;

namespace Frontend.Models
{
    // Ce modèle représente une session de feedback liée à un cours
    public class SessionFeedback
    {
        public int Id { get; set; }                  // Identifiant unique de la session
        public int CoursId { get; set; }             // Lien vers le cours concerné (clé étrangère)
        public List<int> EtudiantsIds { get; set; }  // Liste des ID des étudiants concernés par cette session
        public DateTime Date { get; set; }           // Date et heure de la session
    }
}

/*Note 
 * Ce modèle doit correspondre à ce que le backend attend.
 * Si le backend utilise un autre nom, qdapter*/
