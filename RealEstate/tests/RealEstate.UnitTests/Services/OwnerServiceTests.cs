using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using RealEstate.Application.DTOs.Owner;
using RealEstate.Application.Interfaces.Repositories;
using RealEstate.Application.Profiles;
using RealEstate.Application.Services;
using RealEstate.Domain.Entities;

namespace RealEstate.UnitTests.Services
{
    [TestFixture]
    public class OwnerServiceTests
    {
        private Mock<IOwnerRepository> _repo = null!;
        private IMapper _mapper = null!;
        private OwnerService _sut = null!;

        [SetUp]
        public void SetUp()
        {
            _repo = new Mock<IOwnerRepository>(MockBehavior.Strict);
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile())).CreateMapper();
            _sut = new OwnerService(_repo.Object, _mapper);
        }

        [TearDown]
        public void TearDown()
        {
            _repo.VerifyAll();
        }

        [Test]
        public async Task CreateAsync_Should_Map_And_Persist()
        {
            var dto = new OwnerCreateDto { Name = "John", Address = "Main", Photo = "" };
            _repo.Setup(r => r.CreateAsync(It.IsAny<Owner>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Owner o, CancellationToken _) => o);

            var result = await _sut.CreateAsync(dto, CancellationToken.None);
            Assert.That(result.Name, Is.EqualTo("John"));
            Assert.That(result.Address, Is.EqualTo("Main"));
        }

        [Test]
        public async Task GetByIdAsync_When_Not_Found_Returns_Null()
        {
            _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Owner?)null);
            var res = await _sut.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);
            Assert.That(res, Is.Null);
        }

        [Test]
        public async Task SearchAsync_Maps_List()
        {
            _repo.Setup(r => r.SearchAsync(null, 0, 50, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Owner> { new Owner("A", "B", "") });
            var list = await _sut.SearchAsync(null, 0, 50, CancellationToken.None);
            Assert.That(list, Has.Count.EqualTo(1));
            Assert.That(list[0].Name, Is.EqualTo("A"));
        }

        [Test]
        public async Task UpdateAsync_When_Found_Updates_And_Returns_True()
        {
            var id = Guid.NewGuid();
            var entity = new Owner("A", "B", "");
            _repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
            _repo.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var ok = await _sut.UpdateAsync(id, new OwnerUpdateDto { Name = "AA", Address = "BB", Photo = "" }, CancellationToken.None);
            Assert.That(ok, Is.True);
        }

        [Test]
        public async Task DeleteAsync_Calls_Repository()
        {
            var id = Guid.NewGuid();
            _repo.Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            var ok = await _sut.DeleteAsync(id, CancellationToken.None);
            Assert.That(ok, Is.True);
        }
    }
}


