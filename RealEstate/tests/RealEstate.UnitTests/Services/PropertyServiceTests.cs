using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using RealEstate.Application.DTOs.Property;
using RealEstate.Application.Interfaces.Repositories;
using RealEstate.Application.Profiles;
using RealEstate.Application.Services;
using RealEstate.Domain.Entities;
using RealEstate.Domain.ValueObjects;

namespace RealEstate.UnitTests.Services
{
    [TestFixture]
    public class PropertyServiceTests
    {
        private Mock<IPropertyRepository> _repo = null!;
        private IMapper _mapper = null!;
        private PropertyService _sut = null!;

        [SetUp]
        public void SetUp()
        {
            _repo = new Mock<IPropertyRepository>(MockBehavior.Strict);
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile())).CreateMapper();
            _sut = new PropertyService(_repo.Object, _mapper);
        }

        [TearDown]
        public void TearDown()
        {
            _repo.VerifyAll();
        }

        [Test]
        public async Task CreateAsync_Should_Persist_And_Map()
        {
            var dto = new PropertyCreateDto
            {
                Name = "Casa",
                Street = "Calle",
                City = "Bogotá",
                State = "Cund",
                Country = "CO",
                ZipCode = "110111",
                Price = 100,
                Currency = "USD",
                Year = 2020,
                Area = 80,
                OwnerId = Guid.NewGuid(),
                Active = true
            };
            _repo.Setup(r => r.CreateAsync(It.IsAny<Property>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Property p, CancellationToken _) => p);

            var res = await _sut.CreateAsync(dto, CancellationToken.None);
            Assert.That(res.Name, Is.EqualTo("Casa"));
            Assert.That(res.Price, Is.EqualTo(100));
        }

        [Test]
        public async Task GetByIdAsync_NotFound_Returns_Null()
        {
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Property?)null);
            var res = await _sut.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);
            Assert.That(res, Is.Null);
        }

        [Test]
        public async Task UpdatePriceAsync_Delegates_To_Repo()
        {
            var id = Guid.NewGuid();
            _repo.Setup(r => r.UpdatePriceAsync(id, It.IsAny<Price>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            var ok = await _sut.UpdatePriceAsync(id, new PropertyPriceUpdateDto { Amount = 5, Currency = "USD" }, CancellationToken.None);
            Assert.That(ok, Is.True);
        }
    }
}


