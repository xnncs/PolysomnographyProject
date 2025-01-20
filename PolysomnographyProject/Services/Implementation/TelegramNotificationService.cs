namespace PolysomnographyProject.Services.Implementation;

using Abstract;
using Abstract.Sleep;
using Contracts.Sleep;
using Models.Business.Sleep;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

public class TelegramNotificationService : ITelegramNotificationService
{
    private readonly ILogger<TelegramNotificationService> _logger;
    private readonly ITelegramBotClient _botClient;
    private readonly ISleepQualityAnalyzer _sleepQualityAnalyzer;

    public TelegramNotificationService(ILogger<TelegramNotificationService> logger, ITelegramBotClient botClient, ISleepQualityAnalyzer sleepQualityAnalyzer)
    {
        _logger = logger;
        _botClient = botClient;
        _sleepQualityAnalyzer = sleepQualityAnalyzer;
    }

    public async Task SendSleepResultMessageAsync(SleepResult result, CancellationToken cancellationToken = default)
    {
        string qualityMessage = _sleepQualityAnalyzer.AssessSleepQuality(result.Data);

        string resultMessage = $"""
                                <strong>Ваш сон завершился, вы спали с {result.StartTime.ToShortTimeString()} до {result.EndTime.ToShortTimeString()}</strong>

                                Аналитика итогов вашего сна:
                                <i>{qualityMessage}</i>
                                """;
        
        await _botClient.SendTextMessageAsync(result.User.TelegramUserData.TelegramId, resultMessage, ParseMode.Html, cancellationToken: cancellationToken);
        _logger.LogInformation("Message was sent to {0}, with quality message: {1}", result.User.TelegramUserData.TelegramId, qualityMessage);
    }
}