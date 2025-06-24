using frontend.DTOs;
namespace frontend.DTOs
{
public class SessionDto
{
    public int Id { get; set; }
    public string Nom { get; set; }
    public DateTime Debut { get; set; }
    public DateTime Fin { get; set; }
}
}
