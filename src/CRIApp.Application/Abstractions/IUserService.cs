using CRIApp.Application.Dtos;
using CRIApp.Domain.Entities;

namespace CRIApp.Application.Abstractions;

public interface IUserService
{ 
    Task Create(CreateUserDto dto);
    Task<TokenDto> Login(LoginDto dto);
    Task<User?> GetByEmail(string email);
    Task<User?> GetAndValidateById(Guid id);
}