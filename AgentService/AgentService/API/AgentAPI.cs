using Microsoft.AspNetCore.Mvc;
using AgentService.DTOs;
using AgentService.Facades;
using AgentService.Models;

namespace AgentService.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgentApi : ControllerBase
    {
        private readonly AgentFacade _agentFacade;

        public AgentApi(AgentFacade agentFacade)
        {
            _agentFacade = agentFacade;
        }

        // POST: api/Agent
        [HttpPost]
        public IActionResult CreateAgent([FromBody] AgentDTO agentDto)
        {
            try
            {
                return Ok(new AgentDTO(_agentFacade.CreateAgent(agentDto)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // PUT: api/Agent
        [HttpPut]
        public IActionResult UpdateAgent([FromBody] AgentDTO agentDto)
        {
            try
            {
                var updatedAgent = _agentFacade.UpdateAgent(agentDto);
                return Ok(new AgentDTO(updatedAgent)); // Return the updated agent for verification
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // GET: api/Agent/{id}
        [HttpGet("{id}")]
        public IActionResult GetAgent(int id)
        {
            try
            {
                var agent = _agentFacade.GetAgent(id);
                return Ok(new AgentDTO(agent));
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        
        // PUT: api/Agent/Rating
        [HttpPut("Rating")]
        public IActionResult UpdateAgentWithRating([FromBody] UpdateRatingDTO updateRatingDto)
        {
            try
            {
                var updatedAgent = _agentFacade.UpdateAgentWithRating(updateRatingDto);
                return Ok(new AgentDTO(updatedAgent)); // Return the updated agent for verification
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        // PUT: api/Agent/Status
        [HttpPut("Status")]
        public IActionResult UpdateAgentStatus([FromBody] UpdateStatusAgentDTO updateStatusAgentDto)
        {
            try
            {
                var updatedAgent = _agentFacade.UpdateAgentStatus(updateStatusAgentDto);
                return Ok(new AgentDTO(updatedAgent)); // Return the updated agent for verification
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        // GET: api/Agent
        [HttpGet("All")]
        public IActionResult GetAllAgents()
        {
            try
            {
                var agents = _agentFacade.GetAllAgents();
                return Ok(agents);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}