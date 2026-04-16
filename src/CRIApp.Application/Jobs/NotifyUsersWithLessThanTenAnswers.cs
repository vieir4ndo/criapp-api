using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace CRIApp.Application.Jobs;

[ExcludeFromCodeCoverage]
public class NotifyUsersWithLessThanTenAnswers
{
    public NotifyUsersWithLessThanTenAnswers()
    {
        
    }

    public void Run()
    {
        Console.Write("Entrou no job amem");
    }
}