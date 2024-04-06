using CRIApp.Domain.Entities;

namespace CRIApp.Repository.Seeders;

public class FaqSeeder
{
    public static void Execute(AppContext context, Guid? adminId)
    {
        if (!context.Faq.Any() && adminId is not null)
        {
            context.Faq.Add(
                new Faq("teste?", "teste", adminId.Value)
            );
            
            context.SaveChanges();
        }
    }
}