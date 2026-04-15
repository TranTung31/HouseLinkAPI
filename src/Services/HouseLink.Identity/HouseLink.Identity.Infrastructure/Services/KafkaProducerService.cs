using Confluent.Kafka;
using HouseLink.Identity.Domain.Entities;
using HouseLink.Identity.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text.Json;

namespace HouseLink.Identity.Infrastructure.Services
{
    public class KafkaProducerService : IKafkaProducerService, IDisposable
    {
        private readonly IProducer<Null, string> _producer;
        private readonly string _activityLogTopic;

        public KafkaProducerService(IConfiguration configuration)
        {
            var bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
            _activityLogTopic = configuration["Kafka:ActivityLogTopic"] ?? "activity-logs";

            var config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers,
                Acks = Acks.Leader,
                EnableIdempotence = false,
                MessageSendMaxRetries = 3,
                RetryBackoffMs = 1000,
                RequestTimeoutMs = 30000,
                ClientId = "houselink-identity-producer",
                // Quan trọng: cho phép client resolve hostname
                SocketKeepaliveEnable = true
            };

            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task ProduceActivityLogAsync(ActivityLog activityLog, CancellationToken cancellationToken = default)
        {
            var json = JsonConvert.SerializeObject(activityLog, Formatting.None);

            try
            {
                // Set timeout cho mỗi lần produce
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(TimeSpan.FromSeconds(10)); // 10 giây timeout

                var deliveryResult = await _producer.ProduceAsync(_activityLogTopic,
                    new Message<Null, string> { Value = json }, cts.Token);

                Console.WriteLine($"Delivered '{deliveryResult.Value}' to partition {deliveryResult.Partition}");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Kafka produce operation timed out");
                throw new TimeoutException("Kafka produce operation timed out after 10 seconds");
            }
            catch (ProduceException<Null, string> ex)
            {
                Console.WriteLine($"Failed to deliver message: {ex.Error.Reason}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error in Kafka producer: {ex.Message}");
                throw;
            }
        }

        public async Task ProduceMessageAsync<T>(string topic, T message, CancellationToken cancellationToken = default) where T : class
        {
            string json;

            if (message is JsonElement jsonElement)
                json = jsonElement.GetRawText(); // Xử lý riêng JsonElement
            else
                json = JsonConvert.SerializeObject(message, Formatting.None);

            var deliveryResult = await _producer.ProduceAsync(topic,
                new Message<Null, string> { Value = json }, cancellationToken);

            Console.WriteLine($"Delivered message to partition {deliveryResult.Partition}");
        }

        public void Dispose()
        {
            _producer?.Dispose();
        }
    }
}
