using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using CRIApp.Application.Resources;
using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Exceptions;
using CRIApp.Domain.Extensions;
using CRIApp.Domain.Settings;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CRIApp.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly JwtTokenSettings _jwtTokenSettings;
    private readonly IMessageUser _messageUser;
    private readonly IUserResetPasswordCodeRepository _userResetPasswordCodeRepository;
    private readonly ILogger<UserService> _logger;
    private readonly IStringLocalizer<AppMessages> _localizer;

    public UserService(IUserRepository repository,
        IOptions<JwtTokenSettings> jwtTokenSettings,
        IMessageUser messageUser,
        IUserResetPasswordCodeRepository userResetPasswordCodeRepository,
        ILogger<UserService> logger,
        IStringLocalizer<AppMessages> localizer)
    {
        _repository = repository;
        _jwtTokenSettings = jwtTokenSettings.Value;
        _messageUser = messageUser;
        _userResetPasswordCodeRepository = userResetPasswordCodeRepository;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task Create(CreateUserDto dto)
    {
        var userExisting = await _repository.GetByUsername(dto.Username);

        if (userExisting is not null)
        {
            _logger.LogWarning("Registration attempt with already existing username {Username}", dto.Username);
            throw new ValidationException(_localizer["UserAlreadyExists"]);
        }

        var user = dto.ToDomainObject();
        await _repository.Create(user);
        await _messageUser.NotifyAccountCreation(user);
    }

    public async Task<TokenDto> Login(LoginDto dto)
    {
        var user = await GetByUsername(dto.Username);

        if (user is not null && user.Active && user.VerifyPassword(dto.Password))
        {
            return GenerateToken(user);
        }

        _logger.LogWarning("Failed login attempt for username {Username}", dto.Username);
        throw new ValidationException(_localizer["InvalidCredentials"]);
    }

    private TokenDto GenerateToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtTokenSettings.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var expiresIn = DateTime.Now.AddMinutes(_jwtTokenSettings.ExpiresInMinutes);

        var claims = new List<Claim>()
        {
            new("id", user.Id.ToString())
        };

        var sectoken = new JwtSecurityToken(_jwtTokenSettings.Issuer,
            _jwtTokenSettings.Issuer,
            claims,
            expires: expiresIn,
            signingCredentials: credentials);

        return new TokenDto()
        {
            Token = new JwtSecurityTokenHandler().WriteToken(sectoken),
            ExpiresIn = expiresIn,
            User = new UserDto(user)
        };
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

    public async Task ForgotPassword(string username)
    {
        var user = await GetByUsername(username);
        
        if (user is null)
        {
            _logger.LogWarning("Forgot password requested for unknown username {Username}", username);
            throw new ValidationException(_localizer["UserNotFound"]);
        }

        //TODO: Extract this to settings
        var code = RandomExtensions.GenerateRandomString(5);
        
        await _userResetPasswordCodeRepository.Create(new UserResetPasswordCode(user.Id, code, DateTime.Now.AddSeconds(180).ToUniversalTime()));
        await _messageUser.NotifyForgotPassword(user, code);
    }

    public async Task ResetPassword(ResetPasswordDto dto)
    {
        var user = await GetByUsername(dto.Username);
        
        if (user is null)
        {
            _logger.LogWarning("Password reset attempted for unknown username {Username}", dto.Username);
            throw new ValidationException(_localizer["UserNotFound"]);
        }

        if (!user.VerifyResetPasswordCode(dto.Code))
        {
            _logger.LogWarning("Invalid or expired reset code used for user {UserId}", user.Id);
            throw new ValidationException(_localizer["InvalidResetCode"]);
        }
        
        user.UpdatePassword(dto.NewPassword);
        
        await _repository.Update(user);
    }

    public async Task UpdateScore(User user)
    {
        user.UpdateScore();
        await _repository.Update(user);
    }
    
    public async Task<IList<User>> GetAllUsers()
    {
        return await _repository.GetAll();
    }

    public async Task<IList<AwardDto>> GetAwards(Guid userId)
    {
        var user = await _repository.GetByIdWithAwardsAndRecommendations(userId);

        if (user is null)
        {
            _logger.LogWarning("User {UserId} not found when fetching awards", userId);
            throw new NotFoundException(_localizer["UserNotFound"]);
        }

        return user.Awards.Select(a => new AwardDto(a)).ToList();
    }

    public async Task<IList<RecommendationDto>> GetRecommendations(Guid userId)
    {
        var user = await _repository.GetByIdWithAwardsAndRecommendations(userId);

        if (user is null)
        {
            _logger.LogWarning("User {UserId} not found when fetching recommendations", userId);
            throw new NotFoundException(_localizer["UserNotFound"]);
        }

        return user.Recommendations.Select(r => new RecommendationDto(r)).ToList();
    }

    public async Task Deactivate(Guid userId)
    {
        var user = await GetAndValidateById(userId);

        user.Deactivate();

        await _repository.Update(user);
    }
}