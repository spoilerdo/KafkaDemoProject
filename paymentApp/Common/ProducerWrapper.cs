using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KafkaProducer.Common
{
    public static class ProducerWrapper
    {
        public static async void ProduceAsync(ProducerConfig producerConfig, string topic, object data)
        {
            using(var producer = new ProducerBuilder<Null, string>(producerConfig).Build())
            {
                await producer.ProduceAsync(topic, new Message<Null, string> { Value = JsonConvert.SerializeObject(data) });
            }
        }
    }
}
