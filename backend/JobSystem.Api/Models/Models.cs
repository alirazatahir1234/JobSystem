using Microsoft.AspNetCore.Identity;

namespace JobSystem.Api.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
        public virtual UserProfile? Profile { get; set; }
    }

    public class Job
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Requirements { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Emirate { get; set; } = string.Empty; // Dubai, Abu Dhabi, Sharjah, etc.
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public string Currency { get; set; } = "AED";
        public string ExperienceLevel { get; set; } = string.Empty; // Junior, Mid, Senior
        public string JobType { get; set; } = string.Empty; // Full-time, Part-time, Contract
        public string Source { get; set; } = string.Empty; // Bayt, Dubizzle, etc.
        public string ExternalUrl { get; set; } = string.Empty;
        public string ExternalId { get; set; } = string.Empty;
        public List<string> Technologies { get; set; } = new List<string>();
        public List<string> Benefits { get; set; } = new List<string>();
        public bool IsActive { get; set; } = true;
        public DateTime PostedDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
    }

    public class JobApplication
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int JobId { get; set; }
        public string Status { get; set; } = "Applied"; // Applied, Reviewed, Interview, Rejected, Hired
        public string CoverLetter { get; set; } = string.Empty;
        public string ResumeFileName { get; set; } = string.Empty;
        public DateTime AppliedDate { get; set; } = DateTime.UtcNow;
        public DateTime? InterviewDate { get; set; }
        public string Notes { get; set; } = string.Empty;
        
        // Navigation properties
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual Job Job { get; set; } = null!;
    }

    public class UserProfile
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public List<string> Skills { get; set; } = new List<string>();
        public List<Experience> Experience { get; set; } = new List<Experience>();
        public List<Education> Education { get; set; } = new List<Education>();
        public List<string> Certifications { get; set; } = new List<string>();
        public string LinkedInUrl { get; set; } = string.Empty;
        public string GitHubUrl { get; set; } = string.Empty;
        public string PortfolioUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual ApplicationUser User { get; set; } = null!;
    }

    public class Experience
    {
        public string Title { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; }
        public List<string> Technologies { get; set; } = new List<string>();
    }

    public class Education
    {
        public string Degree { get; set; } = string.Empty;
        public string Institution { get; set; } = string.Empty;
        public string FieldOfStudy { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Grade { get; set; } = string.Empty;
    }

    public class JobSearchCriteria
    {
        public string? Keywords { get; set; }
        public string? Location { get; set; }
        public string? Emirate { get; set; }
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public string? ExperienceLevel { get; set; }
        public string? JobType { get; set; }
        public List<string> Technologies { get; set; } = new List<string>();
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string SortBy { get; set; } = "PostedDate";
        public string SortOrder { get; set; } = "desc";
    }
}