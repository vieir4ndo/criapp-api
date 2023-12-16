using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CRIApp.Application.Abstractions;
using CRIApp.Application.Dto;
using CRIApp.Application.Dtos;
using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Exceptions;
using CRIApp.Domain.Extensions;
using CRIApp.Domain.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CRIApp.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly JwtTokenSettings _jwtTokenSettings;
    private readonly IMessageUser _messageUser;
    private readonly IUserResetPasswordCodeRepository _userResetPasswordCodeRepository;

    public UserService(IUserRepository repository,
        IOptions<JwtTokenSettings> jwtTokenSettings,
        IMessageUser messageUser,
        IUserResetPasswordCodeRepository userResetPasswordCodeRepository)
    {
        _repository = repository;
        _jwtTokenSettings = jwtTokenSettings.Value;
        _messageUser = messageUser;
        _userResetPasswordCodeRepository = userResetPasswordCodeRepository;
    }

    public async Task Create(CreateUserDto dto)
    {
        var userExisting = await _repository.GetByUsername(dto.Username);

        if (userExisting is not null)
            throw new ValidationException("This username is taken");

        var user = dto.ToDomainObject();
        await _repository.Create(user);
        await _messageUser.NotifyAccountCreation(user);
    }

    public async Task<TokenDto> Login(LoginDto dto)
    {
        var user = await GetByUsername(dto.Username);

        if (user is not null && user.VerifyPassword(dto.Password))
        {
            return GenerateToken(user);
        }

        throw new ValidationException("Username or password did not match");
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
            throw new NotFoundException("User not found");

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
            throw new ValidationException("User not found");

        //TODO: Extract this to settings
        var code = RandomExtensions.GenerateRandomString(5);
        
        await _userResetPasswordCodeRepository.Create(new UserResetPasswordCode(user.Id, code, DateTime.Now.AddSeconds(180).ToUniversalTime()));
        
        await _messageUser.NotifyForgotPassword(user, code);
    }

    public async Task ResetPassword(ResetPasswordDto dto)
    {
        var user = await GetByUsername(dto.Username);
        
        if (user is null)
            throw new ValidationException("User not found");

        if (!user.VerifyResetPasswordCode(dto.Code))
            throw new ValidationException("Code is invalid or expired");
        
        user.UpdatePassword(dto.NewPassword);
        
        await _repository.Update(user);
    }
}