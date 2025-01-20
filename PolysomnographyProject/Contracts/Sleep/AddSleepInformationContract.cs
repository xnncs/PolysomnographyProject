namespace PolysomnographyProject.Contracts.Sleep;

using Models.Helping;

public class AddSleepInformationContract
{
    public string Login { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public SleepResultData SleepResult { get; set; }
}