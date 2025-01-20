namespace PolysomnographyProject.Services.Implementation;

using Abstract;
using Database.DataContexts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Models.Business;
using Models.Business.Sleep;
using Models.Helping;
using OneOf;

public class UserRegistrationService : IUserRegistrationService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<UserRegistrationService> _logger;

    public UserRegistrationService(ApplicationDbContext dbContext, ILogger<UserRegistrationService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<bool> ContainsByLoginAsync(string login, CancellationToken cancellationToken)
    {
        try
        {
            return await _dbContext.Users.AnyAsync(u => u.UniqueLogin == login, cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "UserRegistrationService contains error");
            return false;
        }
    }

    public async Task<string?> GetUserLoginByTelegramIdAsync(long telegramId, CancellationToken cancellationToken = default)
    {
        User? user = await _dbContext.Users.AsNoTracking()
                               .FirstOrDefaultAsync(u => u.TelegramUserData.TelegramId == telegramId,
                                    cancellationToken: cancellationToken);
        return user?.UniqueLogin;
    }

    public async Task<bool> ContainsByTelegramIdAsync(long telegramId, CancellationToken cancellationToken)
    {
        try
        {
            return await _dbContext.Users.AnyAsync(u => u.TelegramUserData.TelegramId == telegramId, cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "UserRegistrationService contains error");
            return false;
        }
    }

    public async Task<OneOf<User, BadRequest<string>, InternalServerError>> RegisterUserAsync(User user, CancellationToken cancellationToken)
    {
        bool alreadyExists = await _dbContext.Users.AnyAsync(u => u.UniqueLogin == user.UniqueLogin, cancellationToken: cancellationToken);
        if (alreadyExists)
        {
            return TypedResults.BadRequest<string>("Login already exists.");
        }

        try
        {
            await using IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync();
        
            await _dbContext.Users.AddAsync(user, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.Log(LogLevel.Error, exception, "An error occured while registering user");
            return TypedResults.InternalServerError();
        }

        return user;
    }
}