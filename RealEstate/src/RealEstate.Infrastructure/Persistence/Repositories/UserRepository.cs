using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using RealEstate.Application.Interfaces.Repositories;
using RealEstate.Domain.Entities;
using RealEstate.Infrastructure.Persistence.Context;

namespace RealEstate.Infrastructure.Persistence.Repositories
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _collection;

        public UserRepository(IMongoDbContext context)
        {
            _collection = context.Users;
        }

        public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
        {
            await _collection.InsertOneAsync(user, cancellationToken: cancellationToken);
            return user;
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var cursor = await _collection.FindAsync(x => x.Email == email, cancellationToken: cancellationToken);
            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }
    }
}


