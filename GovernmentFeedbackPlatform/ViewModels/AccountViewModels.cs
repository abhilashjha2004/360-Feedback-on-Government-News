using System.ComponentModel.DataAnnotations;

namespace GovernmentFeedbackPlatform.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Full name is required")]
        [Display(Name = "Full Name")]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Display(Name = "State / UT")]
        public string? State { get; set; }

        [Display(Name = "Preferred Language")]
        public string? Language { get; set; }

        [Display(Name = "Age Group")]
        public string? AgeGroup { get; set; }

        public string? Gender { get; set; }
        public string? Occupation { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }

    public class ProfileViewModel
    {
        public string? Id { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        public string? Email { get; set; }
        public string? State { get; set; }
        public string? Language { get; set; }

        [Display(Name = "Age Group")]
        public string? AgeGroup { get; set; }

        public string? Gender { get; set; }
        public string? Occupation { get; set; }

        public int TotalFeedbacks { get; set; }
        public DateTime MemberSince { get; set; }
    }
}
