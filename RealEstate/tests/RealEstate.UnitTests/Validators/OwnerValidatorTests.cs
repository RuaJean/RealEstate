using FluentValidation;
using NUnit.Framework;
using RealEstate.Application.DTOs.Owner;
using RealEstate.Application.Validators;

namespace RealEstate.UnitTests.Validators
{
    [TestFixture]
    public class OwnerValidatorTests
    {
        [Test]
        public void Create_Invalid_Should_Fail()
        {
            var validator = new OwnerCreateValidator();
            var dto = new OwnerCreateDto { Name = "", Address = "", Photo = new string('a', 600) };
            var result = validator.Validate(dto);
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void Create_Valid_Should_Pass()
        {
            var validator = new OwnerCreateValidator();
            var dto = new OwnerCreateDto { Name = "John", Address = "Main", Photo = "photo.jpg" };
            var result = validator.Validate(dto);
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void Update_Invalid_Should_Fail()
        {
            var validator = new OwnerUpdateValidator();
            var dto = new OwnerUpdateDto { Name = "", Address = "", Photo = new string('b', 600) };
            var result = validator.Validate(dto);
            Assert.That(result.IsValid, Is.False);
        }
    }
}


