using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using frontend.DTOs;
namespace frontend.Dtos
{
   public class AssignCourseDto
{
    public string EtudiantId { get; set; }  // l’ID de l’étudiant (clé étrangère vers ApplicationUser)
    public int CoursId { get; set; }        // l’ID du cours
}

}