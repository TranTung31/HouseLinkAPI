using HouseLink.Identity.Domain.Entities;

namespace HouseLink.Identity.Domain.Interfaces
{
    public interface IActivityLogRepository
    {
        Task AddAsync(ActivityLog activityLog, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<ActivityLog>> GetUserActivitiesAsync(Guid userId, int skip = 0, int take = 50, CancellationToken cancellationToken = default);
        Task<IEnumerable<ActivityLog>> GetRecentActivitiesAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default);
    }
}
