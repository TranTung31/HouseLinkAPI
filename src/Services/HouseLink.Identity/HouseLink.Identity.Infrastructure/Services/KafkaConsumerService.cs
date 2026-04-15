using Confluent.Kafka;
using HouseLink.Identity.Domain.Entities;
using HouseLink.Identity.Domain.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace HouseLink.Identity.Infrastructure.Services
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly ILogger<KafkaConsumerService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private IConsumer<Ignore, string>? _consumer;

        public KafkaConsumerService(
            ILogger<KafkaConsumerService> logger,
            IConfiguration configuration,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Thêm delay khởi động để đảm bảo các dịch vụ khác đã sẵn sàng
            await Task.Delay(5000, stoppingToken);

            var bootstrapServers = _configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
            var topic = _configuration["Kafka:ActivityLogTopic"] ?? "activity-logs";

            var config = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = "activity-log-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                EnablePartitionEof = true,
                // Thêm các cấu hình để tránh block trong startup
                SocketTimeoutMs = 10000,
                SessionTimeoutMs = 30000,
                MaxPollIntervalMs = 300000
            };

            try
            {
                _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
                _consumer.Subscribe(topic);

                _logger.LogInformation("Kafka consumer started and subscribed to topic: {Topic}", topic);

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = _consumer.Consume(stoppingToken);

                        if (consumeResult?.Message?.Value != null)
                        {
                            await ProcessMessage(consumeResult.Message.Value, stoppingToken);
                            _consumer.Commit(consumeResult);

                            _logger.LogInformation($"Processed message from partition {consumeResult.Partition} at offset {consumeResult.Offset}");
                        }
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError(ex, $"Consume error: {ex.Error.Reason}");
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogInformation("Operation was cancelled");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Kafka consumer");
                throw;
            }
            finally
            {
                try
                {
                    _consumer?.Close();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error closing consumer");
                }
                _consumer?.Dispose();
            }
        }

        private async Task ProcessMessage(string messageValue, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var activityLogRepository = scope.ServiceProvider.GetRequiredService<IActivityLogRepository>();

            try
            {
                var activityLog = JsonConvert.DeserializeObject<ActivityLog>(messageValue);

                if (activityLog != null)
                {
                    // Đảm bảo description không trống
                    var description = !string.IsNullOrWhiteSpace(activityLog.Description)
                        ? activityLog.Description
                        : "Activity log from Kafka message";

                    // Tạo một ActivityLog mới với Id mới để tránh lỗi Guid.Empty
                    var newActivityLog = ActivityLog.Create(
                        userId: activityLog.UserId != Guid.Empty ? activityLog.UserId : Guid.NewGuid(),
                        activityType: activityLog.ActivityType,
                        description: description,
                        ipAddress: activityLog.IpAddress,
                        userAgent: activityLog.UserAgent,
                        additionalData: activityLog.GetAdditionalData()
                    );

                    await activityLogRepository.AddAsync(newActivityLog, cancellationToken);
                    await activityLogRepository.SaveChangesAsync(cancellationToken);

                    _logger.LogInformation($"Saved activity log {newActivityLog.Id} to database");
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, $"Failed to deserialize message: {messageValue}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to process message: {messageValue}");
                throw; // Re-throw to prevent committing offset
            }
        }

        public override void Dispose()
        {
            _consumer?.Dispose();
            base.Dispose();
        }
    }
}