using Confluent.Kafka;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RankVotingApi.Votes;
using System;
using System.Collections;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace RankVotingApi.KafkaConsumer
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly KafkaOptions _options;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        private readonly ILogger<KafkaConsumerService> _logger;

        public KafkaConsumerService(IOptions<KafkaOptions> options, IConfiguration configuration,
            IServiceScopeFactory serviceScopeFactory, ILogger<KafkaConsumerService> logger)
        {
            _configuration = configuration;
            _options = options.Value;

            _serviceScopeFactory = serviceScopeFactory;

            _logger = logger;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                var config = Common.Common.GetKafkaConfiguration((IConfigurationRoot)_configuration);

                using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
                consumer.Subscribe(_options.Topic);

                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        var result = consumer.Consume(stoppingToken);


                        _logger.LogInformation("Consumed message '{MessageValue}' at: '{TopicPartitionOffset}'", 
                            result.Message.Value, result.TopicPartitionOffset);

                        var ranking = JsonSerializer.Deserialize<Ranking>(result.Message.Value);

                        using IServiceScope scope = _serviceScopeFactory.CreateScope();

                        IVoteBusiness voteBusiness =
                            scope.ServiceProvider.GetRequiredService<IVoteBusiness>();

                        await voteBusiness.SubmitNewRanking(ranking.Name, ranking.Id,
                            ranking.Candidates.Select(x => x.Name));

                    }
                }
                catch (OperationCanceledException)
                {
                    consumer.Close();
                }
            }, stoppingToken);
        }
    }
}
