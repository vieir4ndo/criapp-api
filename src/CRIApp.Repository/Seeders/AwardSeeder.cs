using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;

namespace CRIApp.Repository.Seeders;

public static class AwardSeeder
{
    public static void Execute(AppContext context)
    {
        if (context.Award.Any()) return;

        var awards = new List<Award>
        {
            new Award(AwardType.HabitBuilder, "Constructor de Hábitos")
                .WithTranslations(
                    "Habit Builder", "Construtor de Hábitos",
                    "Has registrado una rutina al menos 4 días distintos en una semana. ¡Estás construyendo hábitos saludables para tu familia!",
                    "You registered a routine at least 4 distinct days in a week. You are building healthy habits for your family!",
                    "Você registrou uma rotina pelo menos 4 dias distintos em uma semana. Você está construindo hábitos saudáveis para sua família!"),

            new Award(AwardType.ConsistentParent, "Padre/Madre Constante")
                .WithTranslations(
                    "Consistent Parent", "Pai/Mãe Constante",
                    "Has registrado rutinas en 7 días distintos. Tu constancia es el pilar del desarrollo de tus hijos.",
                    "You have registered routines on 7 distinct days. Your consistency is the pillar of your children's development.",
                    "Você registrou rotinas em 7 dias distintos. Sua constância é o pilar do desenvolvimento dos seus filhos."),

            new Award(AwardType.SilverCognitiveDevelopment, "Medalla de Plata — Desarrollo Cognitivo")
                .WithTranslations(
                    "Silver Medal — Cognitive Development", "Medalha de Prata — Desenvolvimento Cognitivo",
                    "Has alcanzado el 100% en el área de andamiaje del desarrollo motor, cognitivo y lingüístico. ¡Un logro extraordinario!",
                    "You have reached 100% in the motor, cognitive and linguistic development scaffolding area. An extraordinary achievement!",
                    "Você atingiu 100% na área de andaime do desenvolvimento motor, cognitivo e linguístico. Uma conquista extraordinária!"),

            new Award(AwardType.SilverEmotionalBonding, "Medalla de Plata — Vínculo Emocional")
                .WithTranslations(
                    "Silver Medal — Emotional Bonding", "Medalha de Prata — Vínculo Emocional",
                    "Has alcanzado el 100% en el área de andamiaje del desarrollo socioemocional. Tu sensibilidad hacia tu hijo/a es admirable.",
                    "You have reached 100% in the socioemotional development scaffolding area. Your sensitivity towards your child is admirable.",
                    "Você atingiu 100% na área de andaime do desenvolvimento socioemocional. Sua sensibilidade em relação ao seu filho/a é admirável."),

            new Award(AwardType.SilverDailyRoutines, "Medalla de Plata — Rutinas Cotidianas")
                .WithTranslations(
                    "Silver Medal — Daily Routines", "Medalha de Prata — Rotinas Diárias",
                    "Has alcanzado el 100% en el perfil parental potenciador del desarrollo. Eres un referente en crianza positiva.",
                    "You have reached 100% in the development-enhancing parental profile area. You are a role model in positive parenting.",
                    "Você atingiu 100% no perfil parental potenciador do desenvolvimento. Você é um modelo de criação positiva."),

            new Award(AwardType.SilverSocialInteraction, "Medalla de Plata — Interacción Social")
                .WithTranslations(
                    "Silver Medal — Social Interaction", "Medalha de Prata — Interação Social",
                    "Has alcanzado el 100% en el área de organización del contexto social y entorno físico. ¡Brindas el mejor ambiente a tu familia!",
                    "You have reached 100% in the social context and physical environment organisation area. You provide the best environment for your family!",
                    "Você atingiu 100% na área de organização do contexto social e ambiente físico. Você proporciona o melhor ambiente para sua família!"),

            new Award(AwardType.GoldParent, "Medalla de Oro — Crianza Completa")
                .WithTranslations(
                    "Gold Medal — Complete Parenting", "Medalha de Ouro — Criação Completa",
                    "Has conquistado medallas de plata en todas las áreas HEFAS. Eres un ejemplo de crianza responsable y consciente. ¡Enhorabuena!",
                    "You have earned silver medals in all HEFAS areas. You are an example of responsible and conscious parenting. Congratulations!",
                    "Você conquistou medalhas de prata em todas as áreas HEFAS. Você é um exemplo de criação responsável e consciente. Parabéns!"),
        };

        context.Award.AddRange(awards);
        context.SaveChanges();
    }
}
