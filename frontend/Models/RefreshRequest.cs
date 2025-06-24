namespace frontend.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = Guid.NewGuid().ToString();
        public DateTime Expiration { get; set; } = DateTime.UtcNow.AddDays(7);
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public bool IsRevoked { get; set; } = false;
    }
}
