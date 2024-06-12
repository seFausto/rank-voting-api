using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RankVotingApi.KafkaConsumer
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly KafkaOptions _options;
        private readonly ILogger<KafkaConsumerService> _logger;

        public KafkaConsumerService(IOptions<KafkaOptions> options, ILogger<KafkaConsumerService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                var config = new ConsumerConfig
                {
                    GroupId = _options.GroupId,
                    BootstrapServers = _options.BootstrapServers,
                    AutoOffsetReset = AutoOffsetReset.Earliest
                };

                using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
                consumer.Subscribe(_options.Topic);

                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        var result = consumer.Consume(stoppingToken);
                        _logger.LogInformation("Consumed message '{MesssageValue}' at: '{TopicPartitionOffset}'.", 
                            result.Message.Value, result.TopicPartitionOffset);
                    }
                }
                catch (OperationCanceledException)
                {
                    consumer.Close();
                }
            });
        }
    }
}
