using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using System.Reflection;

namespace CRIApp.Repository.Seeders;

public static class AwardSeeder
{
    public static void Execute(AppContext context)
    {
        if (context.Award.Any()) return;

        var awards = new List<Award>
        {
            new Award(AwardType.FirstSteps, "Primeros Pasos")
                .WithTranslations(
                    "First Steps", "Primeiros Passos",
                    "Completaste tu primer cuestionario. ¡El inicio de un gran camino junto a tus hijos!",
                    "You completed your first questionnaire. The beginning of a great journey with your children!",
                    "Você completou seu primeiro questionário. O início de uma grande jornada com seus filhos!")
                .WithImage(LoadImage("first_steps_award.png")),

            new Award(AwardType.EngagedFamily, "Familia Comprometida")
                .WithTranslations(
                    "Committed Family", "Família Comprometida",
                    "Tu implicación como familia demuestra que la crianza consciente marca la diferencia. ¡Sois un equipo increíble!",
                    "Your involvement as a family shows that conscious parenting makes a difference. You are an incredible team!",
                    "Seu envolvimento como família demonstra que a criação consciente faz a diferença. Vocês são uma equipe incrível!")
                .WithImage(LoadImage("engaged_family_award.png")),

            new Award(AwardType.Overcoming, "Superación")
                .WithTranslations(
                    "Overcoming", "Superação",
                    "Has demostrado que con esfuerzo y constancia se superan todos los retos. ¡Un gran logro en tu camino de crianza!",
                    "You have shown that with effort and perseverance all challenges can be overcome. A great achievement on your parenting journey!",
                    "Você demonstrou que com esforço e perseverança todos os desafios podem ser superados. Uma grande conquista na sua jornada de criação!")
                .WithImage(LoadImage("overcoming_award.png")),

            new Award(AwardType.PositiveEnvironment, "Entorno Positivo")
                .WithTranslations(
                    "Positive Environment", "Ambiente Positivo",
                    "Contribuyes activamente a crear un ambiente seguro y afectuoso para el desarrollo de tus hijos.",
                    "You actively contribute to creating a safe and loving environment for the development of your children.",
                    "Você contribui ativamente para criar um ambiente seguro e afetuoso para o desenvolvimento dos seus filhos.")
                .WithImage(LoadImage("positive_environment_award.png")),

            new Award(AwardType.SmallProgress, "Pequeño Gran Progreso")
                .WithTranslations(
                    "Little Big Progress", "Pequeno Grande Progresso",
                    "Cada pequeño avance cuenta. Tu perseverancia y dedicación son el motor del crecimiento de tu familia.",
                    "Every small step counts. Your perseverance and dedication are the driving force behind your family's growth.",
                    "Cada pequeno avanço conta. Sua perseverança e dedicação são o motor do crescimento da sua família.")
                .WithImage(LoadImage("small_progress_award.png")),
        };

        context.Award.AddRange(awards);
        context.SaveChanges();
    }

    private static byte[]? LoadImage(string fileName)
    {
        var resourceName = $"CRIApp.Repository.Seeders.Images.{fileName}";
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        if (stream is null) return null;
        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }
}
