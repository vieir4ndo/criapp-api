
using CRIApp.Domain.Entities;

namespace CRIApp.Repository.Seeders;

public static class AudiovisualResourceSeeder
{
    public static void Execute(AppContext context, Guid? adminId)
    {
        if (context.AudiovisualResource.Any() || adminId is null) return;
        
        context.AudiovisualResource.Add(
            new AudiovisualResource("Aquarela", "Canción de Toquinho", "https://www.youtube.com/watch?v=xT8HIiFQ8Y0", adminId.Value)
                .WithTranslations("Aquarela", "Song by Toquinho", "Aquarela", "Música de Toquinho")
        );
            
        context.SaveChanges();
    }
}