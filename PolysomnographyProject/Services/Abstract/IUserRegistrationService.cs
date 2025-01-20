namespace PolysomnographyProject.Services.Abstract;

using Microsoft.AspNetCore.Http.HttpResults;
using Models;
using Models.Business;
using Models.Helping;
using OneOf;

public interface IUserRegistrationService
{
    Task<OneOf<User, BadRequest<string>, InternalServerError>> RegisterUserAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> ContainsByTelegramIdAsync(long telegramId, CancellationToken cancellationToken = default);
    Task<bool> ContainsByLoginAsync(string login, CancellationToken cancellationToken = default);
    Task<string?> GetUserLoginByTelegramIdAsync(long telegramId, CancellationToken cancellationToken = default);
}