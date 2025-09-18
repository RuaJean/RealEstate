using System.Threading;
using System.Threading.Tasks;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User> CreateAsync(User user, CancellationToken cancellationToken = default);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    }
}


