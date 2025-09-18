using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using RealEstate.Application.DTOs.PropertyImage;
using RealEstate.Application.Interfaces.Repositories;
using RealEstate.Application.Profiles;
using RealEstate.Application.Services;
using RealEstate.Domain.Entities;

namespace RealEstate.UnitTests.Services
{
    [TestFixture]
    public class PropertyImageServiceTests
    {
        private Mock<IPropertyImageRepository> _repo = null!;
        private IMapper _mapper = null!;
        private PropertyImageService _sut = null!;

        [SetUp]
        public void SetUp()
        {
            _repo = new Mock<IPropertyImageRepository>(MockBehavior.Strict);
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile())).CreateMapper();
            _sut = new PropertyImageService(_repo.Object, _mapper);
        }

        [TearDown]
        public void TearDown()
        {
            _repo.VerifyAll();
        }

        [Test]
        public async Task CreateAsync_Should_Persist_And_Map()
        {
            var dto = new PropertyImageCreateDto { PropertyId = Guid.NewGuid(), Url = "https://x/y.png", Description = "d", Enabled = true };
            _repo.Setup(r => r.CreateAsync(It.IsAny<PropertyImage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((PropertyImage e, CancellationToken _) => e);
            var res = await _sut.CreateAsync(dto, CancellationToken.None);
            Assert.That(res.Url, Is.EqualTo(dto.Url));
        }

        [Test]
        public async Task GetByPropertyIdAsync_Maps_List()
        {
            var pid = Guid.NewGuid();
            _repo.Setup(r => r.GetByPropertyIdAsync(pid, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PropertyImage> { new PropertyImage(pid, "https://x/z.png", "", true) });
            var list = await _sut.GetByPropertyIdAsync(pid, CancellationToken.None);
            Assert.That(list, Has.Count.EqualTo(1));
        }

        [Test]
        public async Task SetEnabledAsync_Delegates_To_Repo()
        {
            var id = Guid.NewGuid();
            _repo.Setup(r => r.SetEnabledAsync(id, true, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            var ok = await _sut.SetEnabledAsync(id, true, CancellationToken.None);
            Assert.That(ok, Is.True);
        }
    }
}


