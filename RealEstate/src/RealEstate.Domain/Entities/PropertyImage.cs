using System;

namespace RealEstate.Domain.Entities
{
    /// <summary>
    /// Imagen asociada a un inmueble.
    /// </summary>
    public sealed class PropertyImage
    {
        public Guid Id { get; private set; }
        public Guid PropertyId { get; private set; }
        public string Url { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public bool Enabled { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }

        private PropertyImage() { }

        public PropertyImage(Guid propertyId, string url, string description = "", bool enabled = true)
        {
            Id = Guid.NewGuid();
            PropertyId = propertyId == Guid.Empty ? throw new ArgumentException("PropertyId inválido", nameof(propertyId)) : propertyId;
            Url = ValidateUrl(url);
            Description = ValidateDescription(description);
            Enabled = enabled;
            CreatedAtUtc = DateTime.UtcNow;
        }

        public void Update(string url, string description)
        {
            Url = ValidateUrl(url);
            Description = ValidateDescription(description);
        }

        public void Disable() => Enabled = false;
        public void Enable() => Enabled = true;

        private static string ValidateUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("La URL es requerida", nameof(url));
            }
            string trimmed = url.Trim();
            if (trimmed.Length > 1000)
            {
                throw new ArgumentException("URL demasiado larga", nameof(url));
            }
            if (!Uri.TryCreate(trimmed, UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                throw new ArgumentException("URL inválida", nameof(url));
            }
            return trimmed;
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


