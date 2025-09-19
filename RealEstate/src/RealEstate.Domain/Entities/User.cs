using System;

namespace RealEstate.Domain.Entities
{
    /// <summary>
    /// User representa a un usuario del sistema para autenticación/autorización.
    /// </summary>
    public sealed class User
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public string Role { get; private set; } = string.Empty;
        public DateTime CreatedAtUtc { get; private set; }

        private User() { }

        public User(string email, string passwordHash, string role)
        {
            Id = Guid.NewGuid();
            Email = ValidateEmail(email);
            PasswordHash = ValidateRequired(passwordHash, nameof(passwordHash), 500);
            Role = ValidateRequired(role, nameof(role), 100);
            CreatedAtUtc = DateTime.UtcNow;
        }

        public void UpdatePassword(string newPasswordHash)
        {
            PasswordHash = ValidateRequired(newPasswordHash, nameof(newPasswordHash), 500);
        }

        public void UpdateRole(string role)
        {
            Role = ValidateRequired(role, nameof(role), 100);
        }

        private static string ValidateRequired(string? value, string paramName, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{paramName} no puede ser nulo o vacío", paramName);
            }
            string trimmed = value.Trim();
            if (trimmed.Length > maxLength)
            {
                throw new ArgumentException($"{paramName} excede la longitud máxima de {maxLength}", paramName);
            }
            return trimmed;
        }

        private static string ValidateEmail(string? email)
        {
            string value = ValidateRequired(email, nameof(email), 200);
            if (!value.Contains('@') || value.StartsWith('@') || value.EndsWith('@'))
            {
                throw new ArgumentException("Email no es válido", nameof(email));
            }
            return value.ToLowerInvariant();
        }
    }
}


