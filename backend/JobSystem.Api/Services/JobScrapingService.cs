using JobSystem.Api.Models;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Globalization;

namespace JobSystem.Api.Services
{
    public interface IJobScrapingService
    {
        Task ScrapeAllJobsAsync();
        Task ScrapeBaytJobsAsync();
        Task ScrapeDubizzleJobsAsync();
        Task ScrapeGulfTalentJobsAsync();
        Task ScrapeNaukriGulfJobsAsync();
    }

    public class JobScrapingService : IJobScrapingService
    {
        private readonly IJobService _jobService;
        private readonly HttpClient _httpClient;
        private readonly ILogger<JobScrapingService> _logger;

        private readonly string[] DotNetKeywords = {
            ".net", "dotnet", "c#", "csharp", "asp.net", "mvc", "web api", "entity framework",
            "blazor", "xamarin", "maui", "wpf", "winforms", "azure", "sql server"
        };

        private readonly string[] UAEEmirates = {
            "Dubai", "Abu Dhabi", "Sharjah", "Ajman", "Ras Al Khaimah", "Fujairah", "Umm Al Quwain"
        };

        public JobScrapingService(IJobService jobService, HttpClient httpClient, ILogger<JobScrapingService> logger)
        {
            _jobService = jobService;
            _httpClient = httpClient;
            _logger = logger;
            
            _httpClient.DefaultRequestHeaders.Add("User-Agent", 
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
        }

        public async Task ScrapeAllJobsAsync()
        {
            _logger.LogInformation("Starting job scraping for all sources...");
            
            var tasks = new List<Task>
            {
                ScrapeBaytJobsAsync(),
                ScrapeDubizzleJobsAsync(),
                ScrapeGulfTalentJobsAsync(),
                ScrapeNaukriGulfJobsAsync()
            };

            await Task.WhenAll(tasks);
            
            _logger.LogInformation("Completed job scraping for all sources");
        }

        public async Task ScrapeBaytJobsAsync()
        {
            try
            {
                _logger.LogInformation("Starting Bayt.com job scraping...");
                
                var searchUrls = new[]
                {
                    "https://www.bayt.com/en/jobs/dot-net-developer-jobs-uae/",
                    "https://www.bayt.com/en/jobs/c-sharp-developer-jobs-uae/",
                    "https://www.bayt.com/en/jobs/full-stack-developer-jobs-uae/",
                    "https://www.bayt.com/en/jobs/software-developer-jobs-uae/"
                };

                foreach (var url in searchUrls)
                {
                    await ScrapeBaytSearchPage(url);
                    await Task.Delay(2000); // Rate limiting
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scraping Bayt.com jobs");
            }
        }

        private async Task ScrapeBaytSearchPage(string url)
        {
            try
            {
                var html = await _httpClient.GetStringAsync(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var jobCards = doc.DocumentNode.SelectNodes("//div[@class='jb-card']");
                if (jobCards == null) return;

                foreach (var card in jobCards.Take(20)) // Limit per page
                {
                    var job = ParseBaytJobCard(card);
                    if (job != null && IsDotNetRelated(job))
                    {
                        await _jobService.SaveJobAsync(job);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scraping Bayt search page: {Url}", url);
            }
        }

        private Job? ParseBaytJobCard(HtmlNode card)
        {
            try
            {
                var titleNode = card.SelectSingleNode(".//h3[@class='jb-card-title']/a");
                var companyNode = card.SelectSingleNode(".//div[@class='jb-card-company']/a");
                var locationNode = card.SelectSingleNode(".//div[@class='jb-card-location']");
                var salaryNode = card.SelectSingleNode(".//div[@class='jb-card-salary']");
                var descNode = card.SelectSingleNode(".//div[@class='jb-card-desc']");

                if (titleNode == null || companyNode == null) return null;

                var job = new Job
                {
                    Title = CleanText(titleNode.InnerText),
                    Company = CleanText(companyNode.InnerText),
                    Description = CleanText(descNode?.InnerText ?? ""),
                    Location = CleanText(locationNode?.InnerText ?? ""),
                    ExternalUrl = "https://www.bayt.com" + titleNode.GetAttributeValue("href", ""),
                    Source = "Bayt",
                    ExternalId = ExtractIdFromUrl(titleNode.GetAttributeValue("href", "")),
                    PostedDate = DateTime.UtcNow,
                    Currency = "AED",
                    JobType = "Full-time"
                };

                // Parse salary
                if (salaryNode != null)
                {
                    ParseSalary(salaryNode.InnerText, job);
                }

                // Determine emirate from location
                job.Emirate = DetermineEmirate(job.Location);

                // Extract technologies from job title and description
                job.Technologies = ExtractTechnologies(job.Title + " " + job.Description);

                return job;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing Bayt job card");
                return null;
            }
        }

        public async Task ScrapeDubizzleJobsAsync()
        {
            try
            {
                _logger.LogInformation("Starting Dubizzle job scraping...");
                
                var searchUrls = new[]
                {
                    "https://dubizzle.com/jobs/technology/software-development/",
                    "https://dubizzle.com/jobs/technology/web-development/"
                };

                foreach (var url in searchUrls)
                {
                    await ScrapeDubizzleSearchPage(url);
                    await Task.Delay(2000);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scraping Dubizzle jobs");
            }
        }

        private async Task ScrapeDubizzleSearchPage(string url)
        {
            try
            {
                var html = await _httpClient.GetStringAsync(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var jobCards = doc.DocumentNode.SelectNodes("//div[contains(@class, 'listing')]");
                if (jobCards == null) return;

                foreach (var card in jobCards.Take(20))
                {
                    var job = ParseDubizzleJobCard(card);
                    if (job != null && IsDotNetRelated(job))
                    {
                        await _jobService.SaveJobAsync(job);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scraping Dubizzle search page: {Url}", url);
            }
        }

        private Job? ParseDubizzleJobCard(HtmlNode card)
        {
            try
            {
                var titleNode = card.SelectSingleNode(".//h3/a");
                var companyNode = card.SelectSingleNode(".//div[contains(@class, 'company')]");
                var locationNode = card.SelectSingleNode(".//div[contains(@class, 'location')]");

                if (titleNode == null) return null;

                var job = new Job
                {
                    Title = CleanText(titleNode.InnerText),
                    Company = CleanText(companyNode?.InnerText ?? "Not specified"),
                    Location = CleanText(locationNode?.InnerText ?? "UAE"),
                    ExternalUrl = "https://dubizzle.com" + titleNode.GetAttributeValue("href", ""),
                    Source = "Dubizzle",
                    ExternalId = ExtractIdFromUrl(titleNode.GetAttributeValue("href", "")),
                    PostedDate = DateTime.UtcNow,
                    Currency = "AED",
                    JobType = "Full-time"
                };

                job.Emirate = DetermineEmirate(job.Location);
                job.Technologies = ExtractTechnologies(job.Title);

                return job;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing Dubizzle job card");
                return null;
            }
        }

        public async Task ScrapeGulfTalentJobsAsync()
        {
            try
            {
                _logger.LogInformation("Starting GulfTalent job scraping...");
                
                var searchUrl = "https://www.gulftalent.com/jobs/software-developer-united-arab-emirates";
                await ScrapeGulfTalentSearchPage(searchUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scraping GulfTalent jobs");
            }
        }

        private async Task ScrapeGulfTalentSearchPage(string url)
        {
            // Similar implementation pattern as above
            // Implementation would follow the same pattern with GulfTalent-specific selectors
        }

        public async Task ScrapeNaukriGulfJobsAsync()
        {
            try
            {
                _logger.LogInformation("Starting NaukriGulf job scraping...");
                
                var searchUrl = "https://www.naukrigulf.com/dot-net-developer-jobs-in-uae";
                await ScrapeNaukriGulfSearchPage(searchUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scraping NaukriGulf jobs");
            }
        }

        private async Task ScrapeNaukriGulfSearchPage(string url)
        {
            // Similar implementation pattern as above
            // Implementation would follow the same pattern with NaukriGulf-specific selectors
        }

        private bool IsDotNetRelated(Job job)
        {
            var content = (job.Title + " " + job.Description + " " + string.Join(" ", job.Technologies)).ToLower();
            return DotNetKeywords.Any(keyword => content.Contains(keyword.ToLower()));
        }

        private string DetermineEmirate(string location)
        {
            var locationLower = location.ToLower();
            return UAEEmirates.FirstOrDefault(emirate => locationLower.Contains(emirate.ToLower())) ?? "UAE";
        }

        private List<string> ExtractTechnologies(string text)
        {
            var technologies = new List<string>();
            var textLower = text.ToLower();

            var techKeywords = new[]
            {
                ".NET", "C#", "ASP.NET", "MVC", "Web API", "Entity Framework", "Blazor",
                "JavaScript", "TypeScript", "React", "Angular", "Vue.js", "Node.js",
                "SQL Server", "MySQL", "PostgreSQL", "MongoDB", "Redis",
                "Azure", "AWS", "Docker", "Kubernetes", "Git", "DevOps"
            };

            foreach (var tech in techKeywords)
            {
                if (textLower.Contains(tech.ToLower()))
                {
                    technologies.Add(tech);
                }
            }

            return technologies.Distinct().ToList();
        }

        private void ParseSalary(string salaryText, Job job)
        {
            var numbers = Regex.Matches(salaryText, @"\d+(?:,\d+)*")
                .Cast<Match>()
                .Select(m => decimal.TryParse(m.Value.Replace(",", ""), out var val) ? val : 0)
                .Where(v => v > 0)
                .ToList();

            if (numbers.Count >= 2)
            {
                job.SalaryMin = numbers.Min();
                job.SalaryMax = numbers.Max();
            }
            else if (numbers.Count == 1)
            {
                job.SalaryMax = numbers[0];
            }
        }

        private string CleanText(string text)
        {
            return string.IsNullOrEmpty(text) ? "" : 
                Regex.Replace(text.Trim(), @"\s+", " ");
        }

        private string ExtractIdFromUrl(string url)
        {
            var match = Regex.Match(url, @"[^/]+(?=/?$)");
            return match.Success ? match.Value : Guid.NewGuid().ToString();
        }
    }
}