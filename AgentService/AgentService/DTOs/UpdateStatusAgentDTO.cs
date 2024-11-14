namespace AgentService.DTOs;

public class UpdateStatusAgentDTO
{
    public int Id { get; set; } //primary key
    public String Status { get; set; }
    
    
    public UpdateStatusAgentDTO()
    {
    }
    
    
    public UpdateStatusAgentDTO(int id, string status)
    {
        Id = id;
        Status = status;
    }
}