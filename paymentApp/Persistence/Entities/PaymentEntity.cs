using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KafkaProducer.Persistence.Entities
{
    public class PaymentEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public int Amount { get; set; }
        public string Label { get; set; }
        public Guid ReceiverId { get; set; }
        public Guid SenderId { get; set; }
        [JsonIgnore]
        public virtual UserEntity Sender { get; set; }
        public bool Payed { get; set; }
    }
}
