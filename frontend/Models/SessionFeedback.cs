using System;
using System.Collections.Generic;

namespace Frontend.Models
{
    // Ce mod�le repr�sente une session de feedback li�e � un cours
    public class SessionFeedback
    {
        public int Id { get; set; }                  // Identifiant unique de la session
        public int CoursId { get; set; }             // Lien vers le cours concern� (cl� �trang�re)
        public List<int> EtudiantsIds { get; set; }  // Liste des ID des �tudiants concern�s par cette session
        public DateTime Date { get; set; }           // Date et heure de la session
    }
}

/*Note 
 * Ce mod�le doit correspondre � ce que le backend attend.
 * Si le backend utilise un autre nom, qdapter*/
