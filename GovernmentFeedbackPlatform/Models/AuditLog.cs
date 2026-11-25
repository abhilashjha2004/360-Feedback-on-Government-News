using System.ComponentModel.DataAnnotations;

namespace GovernmentFeedbackPlatform.Models
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(450)]
        public string? UserId { get; set; }

        [MaxLength(200)]
        public string? Action { get; set; }

        [MaxLength(100)]
        public string? EntityType { get; set; }

        public int? EntityId { get; set; }

        [MaxLength(2000)]
        public string? Details { get; set; }

        [MaxLength(50)]
        public string? IpAddress { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
