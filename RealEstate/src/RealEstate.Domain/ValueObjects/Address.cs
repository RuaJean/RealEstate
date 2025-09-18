using System;

namespace RealEstate.Domain.ValueObjects
{
    /// <summary>
    /// Address es un Value Object inmutable que representa una dirección postal.
    /// Igualdad por valor y validaciones básicas de no-null/longitud.
    /// </summary>
    public sealed record Address
    {
        public string Street { get; }
        public string City { get; }
        public string State { get; }
        public string Country { get; }
        public string ZipCode { get; }

        // Ctor privado para serializadores
        private Address() { }

        public Address(string street, string city, string state, string country, string zipCode)
        {
            Street = ValidateRequired(street, nameof(street), 200);
            City = ValidateRequired(city, nameof(city), 100);
            State = ValidateOptional(state, 100);
            Country = ValidateRequired(country, nameof(country), 100);
            ZipCode = ValidateRequired(zipCode, nameof(zipCode), 20);
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


