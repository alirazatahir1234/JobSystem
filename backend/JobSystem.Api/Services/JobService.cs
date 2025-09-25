using JobSystem.Api.Models;
using JobSystem.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace JobSystem.Api.Services
{
    public interface IJobService
    {
        Task<(List<Job> Jobs, int TotalCount)> SearchJobsAsync(JobSearchCriteria criteria);
        Task<Job?> GetJobByIdAsync(int id);
        Task<List<Job>> GetRecommendedJobsAsync(string userId);
        Task<bool> SaveJobAsync(Job job);
        Task<bool> UpdateJobAsync(Job job);
        Task<bool> DeactivateExpiredJobsAsync();
    }

    public class JobService : IJobService
    {
        private readonly JobSystemDbContext _context;

        public JobService(JobSystemDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Job> Jobs, int TotalCount)> SearchJobsAsync(JobSearchCriteria criteria)
        {
            var query = _context.Jobs.Where(j => j.IsActive);

            // Apply keyword search
            if (!string.IsNullOrEmpty(criteria.Keywords))
            {
                var keywords = criteria.Keywords.ToLower();
                query = query.Where(j => 
                    j.Title.ToLower().Contains(keywords) ||
                    j.Company.ToLower().Contains(keywords) ||
                    j.Description.ToLower().Contains(keywords) ||
                    j.Requirements.ToLower().Contains(keywords));
            }

            // Apply location filters
            if (!string.IsNullOrEmpty(criteria.Location))
            {
                query = query.Where(j => j.Location.ToLower().Contains(criteria.Location.ToLower()));
            }

            if (!string.IsNullOrEmpty(criteria.Emirate))
            {
                query = query.Where(j => j.Emirate.ToLower() == criteria.Emirate.ToLower());
            }

            // Apply salary filters
            if (criteria.MinSalary.HasValue)
            {
                query = query.Where(j => j.SalaryMin >= criteria.MinSalary || j.SalaryMax >= criteria.MinSalary);
            }

            if (criteria.MaxSalary.HasValue)
            {
                query = query.Where(j => j.SalaryMax <= criteria.MaxSalary || j.SalaryMin <= criteria.MaxSalary);
            }

            // Apply experience level filter
            if (!string.IsNullOrEmpty(criteria.ExperienceLevel))
            {
                query = query.Where(j => j.ExperienceLevel.ToLower() == criteria.ExperienceLevel.ToLower());
            }

            // Apply job type filter
            if (!string.IsNullOrEmpty(criteria.JobType))
            {
                query = query.Where(j => j.JobType.ToLower() == criteria.JobType.ToLower());
            }

            // Apply technology filters
            if (criteria.Technologies.Any())
            {
                foreach (var tech in criteria.Technologies)
                {
                    var techLower = tech.ToLower();
                    query = query.Where(j => j.Technologies.Any(t => t.ToLower().Contains(techLower)) ||
                                           j.Description.ToLower().Contains(techLower) ||
                                           j.Requirements.ToLower().Contains(techLower));
                }
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply sorting
            query = criteria.SortBy.ToLower() switch
            {
                "title" => criteria.SortOrder.ToLower() == "asc" 
                    ? query.OrderBy(j => j.Title) 
                    : query.OrderByDescending(j => j.Title),
                "company" => criteria.SortOrder.ToLower() == "asc" 
                    ? query.OrderBy(j => j.Company) 
                    : query.OrderByDescending(j => j.Company),
                "salary" => criteria.SortOrder.ToLower() == "asc" 
                    ? query.OrderBy(j => j.SalaryMax ?? 0) 
                    : query.OrderByDescending(j => j.SalaryMax ?? 0),
                _ => criteria.SortOrder.ToLower() == "asc" 
                    ? query.OrderBy(j => j.PostedDate) 
                    : query.OrderByDescending(j => j.PostedDate)
            };

            // Apply pagination
            var jobs = await query
                .Skip((criteria.Page - 1) * criteria.PageSize)
                .Take(criteria.PageSize)
                .ToListAsync();

            return (jobs, totalCount);
        }

        public async Task<Job?> GetJobByIdAsync(int id)
        {
            return await _context.Jobs
                .Include(j => j.Applications)
                .FirstOrDefaultAsync(j => j.Id == id && j.IsActive);
        }

        public async Task<List<Job>> GetRecommendedJobsAsync(string userId)
        {
            var userProfile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (userProfile == null || !userProfile.Skills.Any())
            {
                // Return recent .NET jobs if no profile
                return await _context.Jobs
                    .Where(j => j.IsActive && 
                               (j.Title.ToLower().Contains(".net") || 
                                j.Title.ToLower().Contains("c#") ||
                                j.Technologies.Any(t => t.ToLower().Contains(".net") || t.ToLower().Contains("c#"))))
                    .OrderByDescending(j => j.PostedDate)
                    .Take(10)
                    .ToListAsync();
            }

            // Score jobs based on skill match
            var allJobs = await _context.Jobs
                .Where(j => j.IsActive)
                .ToListAsync();

            var scoredJobs = allJobs.Select(job => new
            {
                Job = job,
                Score = CalculateJobMatchScore(job, userProfile)
            })
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .ThenByDescending(x => x.Job.PostedDate)
            .Take(20)
            .Select(x => x.Job)
            .ToList();

            return scoredJobs;
        }

        public async Task<bool> SaveJobAsync(Job job)
        {
            try
            {
                // Check if job already exists
                var existingJob = await _context.Jobs
                    .FirstOrDefaultAsync(j => j.Source == job.Source && j.ExternalId == job.ExternalId);

                if (existingJob != null)
                {
                    // Update existing job
                    existingJob.Title = job.Title;
                    existingJob.Company = job.Company;
                    existingJob.Description = job.Description;
                    existingJob.Requirements = job.Requirements;
                    existingJob.Location = job.Location;
                    existingJob.Emirate = job.Emirate;
                    existingJob.SalaryMin = job.SalaryMin;
                    existingJob.SalaryMax = job.SalaryMax;
                    existingJob.ExperienceLevel = job.ExperienceLevel;
                    existingJob.JobType = job.JobType;
                    existingJob.Technologies = job.Technologies;
                    existingJob.Benefits = job.Benefits;
                    existingJob.UpdatedAt = DateTime.UtcNow;
                    existingJob.IsActive = true;
                }
                else
                {
                    _context.Jobs.Add(job);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateJobAsync(Job job)
        {
            try
            {
                _context.Jobs.Update(job);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeactivateExpiredJobsAsync()
        {
            try
            {
                var expiredJobs = await _context.Jobs
                    .Where(j => j.IsActive && j.PostedDate < DateTime.UtcNow.AddDays(-30))
                    .ToListAsync();

                foreach (var job in expiredJobs)
                {
                    job.IsActive = false;
                    job.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private int CalculateJobMatchScore(Job job, UserProfile profile)
        {
            int score = 0;

            // Check skill matches
            foreach (var skill in profile.Skills)
            {
                if (job.Technologies.Any(t => t.ToLower().Contains(skill.ToLower())) ||
                    job.Description.ToLower().Contains(skill.ToLower()) ||
                    job.Requirements.ToLower().Contains(skill.ToLower()) ||
                    job.Title.ToLower().Contains(skill.ToLower()))
                {
                    score += 10;
                }
            }

            // Bonus for .NET related jobs
            var dotnetKeywords = new[] { ".net", "c#", "asp.net", "entity framework", "blazor", "mvc", "web api" };
            foreach (var keyword in dotnetKeywords)
            {
                if (job.Title.ToLower().Contains(keyword) ||
                    job.Technologies.Any(t => t.ToLower().Contains(keyword)) ||
                    job.Description.ToLower().Contains(keyword))
                {
                    score += 5;
                }
            }

            // Recent jobs get higher score
            var daysSincePosted = (DateTime.UtcNow - job.PostedDate).Days;
            if (daysSincePosted <= 7) score += 5;
            else if (daysSincePosted <= 14) score += 3;
            else if (daysSincePosted <= 30) score += 1;

            return score;
        }
    }
}