namespace PolysomnographyProject.Services.Abstract;

using Contracts.Sleep;
using Models.Business.Sleep;

public interface ITelegramNotificationService
{
    Task SendSleepResultMessageAsync(SleepResult result, CancellationToken cancellationToken = default);
}