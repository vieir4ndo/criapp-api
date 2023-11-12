using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;

namespace CRIApp.Repository.Repositories;

public class AnswerRepository : IAnswerRepository
{
    private readonly AppContext Context;

    public AnswerRepository(AppContext context)
    {
        Context = context;
    }
    
    public async Task Create(Answer answer)
    {
        await Context.Answer.AddAsync(answer);
        await Context.SaveChangesAsync();
    }
}