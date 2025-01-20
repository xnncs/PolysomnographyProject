namespace PolysomnographyProject.Services.Abstract.Sleep;

using PolysomnographyProject.Contracts.Sleep;

public interface ISleepRegistrationService
{
    Task RegisterMessageAsync(AddSleepInformationContract contract, CancellationToken cancellationToken = default);
}