using HouseLink.Identity.Domain.Enums;
using Newtonsoft.Json;

namespace HouseLink.Identity.Domain.Entities
{
    public class ActivityLog
    {
        [JsonProperty]
        public Guid Id { get; private set; }

        [JsonProperty]
        public Guid UserId { get; private set; }

        [JsonProperty]
        public ActivityType ActivityType { get; private set; }

        [JsonProperty]
        public string Description { get; private set; } = string.Empty;

        [JsonProperty]
        public string? IpAddress { get; private set; }

        [JsonProperty]
        public string? UserAgent { get; private set; }

        [JsonProperty]
        public string? AdditionalData { get; private set; }

        [JsonProperty]
        public DateTime CreatedAt { get; private set; }

        private ActivityLog() { }

        public static ActivityLog Create(
            Guid userId,
            ActivityType activityType,
            string description,
            string? ipAddress = null,
            string? userAgent = null,
            Dictionary<string, object>? additionalData = null)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new Domain.Exceptions.DomainException("Description không được trống.");

            var log = new ActivityLog
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ActivityType = activityType,
                Description = description.Trim(),
                IpAddress = ipAddress,
                UserAgent = userAgent,
                CreatedAt = DateTime.UtcNow
            };

            if (additionalData != null && additionalData.Count > 0)
            {
                log.AdditionalData = JsonConvert.SerializeObject(additionalData);
            }

            return log;
        }

        public static ActivityLog CreateForSystem(
            ActivityType activityType,
            string description,
            string? ipAddress = null,
            string? userAgent = null,
            Dictionary<string, object>? additionalData = null)
        {
            return Create(Guid.Empty, activityType, description, ipAddress, userAgent, additionalData);
        }

        public Dictionary<string, object>? GetAdditionalData()
        {
            if (string.IsNullOrWhiteSpace(AdditionalData))
                return null;

            return JsonConvert.DeserializeObject<Dictionary<string, object>>(AdditionalData);
        }

        public void UpdateAdditionalData(Dictionary<string, object> additionalData)
        {
            AdditionalData = JsonConvert.SerializeObject(additionalData);
        }
    }
}
