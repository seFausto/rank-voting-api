using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
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
        private IConfiguration _configuration;
        private KafkaOptions _options;
        private ILogger<KafkaConsumerService> _logger;

        public KafkaConsumerService(IOptions<KafkaOptions> options, IConfiguration configuration, ILogger<KafkaConsumerService> logger)
        {
            _configuration = configuration;
            _options = options.Value;
            _logger = logger;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                var config = Common.Common.GetKafkaConfiguration((IConfigurationRoot)_configuration);

                using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
                consumer.Subscribe(_options.Topic);

                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        var result = consumer.Consume(stoppingToken);
                        _logger.LogInformation($"Consumed message '{result.Message.Value}' at: '{result.TopicPartitionOffset}'.");
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
