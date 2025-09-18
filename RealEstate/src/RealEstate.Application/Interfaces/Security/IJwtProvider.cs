using System;

namespace RealEstate.Application.Interfaces.Security
{
    public sealed class JwtTokenResult
    {
        public string AccessToken { get; }
        public DateTime ExpiresAtUtc { get; }

        public JwtTokenResult(string accessToken, DateTime expiresAtUtc)
        {
            AccessToken = accessToken;
            ExpiresAtUtc = expiresAtUtc;
        }
    }

    public interface IJwtProvider
    {
        JwtTokenResult Generate(Guid userId, string email, string role);
    }
}


