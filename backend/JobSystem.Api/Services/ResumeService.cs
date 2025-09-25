using JobSystem.Api.Models;
using JobSystem.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace JobSystem.Api.Services
{
    public interface IResumeService
    {
        Task<string> GenerateResumeAsync(string userId, int? jobId = null);
        Task<ResumeTemplate> GetResumeTemplateAsync(string templateType = "professional");
        Task<List<string>> GetAvailableTemplatesAsync();
    }

    public class ResumeService : IResumeService
    {
        private readonly JobSystemDbContext _context;
        private readonly ILogger<ResumeService> _logger;

        public ResumeService(JobSystemDbContext context, ILogger<ResumeService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<string> GenerateResumeAsync(string userId, int? jobId = null)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
                
                if (user == null)
                {
                    throw new ArgumentException("User not found");
                }

                Job? targetJob = null;
                if (jobId.HasValue)
                {
                    targetJob = await _context.Jobs.FindAsync(jobId.Value);
                }

                var resume = await BuildResumeContentAsync(user, profile, targetJob);
                return resume;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating resume for user {UserId}", userId);
                throw;
            }
        }

        public Task<ResumeTemplate> GetResumeTemplateAsync(string templateType = "professional")
        {
            var template = templateType.ToLower() switch
            {
                "modern" => GetModernTemplate(),
                "creative" => GetCreativeTemplate(),
                "minimal" => GetMinimalTemplate(),
                _ => GetProfessionalTemplate()
            };
            return Task.FromResult(template);
        }

        public Task<List<string>> GetAvailableTemplatesAsync()
        {
            return Task.FromResult(new List<string> { "Professional", "Modern", "Creative", "Minimal" });
        }

        private Task<string> BuildResumeContentAsync(ApplicationUser user, UserProfile? profile, Job? targetJob)
        {
            var resumeBuilder = new StringBuilder();
            
            // Header
            resumeBuilder.AppendLine($"# {user.FirstName} {user.LastName}");
            resumeBuilder.AppendLine($"**Email:** {user.Email}");
            
            if (profile != null)
            {
                if (!string.IsNullOrEmpty(profile.LinkedInUrl))
                    resumeBuilder.AppendLine($"**LinkedIn:** {profile.LinkedInUrl}");
                
                if (!string.IsNullOrEmpty(profile.GitHubUrl))
                    resumeBuilder.AppendLine($"**GitHub:** {profile.GitHubUrl}");
                
                if (!string.IsNullOrEmpty(profile.PortfolioUrl))
                    resumeBuilder.AppendLine($"**Portfolio:** {profile.PortfolioUrl}");
            }

            resumeBuilder.AppendLine();

            // Professional Summary
            if (profile != null && !string.IsNullOrEmpty(profile.Summary))
            {
                resumeBuilder.AppendLine("## Professional Summary");
                resumeBuilder.AppendLine(profile.Summary);
                resumeBuilder.AppendLine();
            }
            else
            {
                resumeBuilder.AppendLine("## Professional Summary");
                resumeBuilder.AppendLine("Experienced .NET Full Stack Developer with expertise in building scalable web applications using modern technologies. Passionate about clean code, best practices, and delivering high-quality software solutions.");
                resumeBuilder.AppendLine();
            }

            // Skills - Tailored to job if provided
            resumeBuilder.AppendLine("## Technical Skills");
            if (profile?.Skills.Any() == true)
            {
                var skills = profile.Skills;
                if (targetJob != null)
                {
                    // Prioritize skills that match the job
                    var jobSkills = targetJob.Technologies.Concat(ExtractSkillsFromText(targetJob.Description + " " + targetJob.Requirements)).ToList();
                    skills = skills.OrderByDescending(skill => 
                        jobSkills.Any(js => js.ToLower().Contains(skill.ToLower()) || skill.ToLower().Contains(js.ToLower()))
                    ).ToList();
                }
                
                resumeBuilder.AppendLine($"**Technologies:** {string.Join(", ", skills)}");
            }
            else
            {
                resumeBuilder.AppendLine("**Technologies:** .NET Core, C#, ASP.NET MVC, Web API, Entity Framework, JavaScript, TypeScript, React, Angular, SQL Server, Azure");
            }
            resumeBuilder.AppendLine();

            // Experience
            if (profile?.Experience.Any() == true)
            {
                resumeBuilder.AppendLine("## Professional Experience");
                foreach (var exp in profile.Experience.OrderByDescending(e => e.StartDate))
                {
                    resumeBuilder.AppendLine($"### {exp.Title} - {exp.Company}");
                    resumeBuilder.AppendLine($"*{exp.StartDate:MMM yyyy} - {(exp.EndDate?.ToString("MMM yyyy") ?? "Present")}*");
                    resumeBuilder.AppendLine();
                    resumeBuilder.AppendLine(exp.Description);
                    
                    if (exp.Technologies.Any())
                    {
                        resumeBuilder.AppendLine($"**Technologies Used:** {string.Join(", ", exp.Technologies)}");
                    }
                    resumeBuilder.AppendLine();
                }
            }

            // Education
            if (profile?.Education.Any() == true)
            {
                resumeBuilder.AppendLine("## Education");
                foreach (var edu in profile.Education.OrderByDescending(e => e.StartDate))
                {
                    resumeBuilder.AppendLine($"### {edu.Degree} in {edu.FieldOfStudy}");
                    resumeBuilder.AppendLine($"**{edu.Institution}** - {edu.StartDate:yyyy} to {(edu.EndDate?.Year.ToString() ?? "Present")}");
                    if (!string.IsNullOrEmpty(edu.Grade))
                    {
                        resumeBuilder.AppendLine($"**Grade:** {edu.Grade}");
                    }
                    resumeBuilder.AppendLine();
                }
            }

            // Certifications
            if (profile?.Certifications.Any() == true)
            {
                resumeBuilder.AppendLine("## Certifications");
                foreach (var cert in profile.Certifications)
                {
                    resumeBuilder.AppendLine($"- {cert}");
                }
                resumeBuilder.AppendLine();
            }

            // Add job-specific section if targeting a specific job
            if (targetJob != null)
            {
                resumeBuilder.AppendLine("## Why I'm a Great Fit");
                resumeBuilder.AppendLine($"This resume has been tailored for the {targetJob.Title} position at {targetJob.Company}. ");
                resumeBuilder.AppendLine($"My experience with {string.Join(", ", targetJob.Technologies.Take(3))} makes me an ideal candidate for this role.");
                resumeBuilder.AppendLine();
            }

            return Task.FromResult(resumeBuilder.ToString());
        }

        private List<string> ExtractSkillsFromText(string text)
        {
            var skills = new List<string>();
            var techKeywords = new[]
            {
                ".NET", "C#", "ASP.NET", "MVC", "Web API", "Entity Framework", "Blazor",
                "JavaScript", "TypeScript", "React", "Angular", "Vue", "Node.js",
                "SQL Server", "MySQL", "PostgreSQL", "MongoDB", "Redis",
                "Azure", "AWS", "Docker", "Kubernetes"
            };

            foreach (var keyword in techKeywords)
            {
                if (text.ToLower().Contains(keyword.ToLower()))
                {
                    skills.Add(keyword);
                }
            }

            return skills;
        }

        private ResumeTemplate GetProfessionalTemplate()
        {
            return new ResumeTemplate
            {
                Name = "Professional",
                Description = "Clean, traditional format suitable for corporate environments",
                CssStyles = @"
                    body { font-family: 'Times New Roman', serif; color: #333; }
                    .header { text-align: center; border-bottom: 2px solid #000; }
                    .section { margin: 20px 0; }
                    .section-title { font-weight: bold; font-size: 18px; text-transform: uppercase; }
                "
            };
        }

        private ResumeTemplate GetModernTemplate()
        {
            return new ResumeTemplate
            {
                Name = "Modern",
                Description = "Contemporary design with clean lines and modern typography",
                CssStyles = @"
                    body { font-family: 'Arial', sans-serif; color: #2c3e50; }
                    .header { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; }
                    .section { margin: 15px 0; padding: 10px; border-left: 3px solid #667eea; }
                "
            };
        }

        private ResumeTemplate GetCreativeTemplate()
        {
            return new ResumeTemplate
            {
                Name = "Creative",
                Description = "Bold design for creative and tech-forward roles",
                CssStyles = @"
                    body { font-family: 'Helvetica', sans-serif; color: #34495e; }
                    .header { background: #e74c3c; color: white; padding: 30px; border-radius: 10px; }
                    .section { background: #ecf0f1; padding: 15px; margin: 10px 0; border-radius: 5px; }
                "
            };
        }

        private ResumeTemplate GetMinimalTemplate()
        {
            return new ResumeTemplate
            {
                Name = "Minimal",
                Description = "Simple, clean design focusing on content",
                CssStyles = @"
                    body { font-family: 'Georgia', serif; color: #555; line-height: 1.6; }
                    .header { border-bottom: 1px solid #ddd; padding-bottom: 10px; }
                    .section { margin: 25px 0; }
                    .section-title { font-size: 16px; font-weight: normal; text-decoration: underline; }
                "
            };
        }
    }

    public class ResumeTemplate
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CssStyles { get; set; } = string.Empty;
    }
}