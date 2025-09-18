using System;
using FluentValidation;
using FluentValidation.Results;
using NUnit.Framework;
using RealEstate.Application.DTOs.PropertyImage;
using RealEstate.Application.Validators;

namespace RealEstate.UnitTests.Validators
{
    [TestFixture]
    public class PropertyImageValidatorTests
    {
        private PropertyImageCreateValidator _validator = null!;

        [SetUp]
        public void SetUp()
        {
            _validator = new PropertyImageCreateValidator();
        }

        [Test]
        public void Should_Have_Error_When_PropertyId_Empty()
        {
            var dto = new PropertyImageCreateDto
            {
                PropertyId = Guid.Empty,
                Url = "https://example.com/img.png"
            };
            var result = _validator.Validate(dto);
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<ValidationFailure>(e => e.PropertyName == nameof(dto.PropertyId)));
        }

        [Test]
        public void Should_Have_Error_When_Url_Invalid()
        {
            var dto = new PropertyImageCreateDto
            {
                PropertyId = Guid.NewGuid(),
                Url = "notaurl"
            };
            var result = _validator.Validate(dto);
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<ValidationFailure>(e => e.PropertyName == nameof(dto.Url)));
        }

        [Test]
        public void Should_Pass_When_Valid_Dto()
        {
            var dto = new PropertyImageCreateDto
            {
                PropertyId = Guid.NewGuid(),
                Url = "https://cdn.example.com/img.png",
                Description = new string('a', 20)
            };
            var result = _validator.Validate(dto);
            Assert.That(result.IsValid, Is.True);
        }
    }
}


