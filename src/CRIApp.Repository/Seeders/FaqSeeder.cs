using CRIApp.Domain.Entities;

namespace CRIApp.Repository.Seeders;

public static class FaqSeeder
{
    public static void Execute(AppContext context, Guid? adminId)
    {
        if (context.Faq.Any() || adminId is null) return;
        
        context.Faq.Add(
            new Faq("teste?", "teste", adminId.Value)
        );
            
        context.SaveChanges();
    }
}