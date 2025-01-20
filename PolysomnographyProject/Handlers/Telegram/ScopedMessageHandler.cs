namespace PolysomnographyProject.Handlers.Telegram;

using System.Globalization;
using global::Telegram.Bot;
using global::Telegram.Bot.Types;
using global::Telegram.Bot.Types.Enums;
using global::Telegram.Bot.Types.InputFiles;
using global::Telegram.Bot.Types.ReplyMarkups;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http.HttpResults;
using Models.Business;
using Models.Business.Sleep;
using Models.Helping;
using OneOf;
using Services.Abstract;
using Services.Abstract.Sleep;
using TelegramUpdater.UpdateContainer;
using TelegramUpdater.UpdateHandlers.Scoped.ReadyToUse;
using User = Models.Business.User;
using BaseFile = System.IO.File;

public class ScopedMessageHandler : MessageHandler
{
    private readonly string[] _commands = ["/start", "/start_sleep", "/end_sleep", "/get_info"];
    
    private readonly string _bookFilePath =
        Path.Combine(Directory.GetCurrentDirectory(), "Public", "Books", "113956570.a4.pdf");
    private const string DateTimeFormat = "HH:mm";
    
    private readonly ILogger<ScopedMessageHandler> _logger;
    private readonly IUserRegistrationService _userRegistrationService;
    private readonly ISleepPollingService _sleepPollingService;
    private readonly ITelegramBotClient _botClient;

    public ScopedMessageHandler(ILogger<ScopedMessageHandler> logger, IUserRegistrationService userRegistrationService, ISleepPollingService sleepPollingService, ITelegramBotClient botClient)
    {
        _logger = logger;
        _userRegistrationService = userRegistrationService;
        _sleepPollingService = sleepPollingService;
        _botClient = botClient;
    }

    protected override async Task HandleAsync(IContainer<Message> container)
    {
        if (container.Update.Type != MessageType.Text) return;

        Message message = container.Update;
        _logger.LogInformation(
            $"Received message: {message.Text} from user with username: {message.From!.Username} with id: {message.From.Id}");


        if (message.Text!.StartsWith('/'))
        {
            (string? command, string? args) = GetCommandArgumentsObjectAsync(message.Text!);
            await OnCommandAsync(command, args, message);
        }
        else
        {
            await OnTextMessageAsync(message);
        }
    }

    private static (string, string) GetCommandArgumentsObjectAsync(string messageText)
    {
        int spaceIndex = messageText.IndexOf(' ');
        if (spaceIndex < 0) spaceIndex = messageText.Length;

        string command = messageText[..spaceIndex].ToLower();
        string args = messageText[spaceIndex..].TrimStart();

        return (command, args);
    }
    
    private async Task OnCommandAsync(string command, string args, Message message)
    {
        switch (command)
        {
            case "/start":
                await OnStartCommandAsync(args, message.Chat.Id, message.Chat.Username!);
                break;
            case "/start_sleep":
                await OnStartSleepAsync(message.Chat.Id);
                break;
            case "/end_sleep":
                await OnStopSleepAsync(message.Chat.Id);
                break;
            case "/get_info":
                await GetSleepInfoAsync(message.Chat.Id);
                break;
            default:
                await OnUnknownCommandAsync();
                break;
        }
    }
    
    private async Task OnTextMessageAsync(Message message)
    {
        string response = "Неправильный формат команды";
        await ResponseAsync(response);
    }

    private async Task OnUnknownCommandAsync()
    {
        _logger.LogInformation("Unknown command");

        await ResponseAsync("Неизвестная команда");
    }

