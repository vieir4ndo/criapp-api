using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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

public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly IUserResetPasswordCodeRepository _resetPasswordCodeRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IMessageUser _messageUser;
    private readonly JwtTokenSettings _jwtTokenSettings;
    private readonly ILogger<AuthService> _logger;
    private readonly IStringLocalizer<AppMessages> _localizer;

    public AuthService(
        IUserService userService,
        IUserResetPasswordCodeRepository resetPasswordCodeRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IMessageUser messageUser,
        IOptions<JwtTokenSettings> jwtTokenSettings,
        ILogger<AuthService> logger,
        IStringLocalizer<AppMessages> localizer)
    {
        _userService = userService;
        _resetPasswordCodeRepository = resetPasswordCodeRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _messageUser = messageUser;
        _jwtTokenSettings = jwtTokenSettings.Value;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task<TokenDto> Login(LoginDto dto)
    {
        var user = await _userService.GetByUsername(dto.Username);

        if (user is not null && user.Active && user.VerifyPassword(dto.Password))
        {
            return await GenerateAndStoreToken(user);
        }

        _logger.LogWarning("Failed login attempt for username {Username}", dto.Username);
        throw new ValidationException(_localizer["InvalidCredentials"]);
    }

    public async Task<TokenDto> RefreshToken(string refreshToken)
    {
        var tokenHash = HashExtensions.HashPassword(refreshToken);
        var storedToken = await _refreshTokenRepository.GetByTokenHash(tokenHash);

        if (storedToken is null || storedToken.IsExpired())
        {
            _logger.LogWarning("Invalid or expired refresh token");
            throw new ValidationException(_localizer["InvalidRefreshToken"]);
        }

        if (!storedToken.User.Active)
        {
            _logger.LogWarning("Refresh token used for inactive user {UserId}", storedToken.User.Id);
            throw new ValidationException(_localizer["InvalidCredentials"]);
        }

        await _refreshTokenRepository.Delete(storedToken);
        return await GenerateAndStoreToken(storedToken.User);
    }

    public async Task ForgotPassword(string username)
    {
        var user = await _userService.GetByUsername(username);

        if (user is null)
        {
            _logger.LogWarning("Forgot password requested for unknown username {Username}", username);
            throw new ValidationException(_localizer["UserNotFound"]);
        }

        var code = RandomExtensions.GenerateRandomString(5);
        await _resetPasswordCodeRepository.Create(new UserResetPasswordCode(user.Id, code, DateTime.UtcNow.AddSeconds(180)));
        await _messageUser.NotifyForgotPassword(user, code);
    }

    public async Task ResetPassword(ResetPasswordDto dto)
    {
        var user = await _userService.GetByUsername(dto.Username);

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

        await _userService.UpdatePassword(user.Id, dto.NewPassword);
    }

    private async Task<TokenDto> GenerateAndStoreToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtTokenSettings.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var accessTokenExpiresIn = DateTime.UtcNow.AddMinutes(_jwtTokenSettings.ExpiresInMinutes);

        var claims = new List<Claim> { new("id", user.Id.ToString()) };

        var sectoken = new JwtSecurityToken(
            _jwtTokenSettings.Issuer,
            _jwtTokenSettings.Issuer,
            claims,
            expires: accessTokenExpiresIn,
            signingCredentials: credentials);

        var rawRefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var refreshTokenExpiresIn = DateTime.UtcNow.AddDays(_jwtTokenSettings.RefreshTokenExpiresInDays);
        await _refreshTokenRepository.Create(new RefreshToken(user.Id, rawRefreshToken, refreshTokenExpiresIn));

        return new TokenDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(sectoken),
            ExpiresIn = accessTokenExpiresIn,
            RefreshToken = rawRefreshToken,
            RefreshTokenExpiresIn = refreshTokenExpiresIn,
            User = new UserDto(user)
        };
    }
}

