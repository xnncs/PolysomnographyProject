using Microsoft.EntityFrameworkCore;
using PolysomnographyProject.Database.DataContexts;
using PolysomnographyProject.Endpoints;
using PolysomnographyProject.Handlers.Telegram;
using PolysomnographyProject.Services.Abstract;
using PolysomnographyProject.Services.Abstract.Sleep;
using PolysomnographyProject.Services.Implementation;
using PolysomnographyProject.Services.Implementation.Sleep;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUpdater;
using TelegramUpdater.Hosting;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IServiceCollection services = builder.Services;
ConfigurationManager configuration = builder.Configuration;

string environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? throw new ArgumentNullException(nameof(environment));

configuration
   .AddJsonFile("appsettings.json", false, false)
   .AddJsonFile($"appsettings.{environment}.json", true, false);

services.AddDbContext<ApplicationDbContext>(options =>
{
    string connectionString = configuration.GetConnectionString(nameof(ApplicationDbContext))
                           ?? throw new NullReferenceException("No connection string provided");

    options.UseNpgsql(connectionString)
           .EnableSensitiveDataLogging();
});

services.AddSingleton<ISleepPollingService, SleepPollingService>();

services.AddScoped<IUserRegistrationService, UserRegistrationService>();
services.AddScoped<ITelegramNotificationService, TelegramNotificationService>();
services.AddScoped<ISleepRegistrationService, SleepRegistrationService>();

services.AddTransient<ISleepQualityAnalyzer, SleepQualityAnalyzer>();

ConfigureTelegramUpdater(services, configuration);


WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/", () => "Connection established!");

app.MapSleepEndpoint();

app.Run();
return;


void ConfigureTelegramUpdater(IServiceCollection servicesCollection, IConfiguration configurationInstance)
{
    string token = configurationInstance.GetSection("TelegramBotToken").Value ??
                   throw new Exception("Server error: no telegram bot token configured");

    TelegramBotClient client = new(token);

    servicesCollection.AddHttpClient("TelegramBotClient").AddTypedClient<ITelegramBotClient>(httpClient => client);

    UpdaterOptions updaterOptions = new(10,
        allowedUpdates: [UpdateType.Message, UpdateType.CallbackQuery]);

    servicesCollection.AddTelegramUpdater(client, updaterOptions, botBuilder =>
    {
        botBuilder.AddDefaultExceptionHandler()
                  .AddScopedUpdateHandler<ScopedMessageHandler, Message>();
    });
}