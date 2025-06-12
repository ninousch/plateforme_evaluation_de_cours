using CourseFeedbackAPI.DTOs;

namespace CourseFeedbackAPI.Services
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDTO dto);
        Task<string> LoginAsync(LoginDTO dto);
    }
}
