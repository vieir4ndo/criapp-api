using CRIApp.Domain.Entities;

namespace CRIApp.Domain.Data;

public interface IQuestionRepository : IRepository<Question>
{
    Task<IEnumerable<Question>> GetAll();
    Task<IEnumerable<Question>> GetAllExcept(List<Guid> blackList);
    Task<Question?> GetById(Guid id);
} 