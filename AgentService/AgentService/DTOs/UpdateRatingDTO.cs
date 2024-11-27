using AgentService.Models;

namespace AgentService.DTOs;

public class UpdateRatingDTO
{
    public int Id { get; set; } //primary key
    public double Rating { get; set; }
    public int NumberOfRatings { get; set; }


    public UpdateRatingDTO()
    {
    }

    public UpdateRatingDTO(int id, double rating, int numberOfRatings)
    {
        Id = id;
        Rating = rating;
        NumberOfRatings = numberOfRatings;
    }
}