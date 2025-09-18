using System;
using System.Globalization;

namespace RealEstate.Domain.ValueObjects
{
    /// <summary>
    /// Price es un Value Object inmutable para representar montos monetarios.
    /// Incluye validaciones básicas y asegura no-negatividad.
    /// </summary>
    public sealed record Price
    {
        public decimal Amount { get; }
        public string Currency { get; }

        // Ctor privado para serializadores
        private Price() { }

        public Price(decimal amount, string currency = "USD")
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "El monto no puede ser negativo");
            }

            Currency = ValidateCurrency(currency);
            Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
        }

        public Price IncreaseBy(decimal delta)
        {
            if (delta < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(delta), "El incremento debe ser no negativo");
            }

            return new Price(Amount + delta, Currency);
        }

        public Price DecreaseBy(decimal delta)
        {
            if (delta < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(delta), "La disminución debe ser no negativa");
            }

            if (Amount - delta < 0)
            {
                throw new InvalidOperationException("El precio no puede resultar negativo");
            }

            return new Price(Amount - delta, Currency);
        }

        private static string ValidateCurrency(string? currency)
        {
            if (string.IsNullOrWhiteSpace(currency))
            {
                throw new ArgumentException("La moneda es requerida", nameof(currency));
            }

            string code = currency.Trim().ToUpperInvariant();
            if (code.Length is < 3 or > 4)
            {
                throw new ArgumentException("Código de moneda inválido", nameof(currency));
            }
            return code;
        }

        public override string ToString()
        {
            return string.Create(CultureInfo.InvariantCulture, $"{Currency} {Amount:0.00}");
        }
    }
}


