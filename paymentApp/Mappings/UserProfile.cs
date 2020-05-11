using AutoMapper;
using KafkaProducer.Persistence.Entities;
using KafkaProducer.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KafkaProducer.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<RegisterReqeust, UserEntity>();
            CreateMap<UserEntity, User>();
        }
    }
}
