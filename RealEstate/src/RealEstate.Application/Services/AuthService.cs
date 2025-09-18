using System.Threading;
using System.Threading.Tasks;
using RealEstate.Application.DTOs.Auth;
using RealEstate.Application.Interfaces;
using RealEstate.Application.Interfaces.Repositories;
using RealEstate.Application.Interfaces.Security;
using RealEstate.Domain.Entities;
using RealEstate.Shared.Exceptions;

namespace RealEstate.Application.Services
{
    public sealed class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;

        public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtProvider jwtProvider)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
        {
            var existing = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (existing != null)
            {
                throw new BusinessException("El correo ya está registrado");
            }

            string hash = _passwordHasher.Hash(request.Password);
            var user = new User(request.Email, hash, request.Role);
            await _userRepository.CreateAsync(user, cancellationToken);

            var token = _jwtProvider.Generate(user.Id, user.Email, user.Role);
            return new AuthResponseDto
            {
                AccessToken = token.AccessToken,
                ExpiresAtUtc = token.ExpiresAtUtc,
                Email = user.Email,
                Role = user.Role
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (user == null)
            {
                throw new BusinessException("Credenciales inválidas");
            }

            bool ok = _passwordHasher.Verify(request.Password, user.PasswordHash);
            if (!ok)
            {
                throw new BusinessException("Credenciales inválidas");
            }

            var token = _jwtProvider.Generate(user.Id, user.Email, user.Role);
            return new AuthResponseDto
            {
                AccessToken = token.AccessToken,
                ExpiresAtUtc = token.ExpiresAtUtc,
                Email = user.Email,
                Role = user.Role
            };
        }
    }
}


