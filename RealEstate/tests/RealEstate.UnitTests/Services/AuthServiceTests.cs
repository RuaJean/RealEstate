using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using RealEstate.Application.DTOs.Auth;
using RealEstate.Application.Interfaces.Repositories;
using RealEstate.Application.Interfaces.Security;
using RealEstate.Application.Services;
using RealEstate.Domain.Entities;
using RealEstate.Shared.Exceptions;

namespace RealEstate.UnitTests.Services
{
    [TestFixture]
    public class AuthServiceTests
    {
        private Mock<IUserRepository> _userRepositoryMock = null!;
        private Mock<IPasswordHasher> _passwordHasherMock = null!;
        private Mock<IJwtProvider> _jwtProviderMock = null!;
        private AuthService _sut = null!;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>(MockBehavior.Strict);
            _passwordHasherMock = new Mock<IPasswordHasher>(MockBehavior.Strict);
            _jwtProviderMock = new Mock<IJwtProvider>(MockBehavior.Strict);
            _sut = new AuthService(_userRepositoryMock.Object, _passwordHasherMock.Object, _jwtProviderMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _userRepositoryMock.VerifyAll();
            _passwordHasherMock.VerifyAll();
            _jwtProviderMock.VerifyAll();
        }

        [Test]
        public async Task RegisterAsync_WhenEmailNotExists_CreatesUserAndReturnsToken()
        {
            // Arrange
            var request = new RegisterRequestDto { Email = "test@example.com", Password = "Passw0rd!" };
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);
            _passwordHasherMock.Setup(h => h.Hash(request.Password)).Returns("HASH");
            _userRepositoryMock.Setup(r => r.CreateAsync(It.Is<User>(u => u.Email == request.Email && u.PasswordHash == "HASH"), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User u, CancellationToken _) => u);
            _jwtProviderMock.Setup(j => j.Generate(It.IsAny<Guid>(), request.Email, "user"))
                .Returns(new JwtTokenResult("token", DateTime.UtcNow.AddHours(1)));

            // Act
            var result = await _sut.RegisterAsync(request, CancellationToken.None);

            // Assert
            Assert.That(result.AccessToken, Is.EqualTo("token"));
            Assert.That(result.Email, Is.EqualTo(request.Email));
            Assert.That(result.Role, Is.EqualTo("user"));
        }

        [Test]
        public void RegisterAsync_WhenEmailAlreadyExists_ThrowsBusinessException()
        {
            // Arrange
            var request = new RegisterRequestDto { Email = "exists@example.com", Password = "x" };
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new User(request.Email, "hash", "user"));

            // Act + Assert
            Assert.ThrowsAsync<BusinessException>(() => _sut.RegisterAsync(request, CancellationToken.None));
        }

        [Test]
        public async Task LoginAsync_WithValidCredentials_ReturnsToken()
        {
            // Arrange
            var request = new LoginRequestDto { Email = "login@example.com", Password = "secret" };
            var user = new User(request.Email, "HASH", "user");
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _passwordHasherMock.Setup(h => h.Verify(request.Password, user.PasswordHash)).Returns(true);
            _jwtProviderMock.Setup(j => j.Generate(user.Id, user.Email, user.Role))
                .Returns(new JwtTokenResult("jwt", DateTime.UtcNow.AddHours(1)));

            // Act
            var result = await _sut.LoginAsync(request, CancellationToken.None);

            // Assert
            Assert.That(result.AccessToken, Is.EqualTo("jwt"));
            Assert.That(result.Email, Is.EqualTo(request.Email));
        }

        [Test]
        public void LoginAsync_WithUnknownEmail_ThrowsBusinessException()
        {
            // Arrange
            var request = new LoginRequestDto { Email = "unknown@example.com", Password = "x" };
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act + Assert
            Assert.ThrowsAsync<BusinessException>(() => _sut.LoginAsync(request, CancellationToken.None));
        }

        [Test]
        public void LoginAsync_WithInvalidPassword_ThrowsBusinessException()
        {
            // Arrange
            var request = new LoginRequestDto { Email = "login@example.com", Password = "bad" };
            var user = new User(request.Email, "HASH", "user");
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _passwordHasherMock.Setup(h => h.Verify(request.Password, user.PasswordHash)).Returns(false);

            // Act + Assert
            Assert.ThrowsAsync<BusinessException>(() => _sut.LoginAsync(request, CancellationToken.None));
        }
    }
}


