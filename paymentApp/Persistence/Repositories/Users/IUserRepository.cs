using KafkaProducer.Common;
using KafkaProducer.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KafkaProducer.Persistence.Repositories.Users
{
    public interface IUserRepository
    {
        Task<DataResponseObject<UserEntity>> CreateUser(UserEntity user);
        Task<DataResponseObject<UserEntity>> GetUserById(Guid id);
    }
}
