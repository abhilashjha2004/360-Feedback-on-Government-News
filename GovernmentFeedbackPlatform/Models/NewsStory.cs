using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GovernmentFeedbackPlatform.Models
{
    public class NewsStory
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(500)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Summary { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [MaxLength(500)]
        public string? SourceUrl { get; set; }

        [MaxLength(200)]
        public string? MediaSource { get; set; }

        [MaxLength(100)]
        public string? State { get; set; }

        [MaxLength(50)]
        public string? Language { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public bool IsPublished { get; set; } = false;
        public bool IsArchived { get; set; } = false;

        [MaxLength(500)]
        public string? Tags { get; set; }

        public DateTime PublishedDate { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public DateTime PublishedAt { get => PublishedDate; set => PublishedDate = value; }

        [MaxLength(450)]
        public string? CreatedByUserId { get; set; }

        // Navigation
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        [ForeignKey("CreatedByUserId")]
        public ApplicationUser? CreatedBy { get; set; }

        public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    }
}
