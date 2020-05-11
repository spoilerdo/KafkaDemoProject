using AutoMapper;
using KafkaProducer.Persistence.Entities;
using KafkaProducer.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KafkaProducer.Mappings
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            CreateMap<PaymentEntity, PaymentRequest>();
            CreateMap<PaymentRequest, PaymentEntity>();
            CreateMap<PaymentEntity, UserIdRequest>();
            CreateMap<Payment, PaymentEntity>();
            CreateMap<PaymentEntity, Payment>();
        }
    }
}
