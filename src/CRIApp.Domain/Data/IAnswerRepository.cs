using CRIApp.Domain.Entities;

namespace CRIApp.Domain.Data;

public interface IAnswerRepository : IRepository<Answer>
{
    Task Create(Answer answer);
}