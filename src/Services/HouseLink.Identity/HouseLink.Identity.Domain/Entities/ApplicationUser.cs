using HouseLink.Identity.Domain.Enums;

namespace HouseLink.Identity.Domain.Entities
{
    public class ApplicationUser
    {
        public Guid Id { get; private set; }
        public string FullName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string PhoneNumber { get; private set; } = string.Empty;
        public UserRole Role { get; private set; }
        public string? AvatarUrl { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private ApplicationUser() { }

        public static ApplicationUser Create(
            string fullName,
            string email,
            string phoneNumber,
            UserRole role = UserRole.Buyer)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new Domain.Exceptions.DomainException("Họ tên không được trống.");
            if (string.IsNullOrWhiteSpace(email))
                throw new Domain.Exceptions.DomainException("Email không được trống.");

            return new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FullName = fullName.Trim(),
                Email = email.Trim().ToLowerInvariant(),
                PhoneNumber = phoneNumber.Trim(),
                Role = role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void UpdateProfile(string fullName, string phoneNumber, string? avatarUrl)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new Domain.Exceptions.DomainException("Họ tên không được trống.");

            FullName = fullName.Trim();
            PhoneNumber = phoneNumber.Trim();
            AvatarUrl = avatarUrl;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
    }
}