    private async Task GetSleepInfoAsync(long telegramUserId)
    {
        await ResponseAsync("""
                                <strong>Качество сна</strong> — это важнейший аспект, влияющий на наше здоровье, благополучие и общее самочувствие. Недавние исследования показывают, что проблемы с сном являются одной из основных причин стресса, усталости и сниженной работоспособности. В этой статье мы рассмотрим несколько эффективных способов, которые помогут улучшить <strong>качество сна</strong> и, следовательно, повысить уровень жизни.
                                """, ParseMode.Html);
        
        await ResponseAsync("""
                                <i>1. Регулярный график сна</i>
                                
                                Одним из самых важных факторов для улучшения качества сна является соблюдение <strong>регулярного графика сна</strong>. Это означает, что нужно ложиться и вставать в одно и то же время каждый день, даже в выходные. Когда наш организм привыкает к определенному расписанию, он становится более настроенным на отдых в нужное время.
                                
                                Нарушения в графике сна могут привести к так называемому "циркадному диссонансу", когда внутренний биологический ритм сбивается. Это может вызвать бессонницу, усталость и затруднения с засыпанием. Важно понимать, что качественный сон возможен только в том случае, если наш организм привык к определенному режиму.
                                
                                <i>2. Создание комфортной обстановки для сна</i>
                                
                                Важнейшим аспектом является создание <strong>комфортной обстановки для сна</strong>. Это включает в себя выбор правильного матраса и подушки, а также поддержание оптимальной температуры в комнате. Исследования показывают, что комфортный сон возможен, если температура в комнате не превышает 18-22°C. Также стоит позаботиться о том, чтобы помещение было темным и тихим, так как лишний шум и свет могут мешать полноценному отдыху.
                                
                                Матрас и подушка должны поддерживать правильное положение тела, чтобы не возникало болей в спине или шее. Обратите внимание на тип вашего матраса — для разных людей подходят разные уровни жесткости. Если у вас возникли трудности с выбором, проконсультируйтесь с профессионалом, чтобы подобрать оптимальный вариант.
                                
                                <i>3. Ограничение использования электронных устройств перед сном</i>
                                
                                Экраны телевизоров, смартфонов, планшетов и компьютеров излучают <strong>синий свет</strong>, который нарушает выработку мелатонина — гормона, регулирующего сон. Это делает засыпание более трудным и снижает качество сна. Поэтому стоит избегать использования электронных устройств хотя бы за <strong>30-60 минут</strong> до сна. Лучше заменить экранные устройства на книги или спокойные разговоры.
                                
                                Кроме того, электронные устройства могут отвлекать нас от расслабления и подготовиться к сну, что также важно для хорошего ночного отдыха. Использование электронных гаджетов перед сном снижает уровень стресса, что затрудняет процесс засыпания.
                                
                                <i>4. Правильное питание и питьевой режим</i>
                                
                                <strong>Питание</strong> играет ключевую роль в качестве сна. Слишком тяжелая пища или кофеин, особенно если их потреблять вечером, могут значительно ухудшить засыпание. Вместо того чтобы употреблять жирную или острую пищу перед сном, лучше выбирать легкие ужины, такие как овощи, йогурт или творог.
                                
                                Кроме того, стоит помнить, что кофеин может оставаться в организме до <strong>8 часов</strong>, поэтому его потребление лучше ограничить во второй половине дня. Алкоголь тоже может нарушить цикл сна, несмотря на то, что он может вызвать сонливость. На самом деле алкоголь ухудшает фазу глубокого сна, что приводит к частым пробуждениям ночью.
                                
                                Также важно поддерживать правильный <strong>питьевой режим</strong>, однако стоит избегать слишком большого количества жидкости перед сном, чтобы не пришлось часто вставать по ночам.
                                """, ParseMode.Html);

        await ResponseAsync("""
                            <i>5. Физическая активность</i>


                            Физическая активность может значительно улучшить качество сна. Регулярные упражнения способствуют улучшению кровообращения, укреплению мышц и расслаблению тела, что облегчает процесс засыпания. Однако важно учитывать время тренировок — лучше избегать интенсивных нагрузок непосредственно перед сном. Упражнения, проводимые в вечернее время, могут повысить уровень энергии и помешать вам расслабиться.

                            Оптимальное время для физических нагрузок — это <strong>утро</strong> или день. Физическая активность улучшает циркуляцию крови и помогает организму расслабиться. Это, в свою очередь, способствует улучшению качества сна.

                            <i>6. Управление стрессом</i>

                            Стресс является одной из основных причин бессонницы и плохого сна. Когда мы переживаем, наш организм вырабатывает гормоны стресса, такие как кортизол, которые могут сильно мешать нормальному отдыху. Поэтому важно научиться <strong>управлять стрессом</strong> и расслабляться перед сном.

                            Для этого можно использовать различные техники расслабления, такие как медитация, дыхательные упражнения или йога. Также помогает принятие теплой ванны, что расслабляет мышцы и способствует лучшему засыпанию. Регулярная практика медитации и глубокого дыхания может снизить уровень стресса и улучшить общее состояние.

                            <i>7. Поддержание здорового веса</i>

                            Лишний вес может оказывать негативное влияние на качество сна, особенно если он приводит к проблемам с дыханием, таким как апноэ во сне. Апноэ характеризуется временной остановкой дыхания во сне, что приводит к частым пробуждениям и недостаточному отдыху. Поддержание <strong>здорового веса</strong> через правильное питание и регулярную физическую активность помогает предотвратить эти проблемы.

                            Если у вас есть подозрения на апноэ, стоит проконсультироваться с врачом. Он может назначить необходимые обследования и предложить лечение для улучшения качества сна.

                            <i>8. Использование методов релаксации</i>

                            В дополнение к традиционным методам релаксации, таким как медитация, можно попробовать <strong>слушать расслабляющую музыку</strong> или звуки природы перед сном. Это может помочь снизить уровень стресса и подготовить организм к отдыху. Исследования показывают, что музыка с медленным темпом и гармоничными мелодиями может способствовать расслаблению и улучшению качества сна.

                            Релаксация перед сном также может включать чтение книг, просмотр спокойных телепередач или проведение времени с близкими, что поможет снять напряжение и настроиться на отдых.
                            """, ParseMode.Html);
        
        await ResponseAsync("""
                                <i>Заключение</i>
                                
                                Качество сна — это важный аспект нашего здоровья и благополучия. Следуя указанным рекомендациям, можно значительно улучшить свою способность засыпать и поддерживать высокий уровень энергии в течение дня. Основными принципами являются соблюдение регулярного графика сна, создание комфортных условий для отдыха, правильное питание, физическая активность и управление стрессом.
                                
                                Не забывайте, что каждый человек уникален, и то, что подходит одному, может не подойти другому. Важно прислушиваться к своему организму и выбирать методы, которые наиболее эффективно работают для вас. Помните, что качественный сон — это залог здоровья, продуктивности и хорошего настроения.
                                """, ParseMode.Html);
        
        if (!BaseFile.Exists(_bookFilePath))
        {
            _logger.LogInformation($"File not found: {_bookFilePath}");
            throw new FileNotFoundException($"The file at path '{_bookFilePath}' does not exist.");
        }

        
        await using Stream stream = BaseFile.OpenRead(_bookFilePath);

        const string bookCapture = """
                                   Литература по полисомнографии:
                                   <strong>"Зачем мы спим. Новая наука о сне и сновидениях"</strong> - <i>Мэттью Уолкер</i>
                                   """;
        await _botClient.SendDocumentAsync(telegramUserId, document: new InputOnlineFile(stream, "book.pdf"),
            caption: bookCapture, parseMode: ParseMode.Html);
    }
    
