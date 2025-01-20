namespace PolysomnographyProject.Services.Implementation.Sleep;

using Abstract.Sleep;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PolysomnographyProject.Contracts.Sleep;
using PolysomnographyProject.Database.DataContexts;
using PolysomnographyProject.Models.Business;
using PolysomnographyProject.Models.Business.Sleep;
using PolysomnographyProject.Services.Abstract;

public class SleepRegistrationService : ISleepRegistrationService
{
    private readonly ITelegramNotificationService _notificationService;
    private readonly ILogger<SleepRegistrationService> _logger;
    private readonly ApplicationDbContext _dbContext;
    
    public SleepRegistrationService(ITelegramNotificationService notificationService, ILogger<SleepRegistrationService> logger, ApplicationDbContext dbContext)
    {
        _notificationService = notificationService;
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task RegisterMessageAsync(AddSleepInformationContract contract, CancellationToken cancellationToken = default)
    {
        User user = await _dbContext.Users
                                    .Include(u => u.SleepResults)
                                    .FirstOrDefaultAsync(u => u.UniqueLogin == contract.Login, cancellationToken)
                                     ?? throw new NullReferenceException($"User with login {contract.Login} does not exist");

        await using IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        SleepResult sleepResult = new SleepResult()
        {
            Id = Guid.NewGuid(),
            User = user,
            StartTime = contract.StartTime,
            EndTime = contract.EndTime,
            Data = contract.SleepResult
        };
        
        user.SleepResults.Add(sleepResult);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        await transaction.CommitAsync(cancellationToken);
        _logger.LogInformation($"Sleep registration for {contract.Login} successful");
        
        await _notificationService.SendSleepResultMessageAsync(sleepResult, cancellationToken);
    }
}