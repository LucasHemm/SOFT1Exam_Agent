using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AgentService;
using AgentService.DTOs;
using AgentService.Facades;
using Testcontainers.MsSql;

namespace AgentServiceTests
{
    public class IntegrationTest : IAsyncLifetime
    {
        private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("YourStrong!Passw0rd")
            .Build();

        private string _connectionString;

        public async Task InitializeAsync()
        {
            await _msSqlContainer.StartAsync();
            _connectionString = _msSqlContainer.GetConnectionString();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                context.Database.Migrate();
            }
        }

        public async Task DisposeAsync()
        {
            await _msSqlContainer.DisposeAsync().AsTask();
        }

        [Fact]
        public void ShouldCreateAgent()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                AgentFacade agentFacade = new AgentFacade(context);

                AgentDTO agentDto = new AgentDTO
                {
                    Name = "John Doe",
                    PhoneNumber = 12345678,
                    AccountNumber = "123-456-789",
                    Status = "Active",
                    Region = "North",
                    Rating = 4.7,
                    NumberOfRatings = 150
                };

                var agent = agentFacade.CreateAgent(agentDto);
                var createdAgent = context.Agents.Find(agent.Id);

                Assert.NotNull(createdAgent);
                Assert.Equal(agentDto.Name, createdAgent.Name);
            }
        }

        [Fact]
        public void ShouldUpdateAgent()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                AgentFacade agentFacade = new AgentFacade(context);

                AgentDTO agentDto = new AgentDTO
                {
                    Name = "John Doe",
                    PhoneNumber = 12345678,
                    AccountNumber = "123-456-789",
                    Status = "Active",
                    Region = "North",
                    Rating = 4.7,
                    NumberOfRatings = 150
                };

                var agent = agentFacade.CreateAgent(agentDto);

                // Update the agent details
                agentDto.Name = "Jane Smith";
                agentDto.Id = agent.Id;
                agentDto.PhoneNumber = 87654321;
                agentDto.Region = "South";

                var updatedAgent = agentFacade.UpdateAgent(agentDto);
                var retrievedAgent = context.Agents.Find(agent.Id);

                Assert.NotNull(retrievedAgent);
                Assert.Equal("Jane Smith", retrievedAgent.Name);
                Assert.Equal(87654321, retrievedAgent.PhoneNumber);
                Assert.Equal("South", retrievedAgent.Region);
            }
        }

        [Fact]
        public void ShouldGetAgent()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                AgentFacade agentFacade = new AgentFacade(context);

                AgentDTO agentDto = new AgentDTO
                {
                    Name = "John Doe",
                    PhoneNumber = 12345678,
                    AccountNumber = "123-456-789",
                    Status = "Active",
                    Region = "North",
                    Rating = 4.7,
                    NumberOfRatings = 150
                };

                var agent = agentFacade.CreateAgent(agentDto);
                var retrievedAgent = agentFacade.GetAgent(agent.Id);

                Assert.NotNull(retrievedAgent);
                Assert.Equal(agentDto.Name, retrievedAgent.Name);
            }
        }

        [Fact]
        public void ShouldThrowExceptionWhenAgentNotFound()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                AgentFacade agentFacade = new AgentFacade(context);
                Assert.Throws<Exception>(() => agentFacade.GetAgent(999));
            }
        }
        
        [Fact]
        public void ShouldUpdateAgentWithRating()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                AgentFacade agentFacade = new AgentFacade(context);

                AgentDTO agentDto = new AgentDTO
                {
                    Name = "John Doe",
                    PhoneNumber = 12345678,
                    AccountNumber = "123-456-789",
                    Status = "Active",
                    Region = "North",
                    Rating = 4.7,
                    NumberOfRatings = 150
                };

                var agent = agentFacade.CreateAgent(agentDto);

                // Update the agent rating
                UpdateRatingAgentDTO updateRatingAgentDto = new UpdateRatingAgentDTO
                {
                    Id = agent.Id,
                    Rating = 4.8,
                    NumberOfRatings = 200
                };

                var updatedAgent = agentFacade.UpdateAgentWithRating(updateRatingAgentDto);
                var retrievedAgent = context.Agents.Find(agent.Id);

                Assert.NotNull(retrievedAgent);
                Assert.Equal(4.8, retrievedAgent.Rating);
                Assert.Equal(200, retrievedAgent.NumberOfRatings);
            }
        }
        
        //ShouldUpdateAgentStatus
        [Fact]
        public void ShouldUpdateAgentStatus()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                AgentFacade agentFacade = new AgentFacade(context);

                AgentDTO agentDto = new AgentDTO
                {
                    Name = "John Doe",
                    PhoneNumber = 12345678,
                    AccountNumber = "123-456-789",
                    Status = "Active",
                    Region = "North",
                    Rating = 4.7,
                    NumberOfRatings = 150
                };

                var agent = agentFacade.CreateAgent(agentDto);

                // Update the agent status
                UpdateStatusAgentDTO updateStatusAgentDto = new UpdateStatusAgentDTO
                {
                    Id = agent.Id,
                    Status = "Inactive"
                };

                var updatedAgent = agentFacade.UpdateAgentStatus(updateStatusAgentDto);
                var retrievedAgent = context.Agents.Find(agent.Id);

                Assert.NotNull(retrievedAgent);
                Assert.Equal("Inactive", retrievedAgent.Status);
            }
        }
        
        //Get all agents, by getting a list of multiple agents
        [Fact]
        public void ShouldGetAllAgents()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                AgentFacade agentFacade = new AgentFacade(context);

                AgentDTO agentDto1 = new AgentDTO
                {
                    Name = "John Doe",
                    PhoneNumber = 12345678,
                    AccountNumber = "123-456-789",
                    Status = "Active",
                    Region = "North",
                    Rating = 4.7,
                    NumberOfRatings = 150
                };

                AgentDTO agentDto2 = new AgentDTO
                {
                    Name = "Jane Smith",
                    PhoneNumber = 87654321,
                    AccountNumber = "987-654-321",
                    Status = "Active",
                    Region = "South",
                    Rating = 4.5,
                    NumberOfRatings = 100
                };

                agentFacade.CreateAgent(agentDto1);
                agentFacade.CreateAgent(agentDto2);

                var agents = agentFacade.GetAllAgents();

                Assert.NotNull(agents);
                Assert.Equal(2, agents.Count);
            }
        }
    }
}
