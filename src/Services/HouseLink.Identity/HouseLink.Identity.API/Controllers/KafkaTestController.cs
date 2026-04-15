using HouseLink.Identity.Domain.Entities;
using HouseLink.Identity.Domain.Enums;
using HouseLink.Identity.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HouseLink.Identity.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KafkaTestController : ControllerBase
    {
        private readonly IKafkaProducerService _kafkaProducer;
        private readonly IActivityLogRepository _activityLogRepository;

        public KafkaTestController(IKafkaProducerService kafkaProducer, IActivityLogRepository activityLogRepository)
        {
            _kafkaProducer = kafkaProducer;
            _activityLogRepository = activityLogRepository;
        }

        [HttpPost("send-activity-log")]
        public async Task<IActionResult> SendActivityLog([FromBody] ActivityLogRequest request)
        {
            try
            {
                var activityLog = ActivityLog.Create(
                    userId: request.UserId,
                    activityType: request.ActivityType,
                    description: request.Description,
                    ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString(),
                    userAgent: Request.Headers.UserAgent.ToString(),
                    additionalData: request.AdditionalData
                );

                // Gửi đến Kafka
                await _kafkaProducer.ProduceActivityLogAsync(activityLog);

                // Đồng thời lưu vào database
                await _activityLogRepository.AddAsync(activityLog);
                await _activityLogRepository.SaveChangesAsync();

                return Ok(new { message = "Activity log sent to Kafka successfully", id = activityLog.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("send-generic-message")]
        public async Task<IActionResult> SendGenericMessage([FromBody] GenericMessageRequest request)
        {
            try
            {
                await _kafkaProducer.ProduceMessageAsync(request.Topic, request.Message);

                return Ok(new { message = "Message sent to Kafka successfully", topic = request.Topic });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }

    public class ActivityLogRequest
    {
        public Guid UserId { get; set; }
        public ActivityType ActivityType { get; set; }
        public string Description { get; set; } = string.Empty;
        public Dictionary<string, object>? AdditionalData { get; set; }
    }

    public class GenericMessageRequest
    {
        public string Topic { get; set; } = string.Empty;
        public object Message { get; set; } = new { };
    }
}