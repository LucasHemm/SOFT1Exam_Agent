using AgentService.Models;

namespace AgentService.DTOs;

public class UpdateRatingAgentDTO
{
    public int Id { get; set; } //primary key
    public double Rating { get; set; }
    public int NumberOfRatings { get; set; }


    public UpdateRatingAgentDTO()
    {
    }

    public UpdateRatingAgentDTO(int id, double rating, int numberOfRatings)
    {
        Id = id;
        Rating = rating;
        NumberOfRatings = numberOfRatings;
    }
}