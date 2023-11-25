using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Exceptions;
using CRIApp.Domain.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CRIApp.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly JwtTokenSettings _jwtTokenSettings;

    public UserService(IUserRepository repository,
        IOptions<JwtTokenSettings> jwtTokenSettings)
    {
        _repository = repository;
        _jwtTokenSettings = jwtTokenSettings.Value;
    }

    public async Task Create(CreateUserDto dto)
    {
        var userExisting = await _repository.GetByEmail(dto.Email);

        if (userExisting is not null)
            throw new ValidationException("This email is taken");

        var user = dto.ToDomainObject();
        await _repository.Create(user);
    }

    public async Task<TokenDto> Login(LoginDto dto)
    {
        var user = await GetByEmail(dto.Username);

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

    public async Task<User?> GetByEmail(string email)
    {
        return await _repository.GetByEmail(email);
    }

    public async Task<User?> GetAndValidateById(Guid id)
    {
        var user = await _repository.GetById(id);

        if (user is null)
            throw new NotFoundException("User not found");

        return user;
    }
}