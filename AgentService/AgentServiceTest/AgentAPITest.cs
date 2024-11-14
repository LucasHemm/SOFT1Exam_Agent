using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AgentService;
using AgentService.API;
using AgentService.DTOs;
using AgentService.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Testcontainers.MsSql;
using Xunit;

namespace AgentServiceTest
{
    public class AgentApiTest : IAsyncLifetime
    {
        private readonly MsSqlContainer _msSqlContainer;
        private readonly WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        public AgentApiTest()
        {
            _msSqlContainer = new MsSqlBuilder()
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                .WithPassword("YourStrong!Passw0rd")
                .WithCleanUp(true)
                .Build();

            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        // Remove the existing ApplicationDbContext registration
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                        if (descriptor != null)
                        {
                            services.Remove(descriptor);
                        }

                        // Use SQL Server container connection string
                        services.AddDbContext<ApplicationDbContext>(options =>
                        {
                            options.UseSqlServer(_msSqlContainer.GetConnectionString());
                        });

                        // Ensure the database is created
                        var sp = services.BuildServiceProvider();
                        using (var scope = sp.CreateScope())
                        {
                            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                            db.Database.Migrate();
                        }
                    });
                });
        }

        public async Task InitializeAsync()
        {
            await _msSqlContainer.StartAsync();
            _client = _factory.CreateClient();
        }

        public async Task DisposeAsync()
        {
            _client.Dispose();
            await _msSqlContainer.DisposeAsync();
            _factory.Dispose();
        }

        private StringContent GetStringContent(object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        [Fact]
        public async Task CreateAgent_ShouldReturnSuccess()
        {
            var agentDto = new AgentDTO(0, "Test Agent", 12345678, "12345", "", "Active", "Region1", 4.5, 10);
            var response = await _client.PostAsync("/api/AgentApi", GetStringContent(agentDto));
            response.EnsureSuccessStatusCode();

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var agent = await context.Agents.FirstOrDefaultAsync(a => a.Name == agentDto.Name);

                Assert.NotNull(agent);
                Assert.Equal(agentDto.Name, agent.Name);
                Assert.Equal(agentDto.Region, agent.Region);
            }
        }

        [Fact]
        public async Task UpdateAgent_ShouldReturnSuccess()
        {
            // Arrange
            var initialAgentDto = new AgentDTO(0, "Initial Agent", 87654321, "54321", "", "Inactive", "Region2", 3.0, 5);

            // Create the initial agent
            var createResponse = await _client.PostAsync("/api/AgentApi", GetStringContent(initialAgentDto));
            createResponse.EnsureSuccessStatusCode();

            int agentId;
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var agent = await context.Agents.FirstOrDefaultAsync(a => a.Name == initialAgentDto.Name);
                Assert.NotNull(agent); // Ensure the agent is created
                agentId = agent.Id;
            }

            // Prepare the updated agent DTO
            var updatedAgentDto = new AgentDTO(agentId, "Updated Agent", 56565656, "64563", "", "Active", "Region2", 4.2, 8);

            // Act
            var updateResponse = await _client.PutAsync("/api/AgentApi", GetStringContent(updatedAgentDto));
            updateResponse.EnsureSuccessStatusCode();

            // Assert
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var updatedAgent = await context.Agents.AsNoTracking().FirstOrDefaultAsync(a => a.Id == agentId);

                Assert.NotNull(updatedAgent); // Ensure the agent is updated
                Assert.Equal(updatedAgentDto.Name, updatedAgent.Name);
                Assert.Equal(updatedAgentDto.PhoneNumber, updatedAgent.PhoneNumber);
                Assert.Equal(updatedAgentDto.Region, updatedAgent.Region);
            }
        }


        [Fact]
        public async Task GetAgent_ShouldReturnAgentDetails()
        {
            var agentDto = new AgentDTO(0, "Sample Agent", 12312312, "11111", "", "Active", "Region1", 4.0, 12);
            var createResponse = await _client.PostAsync("/api/AgentApi", GetStringContent(agentDto));
            createResponse.EnsureSuccessStatusCode();

            int agentId;
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var agent = await context.Agents.FirstOrDefaultAsync(a => a.Name == agentDto.Name);
                agentId = agent.Id;
            }

            var response = await _client.GetAsync($"/api/AgentApi/{agentId}");
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("Sample Agent", responseContent);
        }

        [Fact]
        public async Task UpdateAgentWithRating_ShouldReturnSuccess()
        {
            var agentDto = new AgentDTO(0, "Rating Agent", 11223344, "99999", "", "Active", "Region3", 3.5, 7);
            var createResponse = await _client.PostAsync("/api/AgentApi", GetStringContent(agentDto));
            createResponse.EnsureSuccessStatusCode();

            int agentId;
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var agent = await context.Agents.FirstOrDefaultAsync(a => a.Name == agentDto.Name);
                agentId = agent.Id;
            }

            var updateRatingDto = new UpdateRatingAgentDTO(agentId, 4.8, 20);
            var response = await _client.PutAsync("/api/AgentApi/Rating", GetStringContent(updateRatingDto));
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("4.8", responseContent);
        }
    }
}
