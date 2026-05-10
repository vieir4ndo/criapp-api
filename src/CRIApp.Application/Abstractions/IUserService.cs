using CRIApp.Application.Dtos;
using CRIApp.Domain.Entities;

namespace CRIApp.Application.Abstractions;

public interface IUserService
{ 
    Task Create(CreateUserDto dto);
    Task<TokenDto> Login(LoginDto dto);
    Task<User?> GetByUsername(string username);
    Task<User> GetAndValidateById(Guid id);
    Task Update(Guid userId, UpdateUserDto dto);
    Task ForgotPassword(string username);
    Task ResetPassword(ResetPasswordDto dto);
    Task UpdateScore(User user);
    Task<IList<User>> GetAllUsers();
    Task<IList<AwardDto>> GetAwards(Guid userId);
    Task<IList<AdviceDto>> GetAdvices(Guid userId);
    Task Deactivate(Guid userId);
}