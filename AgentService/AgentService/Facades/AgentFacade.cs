using Microsoft.EntityFrameworkCore;
using AgentService.DTOs;
using AgentService.Models;

namespace AgentService.Facades
{
    public class AgentFacade
    {
        private readonly ApplicationDbContext _context;

        public AgentFacade(ApplicationDbContext context)
        {
            _context = context;
        }


        public Agent CreateAgent(AgentDTO agentDto)
        {
            if (string.IsNullOrWhiteSpace(agentDto.Name) || string.IsNullOrWhiteSpace(agentDto.Region) || agentDto.PhoneNumber == 0)
            {
                throw new Exception("All mandatory fields (Name, PhoneNumber, Region) must be provided.");
            }

            // Generate a unique AgentId for new agents
            string GenerateAgentId()
            {
                Random random = new Random();
                string digits = random.Next(1000, 9999).ToString();
                string letters = new string(Enumerable.Range(0, 4).Select(_ => (char)random.Next('A', 'Z' + 1)).ToArray());
                return digits + letters;
            }

            agentDto.AgentId = GenerateAgentId();
            while (_context.Agents.Any(a => a.AgentId == agentDto.AgentId))
            {
                agentDto.AgentId = GenerateAgentId();
            }

            // Create agent without setting Id explicitly
            Agent agent = new Agent(agentDto);
            _context.Agents.Add(agent);
            _context.SaveChanges();

            return agent;
        }


        public Agent GetAgent(int id)
        {
            Agent agent = _context.Agents.FirstOrDefault(a => a.Id == id);
            if (agent == null)
            {
                throw new Exception("Agent not found");
            }
            return agent;
        }
        
        public Agent UpdateAgent(AgentDTO agentDto)
        {
            // Retrieve the agent from the database by Id
            var agent = _context.Agents.FirstOrDefault(a => a.Id == agentDto.Id);

            // Check if the agent exists
            if (agent == null)
            {
                throw new Exception("Agent not found");
            }

            // Update only the allowed properties
            agent.Name = agentDto.Name;
            agent.PhoneNumber = agentDto.PhoneNumber;
            agent.AccountNumber = agentDto.AccountNumber;
            agent.Region = agentDto.Region;

            // Save changes to the database
            _context.SaveChanges();

            return agent;
        }

        public Agent UpdateAgentWithRating(UpdateRatingDTO updateRatingDto)
        {
            // Retrieve the agent from the database by Id
            var agent = _context.Agents.FirstOrDefault(a => a.Id == updateRatingDto.Id);

            // Check if the agent exists
            if (agent == null)
            {
                throw new Exception("Agent not found");
            }

            // Update only the allowed properties
            agent.Rating = updateRatingDto.Rating;
            agent.NumberOfRatings = updateRatingDto.NumberOfRatings;

            // Save changes to the database
            _context.SaveChanges();

            return agent;
        }
        
        //Update the agent status
        public Agent UpdateAgentStatus(UpdateStatusAgentDTO updateStatusAgentDto)
        {
            // Retrieve the agent from the database by Id
            var agent = _context.Agents.FirstOrDefault(a => a.Id == updateStatusAgentDto.Id);

            // Check if the agent exists
            if (agent == null)
            {
                throw new Exception("Agent not found");
            }

            // Enforce rules for status change
            if (agent.Status == "Delivering" && updateStatusAgentDto.Status == "Inactive")
            {
                throw new Exception("Cannot change status from 'Delivering' to 'Inactive'.");
            }
            else if (agent.Status == "Inactive" && updateStatusAgentDto.Status == "Delivering")
            {
                throw new Exception("Cannot change status from 'Inactive' to 'Delivering'.");
            }

            // Update the status if rules are not violated
            agent.Status = updateStatusAgentDto.Status;

            // Save changes to the database
            _context.SaveChanges();

            return agent;
        }

        // get all agents
        public List<Agent> GetAllAgents()
        {
            return _context.Agents.ToList();
        }
    }
}