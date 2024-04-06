using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using CRIApp.Domain.Data;

namespace CRIApp.Application.Services;

public class FaqService : IFaqService
{
    private readonly IFaqRepository _questionRepository;

    public FaqService(IFaqRepository questionRepository)
    {
        _questionRepository = questionRepository;
    }
    
    public async Task<IList<FaqDto>> GetAll()
    {
        var faqs = await _questionRepository.GetAll();

        return faqs.Select(faq => new FaqDto(faq)).ToList();
    }
}