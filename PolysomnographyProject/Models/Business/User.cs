namespace PolysomnographyProject.Models.Business;

using PolysomnographyProject.Models.Business.Sleep;

public class User
{
    public Guid Id { get; set; }
    public string UniqueLogin { get; set; }
    
    public TelegramUserData TelegramUserData { get; set; }
    
    public PersonalSleepData PersonalSleepData { get; set; }
    
    public List<SleepResult> SleepResults { get; set; }
}