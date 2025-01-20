namespace PolysomnographyProject.Services.Abstract.Sleep;

public interface ISleepPollingService
{
    Task<string> WaitForCommandAsync(string deviceLogin);
    void StartSleep(string deviceLogin);
    void StopSleep(string deviceLogin);
}