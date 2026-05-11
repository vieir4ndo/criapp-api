using CRIApp.Application.Dtos;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;

namespace CRIApp.Application.Abstractions;

public interface IQuestionService
{
    Task<IEnumerable<QuestionDto>> GetQuestionsToReply(Guid userId, Guid childId);
    Task<Question?> GetAndValidateById(Guid id);
    Task<IEnumerable<Question>> GetByGroup(QuestionGroup group);
}