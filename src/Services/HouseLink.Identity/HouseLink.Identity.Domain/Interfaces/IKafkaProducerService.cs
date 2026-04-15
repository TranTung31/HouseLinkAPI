using HouseLink.Identity.Domain.Entities;

namespace HouseLink.Identity.Domain.Interfaces
{
    public interface IKafkaProducerService
    {
        Task ProduceActivityLogAsync(ActivityLog activityLog, CancellationToken cancellationToken = default);
        Task ProduceMessageAsync<T>(string topic, T message, CancellationToken cancellationToken = default) where T : class;
    }
}
