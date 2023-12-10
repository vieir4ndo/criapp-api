using CRIApp.Application.Dtos;
using CRIApp.Domain.Entities;

namespace CRIApp.Application.Abstractions;

public interface IQuestionService
{
    Task<IEnumerable<QuestionDto>> GetQuestionsToReply(Guid userId, Guid childId);
    Task<Question?> GetAndValidateById(Guid id);
}