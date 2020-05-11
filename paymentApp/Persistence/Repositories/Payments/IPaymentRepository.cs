using KafkaProducer.Common;
using KafkaProducer.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KafkaProducer.Persistence.Repositories.Payments
{
    public interface IPaymentRepository
    {
        Task<DataResponseObject<PaymentEntity>> CreatePayment(PaymentEntity payment);
        Task<DataResponseObject<PaymentEntity>> DeletePayment(Guid id);
        Task<DataResponseObject<PaymentEntity>> Pay(Guid id);
        Task<DataResponseObject<IEnumerable<PaymentEntity>>> GetPaymentsFromUser(Guid id);
    }
}
