using JobSystem.Api.Models;
using JobSystem.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly IJobService _jobService;
        private readonly ILogger<JobsController> _logger;

        public JobsController(IJobService jobService, ILogger<JobsController> logger)
        {
            _jobService = jobService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<JobSearchResponse>> SearchJobs([FromQuery] JobSearchCriteria criteria)
        {
            try
            {
                var (jobs, totalCount) = await _jobService.SearchJobsAsync(criteria);
                
                var response = new JobSearchResponse
                {
                    Jobs = jobs,
                    TotalCount = totalCount,
                    Page = criteria.Page,
                    PageSize = criteria.PageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / criteria.PageSize)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching jobs");
                return StatusCode(500, "An error occurred while searching jobs");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Job>> GetJob(int id)
        {
            try
            {
                var job = await _jobService.GetJobByIdAsync(id);
                if (job == null)
                {
                    return NotFound("Job not found");
                }

                return Ok(job);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting job {JobId}", id);
                return StatusCode(500, "An error occurred while retrieving the job");
            }
        }

        [HttpGet("recommended")]
        [Authorize]
        public async Task<ActionResult<List<Job>>> GetRecommendedJobs()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var jobs = await _jobService.GetRecommendedJobsAsync(userId);
                return Ok(jobs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recommended jobs");
                return StatusCode(500, "An error occurred while getting recommended jobs");
            }
        }

        [HttpGet("technologies")]
        public ActionResult<List<string>> GetTechnologies()
        {
            var technologies = new List<string>
            {
                ".NET", "C#", "ASP.NET Core", "ASP.NET MVC", "Web API", "Entity Framework",
                "Entity Framework Core", "Blazor", "WPF", "WinForms", "Xamarin", "MAUI",
                "JavaScript", "TypeScript", "React", "Angular", "Vue.js", "jQuery",
                "HTML5", "CSS3", "Bootstrap", "Tailwind CSS",
                "SQL Server", "MySQL", "PostgreSQL", "Oracle", "MongoDB", "Redis",
                "Azure", "AWS", "Google Cloud", "Docker", "Kubernetes",
                "Git", "GitHub", "Azure DevOps", "Jenkins", "CI/CD",
                "REST API", "GraphQL", "gRPC", "SignalR", "WCF",
                "Microservices", "Clean Architecture", "SOLID Principles", "Design Patterns"
            };

            return Ok(technologies.OrderBy(t => t).ToList());
        }

        [HttpGet("locations")]
        public ActionResult<List<string>> GetLocations()
        {
            var locations = new List<string>
            {
                "Dubai", "Abu Dhabi", "Sharjah", "Ajman", "Ras Al Khaimah", "Fujairah", "Umm Al Quwain",
                "Dubai Marina", "Downtown Dubai", "Business Bay", "DIFC", "Dubai Internet City",
                "Dubai Media City", "Jumeirah Lake Towers", "Al Barsha", "Deira",
                "Abu Dhabi City", "Al Ain", "Corniche", "Khalifa City", "Yas Island",
                "Sharjah City", "Al Majaz", "Al Nahda"
            };

            return Ok(locations.OrderBy(l => l).ToList());
        }

        [HttpGet("experience-levels")]
        public ActionResult<List<string>> GetExperienceLevels()
        {
            var levels = new List<string>
            {
                "Entry Level", "Junior", "Mid-Level", "Senior", "Lead", "Principal", "Architect"
            };

            return Ok(levels);
        }

        [HttpGet("job-types")]
        public ActionResult<List<string>> GetJobTypes()
        {
            var types = new List<string>
            {
                "Full-time", "Part-time", "Contract", "Freelance", "Internship", "Remote"
            };

            return Ok(types);
        }
    }

    public class JobSearchResponse
    {
        public List<Job> Jobs { get; set; } = new List<Job>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}