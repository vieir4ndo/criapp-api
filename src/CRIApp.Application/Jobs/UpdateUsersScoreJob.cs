using CRIApp.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace CRIApp.Application.Jobs;

public class UpdateUsersScoreJob : IRecurrentJob
{
    private readonly ILogger<UpdateUsersScoreJob> _logger;
    private readonly IUserService _userService;
    
    public UpdateUsersScoreJob(ILogger<UpdateUsersScoreJob> logger,  IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    public async Task Run()
    {
        _logger.LogInformation("Job {JobName} started", nameof(UpdateUsersScoreJob));

        var users = await _userService.GetAllUsers();

        foreach (var user in users)
        {
            await _userService.UpdateScore(user);
        }

        _logger.LogInformation("Job {JobName} completed", nameof(UpdateUsersScoreJob));
    }
}