    private async Task OnStartSleepAsync(long telegramUserId)
    {
        string? userLogin = await _userRegistrationService.GetUserLoginByTelegramIdAsync(telegramUserId);
        if (userLogin == null)
        {
            await ResponseAsync("Сначала пройдите процедуру регистрации для того чтобы выполнить эту комманду");
        }
        _sleepPollingService.StartSleep(userLogin);

        string onStartSleepRequestMessage = """
                                            Комманда была успешно <strong>послана</strong> на вашу плату.
                                            Приятного сна!
                                            """;
        await ResponseAsync(onStartSleepRequestMessage, ParseMode.Html);
    }
    
    private async Task OnStopSleepAsync(long telegramUserId)
    {
        string? userLogin = await _userRegistrationService.GetUserLoginByTelegramIdAsync(telegramUserId);
        if (userLogin == null)
        {
            await ResponseAsync("Сначала пройдите процедуру регистрации для того чтобы выполнить эту комманду");
        }
        _sleepPollingService.StopSleep(userLogin);
        
        string onStopSleepRequestMessage = """
                                            Комманда была успешно <strong>послана</strong>.
                                            Через некоторое время вам должен будет прийти отчет о вашем сне
                                            """;
        await ResponseAsync(onStopSleepRequestMessage, ParseMode.Html);
    }
    
    [Obsolete("No longer used.")]
    private async Task OnUpdateIpCommandAsync(long telegramUserId)
    {
        if (!await _userRegistrationService.ContainsByTelegramIdAsync(telegramUserId))
        {
            await ResponseAsync(
                "Вы не можете обновить IP адресс своего устройста так как вы еще не <strong>зарегестрированны</strong>",
                ParseMode.Html);
        }

        const string updateIpRequestMessage = """
                                        Введите <strong>новый</strong> IP вашего устройства
                                        """;
        string? newIp = await AwaitTextInputAsync(TimeSpan.FromSeconds(600), updateIpRequestMessage, ParseMode.Html);
        if (newIp == null)
        {
            return;
        }

        try
        {
            // await _userRegistrationService.UpdateUserIpAsync(telegramUserId, newIp);
            // await ResponseAsync("Вы успешно поменяли IP вашего устройства на {}");
        }
        catch (Exception exception)
        {
            _logger.LogError("Error while updating user with telegram id {0}: {1}", telegramUserId, exception.Message);
            await ResponseAsync("Что то пошло не так, попробуйте позже");
        }
        
    }
    
