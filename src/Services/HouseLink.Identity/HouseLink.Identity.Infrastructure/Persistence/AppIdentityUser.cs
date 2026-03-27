using HouseLink.Identity.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace HouseLink.Identity.Infrastructure.Persistence
{
    public class AppIdentityUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Buyer;
        public string? AvatarUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
