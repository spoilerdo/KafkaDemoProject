using AutoMapper;
using Confluent.Kafka;
using Grpc.Core;
using KafkaProducer.Common;
using KafkaProducer.Persistence.Entities;
using KafkaProducer.Persistence.Repositories.Payments;
using KafkaProducer.Protos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KafkaProducer.Services
{
    public class PaymentService : Payment_Service.Payment_ServiceBase
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMapper _mapper;
        private readonly ProducerConfig _producerConfig;
        private readonly ConsumerConfig _consumerConfig;

        public PaymentService(IPaymentRepository paymentRepository, IMapper mapper, ProducerConfig producerConfig, ConsumerConfig consumerConfig)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
            _producerConfig = producerConfig;
            _consumerConfig = consumerConfig;
        }

        public override async Task<Payment> Pay(PaymentRequest request, ServerCallContext context)
        {           
            var createdPayment = await _paymentRepository.CreatePayment(_mapper.Map<PaymentEntity>(request));
            if (!createdPayment.Success)
                throw new RpcException(new Status(StatusCode.InvalidArgument, createdPayment.Message));

            ProducerWrapper.ProduceAsync(_producerConfig, "handle-payment", createdPayment.Data);
            return _mapper.Map<Payment>(createdPayment.Data);

            throw new RpcException(new Status(StatusCode.OK, "In progress"));
        }

        public override async Task<Payments> Receive(UserIdRequest request, ServerCallContext context)
        {
            var receivedPayments = await _paymentRepository.GetPaymentsFromUser(new Guid(request.UserId));
            if (!receivedPayments.Success)
                throw new RpcException(new Status(StatusCode.NotFound, receivedPayments.Message));
            return new Payments
            {
                Payments_ = { _mapper.Map<IEnumerable<Payment>>(receivedPayments.Data) }
            };
        }
    }
}
