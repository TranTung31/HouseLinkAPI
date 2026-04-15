using HouseLink.Identity.Domain.Entities;
using HouseLink.Identity.Domain.Enums;
using HouseLink.Identity.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HouseLink.Identity.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivityLogController : ControllerBase
    {
        private readonly IKafkaProducerService _kafkaProducer;
        private readonly IActivityLogRepository _activityLogRepository;

        public ActivityLogController(IKafkaProducerService kafkaProducer, IActivityLogRepository activityLogRepository)
        {
            _kafkaProducer = kafkaProducer;
            _activityLogRepository = activityLogRepository;
        }

        [HttpPost("log-login")]
        public async Task<IActionResult> LogUserLogin([FromBody] LoginLogRequest request)
        {
            var activityLog = ActivityLog.Create(
                userId: request.UserId,
                activityType: ActivityType.UserLoggedIn,
                description: $"User logged in from {request.IpAddress ?? "unknown location"}",
                ipAddress: request.IpAddress,
                userAgent: Request.Headers.UserAgent.ToString(),
                additionalData: new Dictionary<string, object>
                {
                    { "loginMethod", request.LoginMethod ?? "standard" },
                    { "timestamp", DateTime.UtcNow }
                }
            );

            // Gửi đến Kafka
            await _kafkaProducer.ProduceActivityLogAsync(activityLog);

            // Lưu vào database
            //await _activityLogRepository.AddAsync(activityLog);
            //await _activityLogRepository.SaveChangesAsync();

            return Ok(new { message = "Login activity logged successfully", id = activityLog.Id });
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserActivities(Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var skip = (page - 1) * pageSize;
            var activities = await _activityLogRepository.GetUserActivitiesAsync(userId, skip, pageSize);

            return Ok(activities);
        }
    }

    public class LoginLogRequest
    {
        public Guid UserId { get; set; }
        public string? IpAddress { get; set; }
        public string? LoginMethod { get; set; }
    }
}