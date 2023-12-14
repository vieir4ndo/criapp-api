using CRIApp.Domain.Entities;

namespace CRIApp.Domain.Data;

public interface IUserResetPasswordCodeRepository : IRepository<UserResetPasswordCode>
{
    Task Create(UserResetPasswordCode code);
    Task<UserResetPasswordCode?> GetLastCodeByUserId(Guid userId);
}