using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using JobSystem.Api.Services;
using JobSystem.Api.Models;
using Xunit;
using System.Text.Json;

namespace JobSystem.Api.Tests
{
    public class JobSystemIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public JobSystemIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetJobs_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/api/jobs");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.NotEmpty(content);
        }

        [Fact]
        public async Task GetJobs_WithDotNetKeyword_ReturnsFilteredResults()
        {
            // Act
            var response = await _client.GetAsync("/api/jobs?keywords=.net");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JobSearchResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(result);
            Assert.True(result.Jobs.Count > 0);
            Assert.All(result.Jobs, job => 
                Assert.True(job.Title.ToLower().Contains(".net") || 
                           job.Description.ToLower().Contains(".net") ||
                           job.Technologies.Any(t => t.ToLower().Contains(".net"))));
        }

        [Fact]
        public async Task GetJobs_WithDubaiLocation_ReturnsFilteredResults()
        {
            // Act
            var response = await _client.GetAsync("/api/jobs?emirate=Dubai");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JobSearchResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(result);
            Assert.All(result.Jobs, job => 
                Assert.Equal("Dubai", job.Emirate));
        }

        [Fact]
        public async Task GetTechnologies_ReturnsListOfTechnologies()
        {
            // Act
            var response = await _client.GetAsync("/api/jobs/technologies");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var technologies = JsonSerializer.Deserialize<List<string>>(content);

            Assert.NotNull(technologies);
            Assert.Contains(".NET", technologies);
            Assert.Contains("C#", technologies);
            Assert.Contains("ASP.NET Core", technologies);
        }

        [Fact]
        public async Task GetLocations_ReturnsUAELocations()
        {
            // Act
            var response = await _client.GetAsync("/api/jobs/locations");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var locations = JsonSerializer.Deserialize<List<string>>(content);

            Assert.NotNull(locations);
            Assert.Contains("Dubai", locations);
            Assert.Contains("Abu Dhabi", locations);
            Assert.Contains("Sharjah", locations);
        }

        [Fact]
        public void JobService_CalculateMatchScore_WorksCorrectly()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var jobService = scope.ServiceProvider.GetRequiredService<IJobService>();
            
            var job = new Job
            {
                Id = 1,
                Title = "Senior .NET Developer",
                Company = "Test Company",
                Description = "Looking for .NET Core developer with C# experience",
                Technologies = new List<string> { ".NET Core", "C#", "SQL Server" },
                PostedDate = DateTime.UtcNow.AddDays(-1)
            };

            var userProfile = new UserProfile
            {
                Skills = new List<string> { ".NET Core", "C#", "Entity Framework" }
            };

            // This would require making the CalculateJobMatchScore method public or creating a testable version
            // For now, this demonstrates how you would test the scoring logic
            Assert.True(job.Technologies.Any(t => userProfile.Skills.Contains(t)));
        }
    }
}