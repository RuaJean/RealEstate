using System;
using RealEstate.Domain.ValueObjects;

namespace RealEstate.Domain.Entities
{
    /// <summary>
    /// Property representa un inmueble con dirección y precio.
    /// </summary>
    public sealed class Property
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public Address Address { get; private set; }
        public Price Price { get; private set; }
        public int Year { get; private set; }
        public double Area { get; private set; }
        public Guid OwnerId { get; private set; }
        public bool Active { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }

        private Property() { }

        public Property(string name, Address address, Price price, int year, double area, Guid ownerId, bool active = true)
        {
            Id = Guid.NewGuid();
            Name = ValidateName(name);
            Address = address ?? throw new ArgumentNullException(nameof(address));
            Price = price ?? throw new ArgumentNullException(nameof(price));
            Year = ValidateYear(year);
            Area = ValidateArea(area);
            OwnerId = ownerId == Guid.Empty ? throw new ArgumentException("OwnerId inválido", nameof(ownerId)) : ownerId;
            Active = active;
            CreatedAtUtc = DateTime.UtcNow;
        }

        public void UpdateBasics(string name, Address address, int year, double area)
        {
            Name = ValidateName(name);
            Address = address ?? throw new ArgumentNullException(nameof(address));
            Year = ValidateYear(year);
            Area = ValidateArea(area);
        }

        public void ChangePrice(Price newPrice)
        {
            Price = newPrice ?? throw new ArgumentNullException(nameof(newPrice));
        }

        public void Deactivate() => Active = false;
        public void Activate() => Active = true;

        private static string ValidateName(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("El nombre es requerido", nameof(value));
            }
            string trimmed = value.Trim();
            if (trimmed.Length > 200)
            {
                throw new ArgumentException("El nombre excede la longitud máxima de 200", nameof(value));
            }
            return trimmed;
        }

        private static int ValidateYear(int year)
        {
            int current = DateTime.UtcNow.Year + 1; // permite próximo año por presale
            if (year < 1800 || year > current)
            {
                throw new ArgumentOutOfRangeException(nameof(year), "Año fuera de rango razonable");
            }
            return year;
        }

        private static double ValidateArea(double area)
        {
            if (area <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(area), "Área debe ser positiva");
            }
            return area;
        }
    }
}


