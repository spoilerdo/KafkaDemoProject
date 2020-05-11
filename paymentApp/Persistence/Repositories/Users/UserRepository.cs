using KafkaProducer.Common;
using KafkaProducer.Persistence.Context;
using KafkaProducer.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KafkaProducer.Persistence.Repositories.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly ProducerDbContext _context;

        public UserRepository(ProducerDbContext context)
        {
            _context = context;
        }

        public async Task<DataResponseObject<UserEntity>> CreateUser(UserEntity user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new DataResponseObject<UserEntity>(user);
        }

        public async Task<DataResponseObject<UserEntity>> GetUserById(Guid id)
        {
            var foundUser = await _context.Users.FindAsync(id);
            if (foundUser == null)
                return new DataResponseObject<UserEntity>($"User with id: {id} could not be found");

            return new DataResponseObject<UserEntity>(foundUser);
        }
    }
}
