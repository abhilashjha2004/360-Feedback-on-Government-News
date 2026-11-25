using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GovernmentFeedbackPlatform.Models
{
    public enum SentimentType
    {
        Positive = 1,
        Neutral = 2,
        Negative = 3
    }

    public class Feedback
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int NewsStoryId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        public SentimentType UserSentiment { get; set; }

        public SentimentType? DetectedSentiment { get; set; }

        [MaxLength(2000)]
        public string? Comment { get; set; }

        [MaxLength(2000)]
        public string? Suggestions { get; set; }

        [MaxLength(100)]
        public string? FeedbackCategory { get; set; }

        public bool IsModerated { get; set; } = false;
        public bool IsApproved { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        [MaxLength(500)]
        public string? ModerationNote { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public SentimentType Sentiment => DetectedSentiment ?? UserSentiment;

        [NotMapped]
        public DateTime SubmittedAt => CreatedAt;

        // Navigation
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        [ForeignKey("NewsStoryId")]
        public NewsStory? NewsStory { get; set; }
    }
}
