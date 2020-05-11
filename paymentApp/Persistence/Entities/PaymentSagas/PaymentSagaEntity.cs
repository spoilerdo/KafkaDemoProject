using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KafkaProducer.Persistence.Entities.PaymentSagas
{
    public class PaymentSagaEntity
    {
        public Guid paymentId { get; set; }
        public string message { get; set; }

        public PaymentSagaEntity()
        {

        }

        public PaymentSagaEntity(Guid paymentId, string message)
        {
            this.paymentId = paymentId;
            this.message = message;
        }
    }
}
