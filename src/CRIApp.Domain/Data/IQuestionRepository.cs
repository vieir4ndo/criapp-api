using System.Linq.Expressions;
using CRIApp.Domain.Entities;

namespace CRIApp.Domain.Data;

public interface IQuestionRepository : IRepository<Question>
{
    Task<IEnumerable<Question>> GetAll();
    Task<Question?> GetById(Guid id);
    Task<List<Question>> GetFiltered(Expression<Func<Question, bool>> predicate);
} 