namespace PolysomnographyProject.Endpoints;

using Contracts.Sleep;
using Database.DataContexts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Models.Business;
using Services.Abstract;
using Services.Abstract.Sleep;

public static class SleepEndpoints
{
    public static IEndpointRouteBuilder MapSleepEndpoint(this IEndpointRouteBuilder app)
    {
        IEndpointRouteBuilder endpoints = app.MapGroup("api/sleep");

        endpoints.MapPost("report", AddSleepInformationAsync);

        endpoints.MapGet("commands/{deviceLogin}", WaitForCommandAsync);
        
        return endpoints;
    }

    private static async Task<Ok<string>> WaitForCommandAsync(string deviceLogin,
                                                              ISleepPollingService sleepPollingService, ILogger logger)
    {
        string command = await sleepPollingService.WaitForCommandAsync(deviceLogin);
        logger.LogInformation("Received a command to {deviceLogin}: {command}", deviceLogin, command);
        return TypedResults.Ok(command);
    }

    private static async Task<Results<Ok, BadRequest>> AddSleepInformationAsync(AddSleepInformationContract request, ApplicationDbContext applicationDbContext, ISleepRegistrationService sleepRegistrationService, CancellationToken cancellationToken)
    {
        User? user = await applicationDbContext.Users.AsNoTracking()
                                               .FirstOrDefaultAsync(u => u.UniqueLogin == request.Login, cancellationToken);
        if (user == null)
        {
            return TypedResults.BadRequest();
        }
        
        await sleepRegistrationService.RegisterMessageAsync(request, cancellationToken);
        return TypedResults.Ok();
    }
}