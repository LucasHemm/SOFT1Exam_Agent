﻿using Microsoft.AspNetCore.Mvc;
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
                _agentFacade.CreateAgent(agentDto);
                return Ok("Agent created successfully");
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
                return Ok(updatedAgent); // Return the updated agent for verification
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
                return Ok(agent);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        
        // PUT: api/Agent/Rating
        [HttpPut("Rating")]
        public IActionResult UpdateAgentWithRating([FromBody] UpdateRatingAgentDTO updateRatingAgentDto)
        {
            try
            {
                var updatedAgent = _agentFacade.UpdateAgentWithRating(updateRatingAgentDto);
                return Ok(updatedAgent); // Return the updated agent for verification
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
                return Ok(updatedAgent); // Return the updated agent for verification
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}