using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using frontend.DTOs;
namespace frontend.DTOs

{
    public class AdminCoursDto
    {
        public int Id { get; set; }
        public string Titre { get; set; }
        public string Description { get; set; }
        public string EnseignantNom { get; set; }

        // ✅ On utilise bien EtudiantDto ici
        public List<EtudiantDto> Etudiants { get; set; }
    }
}
