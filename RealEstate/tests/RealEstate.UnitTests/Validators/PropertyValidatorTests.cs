using System;
using NUnit.Framework;
using RealEstate.Application.DTOs.Property;
using RealEstate.Application.Validators;

namespace RealEstate.UnitTests.Validators
{
    [TestFixture]
    public class PropertyValidatorTests
    {
        [Test]
        public void Create_Invalid_Should_Fail()
        {
            var validator = new PropertyCreateValidator();
            var dto = new PropertyCreateDto
            {
                Name = "",
                Street = "",
                City = "",
                Country = "",
                ZipCode = "",
                Price = -1,
                Currency = "U",
                Year = 1000,
                Area = 0,
                OwnerId = Guid.Empty
            };
            var result = validator.Validate(dto);
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void Create_Valid_Should_Pass()
        {
            var validator = new PropertyCreateValidator();
            var dto = new PropertyCreateDto
            {
                Name = "Casa",
                Street = "Calle",
                City = "Bogotá",
                State = "Cundinamarca",
                Country = "CO",
                ZipCode = "110111",
                Price = 100000,
                Currency = "USD",
                Year = DateTime.UtcNow.Year,
                Area = 50,
                OwnerId = Guid.NewGuid(),
                Active = true
            };
            var result = validator.Validate(dto);
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void PriceUpdate_Invalid_Should_Fail()
        {
            var validator = new PropertyPriceUpdateValidator();
            var dto = new PropertyPriceUpdateDto { Amount = -5, Currency = "" };
            var result = validator.Validate(dto);
            Assert.That(result.IsValid, Is.False);
        }
    }
}


