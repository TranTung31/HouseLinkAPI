using HouseLink.Identity.Domain.Entities;
using HouseLink.Identity.Domain.Interfaces;
using HouseLink.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HouseLink.Identity.Infrastructure.Repositories
{
    public class ActivityLogRepository : IActivityLogRepository
    {
        private readonly AppDbContext _context;

        public ActivityLogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ActivityLog activityLog, CancellationToken cancellationToken = default)
        {
            await _context.ActivityLogs.AddAsync(activityLog, cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<ActivityLog>> GetUserActivitiesAsync(Guid userId, int skip = 0, int take = 50, CancellationToken cancellationToken = default)
        {
            return await _context.ActivityLogs
                .Where(log => log.UserId == userId)
                .OrderByDescending(log => log.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ActivityLog>> GetRecentActivitiesAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default)
        {
            return await _context.ActivityLogs
                .OrderByDescending(log => log.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync(cancellationToken);
        }
    }
}
