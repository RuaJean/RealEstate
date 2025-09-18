namespace RealEstate.Application.DTOs.Auth
{
    public sealed class RegisterRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}


