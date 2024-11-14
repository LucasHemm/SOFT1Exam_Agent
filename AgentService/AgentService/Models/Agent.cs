using AgentService.DTOs;

namespace AgentService.Models;

public class Agent
{
    public int Id { get; set; } //primary key
    public String Name { get; set; }
    public int PhoneNumber { get; set; }
    public String AccountNumber { get; set; }
    public String AgentId { get; set; }
    public String Status { get; set; }
    public String Region { get; set; }
    public double Rating { get; set; }
    public int NumberOfRatings { get; set; }

    public Agent()
    {
    }

    public Agent(int id, string name, int phoneNumber, string accountNumber, string agentId, string status, string region, double rating, int numberOfRatings)
    {
        Id = id;
        Name = name;
        PhoneNumber = phoneNumber;
        AccountNumber = accountNumber;
        AgentId = agentId;
        Status = status;
        Region = region;
        Rating = rating;
        NumberOfRatings = numberOfRatings;
    }

    // public Agent(AgentDTO agentDto)
    // {
    //     Id = agentDto.Id;
    //     Name = agentDto.Name;
    //     PhoneNumber = agentDto.PhoneNumber;
    //     AccountNumber = agentDto.AgentId;
    //     Status = agentDto.Status;
    //     Region = agentDto.Region;
    //     Rating = agentDto.Rating;
    //     NumberOfRatings = agentDto.NumberOfRatings;
    // }

    public Agent(AgentDTO agentDto)
    {
        Name = agentDto.Name;
        PhoneNumber = agentDto.PhoneNumber;
        AccountNumber = agentDto.AccountNumber;
        AgentId = agentDto.AgentId;
        Status = agentDto.Status;
        Region = agentDto.Region;
        Rating = agentDto.Rating;
        NumberOfRatings = agentDto.NumberOfRatings;
    }
}