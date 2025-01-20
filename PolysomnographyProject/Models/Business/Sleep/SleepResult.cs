namespace PolysomnographyProject.Models.Business.Sleep;

using Helping;

public class SleepResult
{
    public Guid Id { get; set; }
    public User User { get; set; }
    
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    
    public SleepResultData Data { get; set; }
}