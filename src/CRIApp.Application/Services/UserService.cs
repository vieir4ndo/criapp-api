using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using CRIApp.Application.Resources;
using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Exceptions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace CRIApp.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IMessageUser _messageUser;
    private readonly ILogger<UserService> _logger;
    private readonly IStringLocalizer<AppMessages> _localizer;

    public UserService(
        IUserRepository repository,
        IRefreshTokenRepository refreshTokenRepository,
        IMessageUser messageUser,
        ILogger<UserService> logger,
        IStringLocalizer<AppMessages> localizer)
    {
        _repository = repository;
        _refreshTokenRepository = refreshTokenRepository;
        _messageUser = messageUser;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task Create(CreateUserDto dto)
    {
        var existing = await _repository.GetByUsername(dto.Username);

        if (existing is not null)
        {
            _logger.LogWarning("Registration attempt with already existing username {Username}", dto.Username);
            throw new ValidationException(_localizer["UserAlreadyExists"]);
        }

        var user = dto.ToDomainObject();
        await _repository.Create(user);
        await _messageUser.NotifyAccountCreation(user);
    }

    public async Task<User?> GetByUsername(string username)
    {
        return await _repository.GetByUsername(username);
    }

    public async Task<User> GetAndValidateById(Guid id)
    {
        var user = await _repository.GetById(id);

        if (user is null)
        {
            _logger.LogWarning("User {UserId} not found", id);
            throw new NotFoundException(_localizer["UserNotFound"]);
        }

        return user;
    }

    public async Task Update(Guid userId, UpdateUserDto dto)
    {
        var user = await GetAndValidateById(userId);
        user.Update(dto.Name, dto.BirthDate, dto.Sex, dto.EducationalLevel, dto.FamilyType, dto.NumberOfChildren);
        await _repository.Update(user);
    }

    public async Task UpdatePassword(Guid userId, string newPassword)
    {
        var user = await GetAndValidateById(userId);
        user.UpdatePassword(newPassword);
        await _repository.Update(user);
    }

    public async Task Deactivate(Guid userId)
    {
        var user = await GetAndValidateById(userId);
        user.Deactivate();
        await _repository.Update(user);
        await _refreshTokenRepository.DeleteByUserId(userId);
    }
}
