using Confluent.Kafka;
using KafkaProducer.Common;
using KafkaProducer.Persistence.Entities;
using KafkaProducer.Persistence.Entities.PaymentSagas;
using KafkaProducer.Persistence.Repositories.Payments;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaProducer.Services
{
    public class ProcessPaymentService
    {
        public class ProcessPayment : BackgroundService, IHostedService
        {
            private readonly IServiceScopeFactory _serviceScopeFactory;
            private readonly ConsumerConfig _consumerConfig;
            private readonly ProducerConfig _producerConfig;
            public ProcessPayment(IServiceScopeFactory serviceScopeFactory, ConsumerConfig consumerConfig, ProducerConfig producerConfig)
            {
                _serviceScopeFactory = serviceScopeFactory;
                _consumerConfig = consumerConfig;
                _producerConfig = producerConfig;
            }
            protected override async Task ExecuteAsync(CancellationToken stoppingToken)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var _paymentRepository = scope.ServiceProvider.GetRequiredService<IPaymentRepository>();

                    await ConsumerWrapper.Consume(_consumerConfig, stoppingToken, "handle-payment", async (cr) =>
                    {
                        var entity = JsonConvert.DeserializeObject<PaymentEntity>(cr.Message.Value);
                        var payResult = await _paymentRepository.Pay(entity.Id);
                        if (!payResult.Success)
                        {
                            var data = new PaymentSagaEntity(entity.Id, payResult.Message);
                            ProducerWrapper.ProduceAsync(_producerConfig, "handle-payment-error", data);
                        }
                    });
                }
            }
        }

        public class ProcessPaymentFailed : BackgroundService, IHostedService
        {
            private IServiceScopeFactory _serviceScopeFactory;
            private readonly ConsumerConfig _consumerConfig;
            public ProcessPaymentFailed(IServiceScopeFactory serviceScopeFactory, ConsumerConfig consumerConfig)
            {
                _serviceScopeFactory = serviceScopeFactory;
                _consumerConfig = consumerConfig;
            }
            protected override async Task ExecuteAsync(CancellationToken stoppingToken)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var _paymentRepository = scope.ServiceProvider.GetRequiredService<IPaymentRepository>();

                    await ConsumerWrapper.Consume(_consumerConfig, stoppingToken, "handle-payment-error", async (cr) =>
                    {
                        var entity = JsonConvert.DeserializeObject<PaymentSagaEntity>(cr.Message.Value);
                        await _paymentRepository.DeletePayment(entity.paymentId);
                    });
                }
            }
        }
    }
}
