using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GovernmentFeedbackPlatform.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? State { get; set; }

        [MaxLength(50)]
        public string? Language { get; set; }

        [MaxLength(20)]
        public string? AgeGroup { get; set; }

        [MaxLength(50)]
        public string? Gender { get; set; }

        [MaxLength(50)]
        public string? Occupation { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    }
}
