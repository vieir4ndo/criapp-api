using CRIApp.Application.Dtos;
using CRIApp.Domain.Entities;

namespace CRIApp.Application.Abstractions;

public interface IUserService
{
    Task Create(CreateUserDto dto);
    Task<User?> GetByUsername(string username);
    Task<User> GetAndValidateById(Guid id);
    Task UpdatePassword(Guid userId, string newPassword);
    Task Update(Guid userId, UpdateUserDto dto);
    Task Deactivate(Guid userId);
}