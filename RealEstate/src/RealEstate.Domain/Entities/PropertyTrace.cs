using System;
using RealEstate.Domain.ValueObjects;

namespace RealEstate.Domain.Entities
{
    /// <summary>
    /// Traza de operaciones de un inmueble (por ejemplo, cambios de precio o ventas).
    /// </summary>
    public sealed class PropertyTrace
    {
        public Guid Id { get; private set; }
        public Guid PropertyId { get; private set; }
        public DateTime DateUtc { get; private set; }
        public string Description { get; private set; } = string.Empty;
        public Price Value { get; private set; } = null!;

        private PropertyTrace() { }

        public PropertyTrace(Guid propertyId, DateTime dateUtc, string description, Price value)
        {
            Id = Guid.NewGuid();
            PropertyId = propertyId == Guid.Empty ? throw new ArgumentException("PropertyId inválido", nameof(propertyId)) : propertyId;
            DateUtc = ValidateDate(dateUtc);
            Description = ValidateDescription(description);
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        private static DateTime ValidateDate(DateTime dateUtc)
        {
            if (dateUtc.Kind == DateTimeKind.Unspecified)
            {
                // Normalizamos a UTC si no está especificado
                dateUtc = DateTime.SpecifyKind(dateUtc, DateTimeKind.Utc);
            }
            if (dateUtc > DateTime.UtcNow.AddDays(1))
            {
                throw new ArgumentOutOfRangeException(nameof(dateUtc), "La fecha no puede estar muy en el futuro");
            }
            return dateUtc.ToUniversalTime();
        }

        private static string ValidateDescription(string? description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                return string.Empty;
            }
            string trimmed = description.Trim();
            if (trimmed.Length > 500)
            {
                throw new ArgumentException("Descripción demasiado larga", nameof(description));
            }
            return trimmed;
        }
    }
}


