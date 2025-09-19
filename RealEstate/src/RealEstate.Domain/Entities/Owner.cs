using System;

namespace RealEstate.Domain.Entities
{
    /// <summary>
    /// Owner representa al propietario de inmuebles.
    /// </summary>
    public sealed class Owner
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Address { get; private set; } = string.Empty;
        public string Photo { get; private set; } = string.Empty;
        public DateTime CreatedAtUtc { get; private set; }

        private Owner() { }

        public Owner(string name, string address, string photo)
        {
            Id = Guid.NewGuid();
            Name = ValidateRequired(name, nameof(name), 200);
            Address = ValidateRequired(address, nameof(address), 300);
            Photo = ValidateOptional(photo, 500);
            CreatedAtUtc = DateTime.UtcNow;
        }

        public void Update(string name, string address, string photo)
        {
            Name = ValidateRequired(name, nameof(name), 200);
            Address = ValidateRequired(address, nameof(address), 300);
            Photo = ValidateOptional(photo, 500);
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

        private static string ValidateOptional(string? value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }
            string trimmed = value.Trim();
            if (trimmed.Length > maxLength)
            {
                throw new ArgumentException($"Valor excede la longitud máxima de {maxLength}");
            }
            return trimmed;
        }
    }
}


