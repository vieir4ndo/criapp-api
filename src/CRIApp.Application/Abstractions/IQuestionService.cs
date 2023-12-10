using CRIApp.Application.Dtos;
using CRIApp.Domain.Entities;

namespace CRIApp.Application.Abstractions;

public interface IQuestionService
{
    Task<IEnumerable<QuestionDto>> GetAllQuestionsToReply(Guid userId);
    Task<Question?> GetAndValidateById(Guid id);
}