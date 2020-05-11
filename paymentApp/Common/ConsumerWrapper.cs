using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaProducer.Common
{
    public static class ConsumerWrapper
    {
        public static async Task Consume(ConsumerConfig consumerConfig, CancellationToken cancellationToken, string topic, Action<ConsumeResult<Ignore, string>> action)
        {
            using(var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
            {
                consumer.Subscribe(topic);
                try
                {
                    await Task.Run(() =>
                    {
                        while (!cancellationToken.IsCancellationRequested)
                        {
                            var cr = consumer.Consume(cancellationToken);

                            action(cr);
                        }
                    }, cancellationToken);
                }
                catch(ConsumeException e)
                {
                    Console.WriteLine($"Error occured: {e.Error.Reason}");
                }
                finally
                {
                    consumer.Close();
                }
            }
        }
    }
}
