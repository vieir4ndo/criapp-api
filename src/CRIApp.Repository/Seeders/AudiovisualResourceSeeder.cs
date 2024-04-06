
using CRIApp.Domain.Entities;

namespace CRIApp.Repository.Seeders;

public class AudiovisualResourceSeeder
{
    public static void Execute(AppContext context, Guid? adminId)
    {
        if (!context.AudiovisualResource.Any() && adminId is not null)
        {
            context.AudiovisualResource.Add(
                new AudiovisualResource("Aquarela", "Canción de Toquinho", "https://www.youtube.com/watch?v=xT8HIiFQ8Y0", adminId.Value)
            );
            
            context.SaveChanges();
        }
    }
}