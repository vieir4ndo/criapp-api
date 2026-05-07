using System.Linq.Expressions;
using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRIApp.Repository.Repositories;

public class QuestionRepository : IQuestionRepository
{
    private readonly AppContext Context;

    public QuestionRepository(AppContext context)
    {
        Context = context;
    }
    
    public async Task<IEnumerable<Question>> GetAll()
    {
        return await Context.Question
            .ToListAsync();
    }

    public async Task<Question?> GetById(Guid id)
    {
        return await Context.Question.FirstOrDefaultAsync(x => x.Id == id);
    }
    
    public async Task<List<Question>> GetFiltered(
        Expression<Func<Question, bool>> predicate)
    {
        return await Context.Question
            .Where(predicate)
            .ToListAsync();
    }
}