    private async Task OnStartCommandAsync(string args, long telegramUserId, string username)
    {
        IReplyMarkup replyMarkup = new ReplyKeyboardMarkup(_commands
           .Select(x => new KeyboardButton(x)));
        
        if (await _userRegistrationService.ContainsByTelegramIdAsync(telegramUserId))
        {
            await ResponseAsync("Добро пожаловать!", ParseMode.Html, replyMarkup: replyMarkup);
            return;
        }

        const string welcomeText = """
                                   <strong>Добро пожаловать!</strong>
                                   <i>Введите логин вашего устройства для регестрации</i>
                                   """;
        string? login = await AwaitTextInputAsync(TimeSpan.FromSeconds(600), welcomeText, ParseMode.Html);
        if (login == null)
        {
            return;
        }

        while (await _userRegistrationService.ContainsByLoginAsync(login))
        {
            login = await AwaitTextInputAsync(TimeSpan.FromSeconds(600), welcomeText, ParseMode.Html);
            if (login == null)
            {
                return;
            }
        }

        const string startSleepTimeRequestText = """
                                                 <strong>Хорошо!</strong> Теперь введите время, в которое планируете ложиться спать (в формате hh:mm) по московскому времени. 
                                                 """;
        string? startSleepTimeString = await AwaitTextInputAsync(TimeSpan.FromSeconds(600), startSleepTimeRequestText, ParseMode.Html);
        if (startSleepTimeString == null)
        {
            return;
        }

        DateTime startSleepTime;
        while (!DateTime.TryParseExact(startSleepTimeString, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out startSleepTime))
        {
            const string invalidTimeFormatText = "Вы ввели время в неверном формате. <strong>Попробуйте снова</strong>";
            startSleepTimeString = await AwaitTextInputAsync(TimeSpan.FromSeconds(600), invalidTimeFormatText, ParseMode.Html);
            if (startSleepTimeString == null)
            {
                return;
            }
        }
        
        const string endSleepTimeRequestText = """
                                                 <strong>Прекрасно!</strong> Теперь введите время, в которое планируете просыпаться (в формате hh:mm) по московскому времени. 
                                                 """;
        string? endSleepTimeString = await AwaitTextInputAsync(TimeSpan.FromSeconds(600), endSleepTimeRequestText, ParseMode.Html);
        if (endSleepTimeString == null)
        {
            return;
        }

        DateTime endSleepTime;
        while (!DateTime.TryParseExact(endSleepTimeString, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out endSleepTime))
        {
            const string invalidTimeFormatText = "Вы ввели время в неверном формате. <strong>Попробуйте снова</strong>";
            endSleepTimeString = await AwaitTextInputAsync(TimeSpan.FromSeconds(600), invalidTimeFormatText, ParseMode.Html);
            if (endSleepTimeString == null)
            {
                return;
            }
        }

        User created = new User()
        {
            Id = Guid.NewGuid(),
            UniqueLogin = login,
            PersonalSleepData = new PersonalSleepData()
            {
                SleepTimePreferences = new SleepTimePreferences()
                {
                    StartTime = startSleepTime.ToUniversalTime(),
                    EndTime = endSleepTime.ToUniversalTime()
                },
            },
            TelegramUserData = new TelegramUserData()
            {
                TelegramId = telegramUserId,
                TelegramUsername = username
            },
            SleepResults = []
        };

        
        OneOf<User, BadRequest<string>, InternalServerError> creatingResult = await _userRegistrationService.RegisterUserAsync(created);
        if (creatingResult.IsT1)
        {
            BadRequest<string> result = creatingResult.AsT1;
            await ResponseAsync(result.Value!, ParseMode.Html);
            return;
        }
        
        if (creatingResult.IsT2)
        {
            await ResponseAsync("Что то пошло не так, попробуйте позже", ParseMode.Html);
            return;
        }

        User user = creatingResult.AsT0;
        string onRegistrationMessageText = $"""
                                            <i>Благодарим за регестрацию!</i>
                                            Теперь вы зарегистрированны под уникальным логином: <strong>{user.UniqueLogin}</strong>
                                            """;
        await ResponseAsync(onRegistrationMessageText, ParseMode.Html, replyMarkup: replyMarkup);
    }
}