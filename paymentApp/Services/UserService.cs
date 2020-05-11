using AutoMapper;
using Grpc.Core;
using KafkaProducer.Persistence.Entities;
using KafkaProducer.Persistence.Repositories.Users;
using KafkaProducer.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KafkaProducer.Services
{
    public class UserService : User_Service.User_ServiceBase
    {
        private readonly IUserRepository _userRepositry;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepositry = userRepository;
            _mapper = mapper;
        }

        public override async Task<User> RegisterUser(RegisterReqeust request, ServerCallContext context)
        {
            var createdUser = await _userRepositry.CreateUser(_mapper.Map<UserEntity>(request));
            if (!createdUser.Success)
                throw new RpcException(new Status(StatusCode.InvalidArgument, createdUser.Message));

            return _mapper.Map<User>(createdUser.Data);
        }

        public override async Task<User> GetUser(IdRequest request, ServerCallContext context)
        {
            var foundUser = await _userRepositry.GetUserById(new Guid(request.Id));
            if (!foundUser.Success)
                throw new RpcException(new Status(StatusCode.NotFound, foundUser.Message));

            return _mapper.Map<User>(foundUser.Data);
        }
    }
}
