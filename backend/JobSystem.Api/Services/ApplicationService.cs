using JobSystem.Api.Models;
using JobSystem.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace JobSystem.Api.Services
{
    public interface IApplicationService
    {
        Task<List<JobApplication>> GetUserApplicationsAsync(string userId);
        Task<JobApplication?> GetApplicationByIdAsync(int id, string userId);
        Task<bool> ApplyForJobAsync(CreateApplicationRequest request, string userId);
        Task<bool> UpdateApplicationStatusAsync(int id, string status, string userId);
        Task<bool> WithdrawApplicationAsync(int id, string userId);
        Task<Dictionary<string, int>> GetApplicationStatsAsync(string userId);
    }

    public class ApplicationService : IApplicationService
    {
        private readonly JobSystemDbContext _context;
        private readonly ILogger<ApplicationService> _logger;

        public ApplicationService(JobSystemDbContext context, ILogger<ApplicationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<JobApplication>> GetUserApplicationsAsync(string userId)
        {
            return await _context.JobApplications
                .Include(a => a.Job)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.AppliedDate)
                .ToListAsync();
        }

        public async Task<JobApplication?> GetApplicationByIdAsync(int id, string userId)
        {
            return await _context.JobApplications
                .Include(a => a.Job)
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
        }

        public async Task<bool> ApplyForJobAsync(CreateApplicationRequest request, string userId)
        {
            try
            {
                // Check if user already applied for this job
                var existingApplication = await _context.JobApplications
                    .FirstOrDefaultAsync(a => a.UserId == userId && a.JobId == request.JobId);

                if (existingApplication != null)
                {
                    return false; // Already applied
                }

                // Check if job exists and is active
                var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == request.JobId && j.IsActive);
                if (job == null)
                {
                    return false; // Job not found or inactive
                }

                var application = new JobApplication
                {
                    UserId = userId,
                    JobId = request.JobId,
                    CoverLetter = request.CoverLetter,
                    ResumeFileName = request.ResumeFileName,
                    Status = "Applied",
                    AppliedDate = DateTime.UtcNow,
                    Notes = request.Notes
                };

                _context.JobApplications.Add(application);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying for job {JobId} by user {UserId}", request.JobId, userId);
                return false;
            }
        }

        public async Task<bool> UpdateApplicationStatusAsync(int id, string status, string userId)
        {
            try
            {
                var application = await _context.JobApplications
                    .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

                if (application == null) return false;

                var validStatuses = new[] { "Applied", "Reviewed", "Interview", "Rejected", "Hired", "Withdrawn" };
                if (!validStatuses.Contains(status)) return false;

                application.Status = status;
                
                if (status == "Interview" && !application.InterviewDate.HasValue)
                {
                    // This would typically be set by the employer, but for demo purposes
                    application.InterviewDate = DateTime.UtcNow.AddDays(7);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating application {ApplicationId} status", id);
                return false;
            }
        }

        public async Task<bool> WithdrawApplicationAsync(int id, string userId)
        {
            return await UpdateApplicationStatusAsync(id, "Withdrawn", userId);
        }

        public async Task<Dictionary<string, int>> GetApplicationStatsAsync(string userId)
        {
            var applications = await _context.JobApplications
                .Where(a => a.UserId == userId)
                .GroupBy(a => a.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            var stats = new Dictionary<string, int>
            {
                { "Total", applications.Sum(a => a.Count) },
                { "Applied", 0 },
                { "Reviewed", 0 },
                { "Interview", 0 },
                { "Rejected", 0 },
                { "Hired", 0 },
                { "Withdrawn", 0 }
            };

            foreach (var app in applications)
            {
                if (stats.ContainsKey(app.Status))
                {
                    stats[app.Status] = app.Count;
                }
            }

            return stats;
        }
    }

    public class CreateApplicationRequest
    {
        public int JobId { get; set; }
        public string CoverLetter { get; set; } = string.Empty;
        public string ResumeFileName { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}