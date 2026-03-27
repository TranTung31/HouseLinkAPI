namespace HouseLink.Identity.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Token { get; private set; } = string.Empty;
        public DateTime ExpiresAt { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public bool IsRevoked { get; private set; }
        public string? ReplacedBy { get; private set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsActive => !IsRevoked && !IsExpired;

        private RefreshToken() { }

        public static RefreshToken Create(Guid userId, string token, int expiryDays = 7)
            => new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddDays(expiryDays),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };

        public void Revoke(string? replacedByToken = null)
        {
            IsRevoked = true;
            ReplacedBy = replacedByToken;
        }
    }
}
