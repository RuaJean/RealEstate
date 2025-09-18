using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using RealEstate.Application.DTOs.PropertyTrace;
using RealEstate.Application.Interfaces.Repositories;
using RealEstate.Application.Profiles;
using RealEstate.Application.Services;
using RealEstate.Domain.Entities;
using RealEstate.Domain.ValueObjects;

namespace RealEstate.UnitTests.Services
{
    [TestFixture]
    public class PropertyTraceServiceTests
    {
        private Mock<IPropertyTraceRepository> _repoMock = null!;
        private IMapper _mapper = null!;
        private PropertyTraceService _sut = null!;

        [SetUp]
        public void SetUp()
        {
            _repoMock = new Mock<IPropertyTraceRepository>(MockBehavior.Strict);
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
            _mapper = config.CreateMapper();
            _sut = new PropertyTraceService(_repoMock.Object, _mapper);
        }

        [TearDown]
        public void TearDown()
        {
            _repoMock.VerifyAll();
        }

        [Test]
        public async Task CreateAsync_PersistsEntityAndReturnsResponse()
        {
            var dto = new PropertyTraceCreateDto
            {
                PropertyId = Guid.NewGuid(),
                DateUtc = DateTime.UtcNow,
                Description = "Cambio de precio",
                Value = 123.45m,
                Currency = "USD"
            };

            _repoMock.Setup(r => r.CreateAsync(It.IsAny<PropertyTrace>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((PropertyTrace e, CancellationToken _) => e);

            var result = await _sut.CreateAsync(dto, CancellationToken.None);

            Assert.That(result.PropertyId, Is.EqualTo(dto.PropertyId));
            Assert.That(result.Value, Is.EqualTo(dto.Value));
            Assert.That(result.Currency, Is.EqualTo(dto.Currency));
        }

        [Test]
        public async Task GetByPropertyIdAsync_ReturnsMappedList()
        {
            var propertyId = Guid.NewGuid();
            var list = new List<PropertyTrace>
            {
                new PropertyTrace(propertyId, DateTime.UtcNow, "desc", new Price(10, "USD")),
                new PropertyTrace(propertyId, DateTime.UtcNow.AddMinutes(-5), "desc2", new Price(20, "USD"))
            };

            _repoMock.Setup(r => r.GetByPropertyIdAsync(propertyId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(list);

            var result = await _sut.GetByPropertyIdAsync(propertyId, CancellationToken.None);

            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0].PropertyId, Is.EqualTo(propertyId));
        }
    }
}


