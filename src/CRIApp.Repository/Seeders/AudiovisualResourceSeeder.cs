
using CRIApp.Domain.Entities;

namespace CRIApp.Repository.Seeders;

public static class AudiovisualResourceSeeder
{
    public static void Execute(AppContext context, Guid? adminId)
    {
        if (context.AudiovisualResource.Any() || adminId is null) return;

        var resources = new List<AudiovisualResource>
        {
            new AudiovisualResource(
                "El genio lingüístico de los bebés",
                "Patricia Kuhl explica cómo los bebés aprenden el lenguaje en los primeros meses de vida y la importancia de la interacción social en ese proceso.",
                "https://www.youtube.com/watch?v=G2XBIkHW954",
                adminId.Value
            ).WithTranslations(
                "The Linguistic Genius of Babies",
                "Patricia Kuhl explains how babies learn language in the first months of life and the importance of social interaction in that process.",
                "O gênio linguístico dos bebês",
                "Patricia Kuhl explica como os bebês aprendem a linguagem nos primeiros meses de vida e a importância da interação social nesse processo."
            ),

            new AudiovisualResource(
                "Serve y devuelve: construyendo el cerebro",
                "El Centro de Harvard explica cómo las interacciones cotidianas entre cuidadores y bebés fortalecen las conexiones cerebrales en la infancia temprana.",
                "https://www.youtube.com/watch?v=m_5u8-QSh6A",
                adminId.Value
            ).WithTranslations(
                "Serve and Return: Building the Brain",
                "Harvard's Center explains how everyday interactions between caregivers and babies strengthen brain connections in early childhood.",
                "Servir e devolver: construindo o cérebro",
                "O Centro de Harvard explica como as interações cotidianas entre cuidadores e bebês fortalecem as conexões cerebrais na primeira infância."
            ),

            new AudiovisualResource(
                "¿Cómo el trauma infantil afecta la salud?",
                "Nadine Burke Harris explica cómo las experiencias adversas en la infancia tienen un impacto profundo y duradero en la salud física y mental a lo largo de la vida.",
                "https://www.youtube.com/watch?v=95ovIJ3dsNk",
                adminId.Value
            ).WithTranslations(
                "How Childhood Trauma Affects Health Across a Lifetime",
                "Nadine Burke Harris explains how adverse childhood experiences have a profound and lasting impact on physical and mental health throughout life.",
                "Como o trauma infantil afeta a saúde ao longo da vida",
                "Nadine Burke Harris explica como as experiências adversas na infância têm um impacto profundo e duradouro na saúde física e mental ao longo da vida."
            ),

            new AudiovisualResource(
                "¿Las escuelas matan la creatividad?",
                "Sir Ken Robinson argumenta que los sistemas educativos actuales suprimen la creatividad de los niños, y propone un enfoque más humano y centrado en el talento individual.",
                "https://www.youtube.com/watch?v=iG9CE55wbtY",
                adminId.Value
            ).WithTranslations(
                "Do Schools Kill Creativity?",
                "Sir Ken Robinson argues that current education systems suppress children's creativity, and proposes a more human approach centered on individual talent.",
                "As escolas matam a criatividade?",
                "Sir Ken Robinson argumenta que os sistemas educacionais atuais suprimem a criatividade das crianças e propõe uma abordagem mais humana centrada no talento individual."
            ),

            new AudiovisualResource(
                "Arquitectura cerebral en la infancia",
                "El Centro de Desarrollo Infantil de Harvard explica cómo las experiencias tempranas construyen la arquitectura del cerebro, con consecuencias para toda la vida.",
                "https://www.youtube.com/watch?v=VNNsN9IJkws",
                adminId.Value
            ).WithTranslations(
                "Brain Architecture in Early Childhood",
                "Harvard's Center on the Developing Child explains how early experiences build the brain's architecture, with lifelong consequences.",
                "Arquitetura cerebral na infância",
                "O Centro de Harvard explica como as experiências precoces constroem a arquitetura do cérebro, com consequências para toda a vida."
            ),

            new AudiovisualResource(
                "Cómo el aprendizaje cambia tu cerebro",
                "La neurocientífica Lara Boyd explica cómo el aprendizaje modifica físicamente el cerebro y qué comportamientos favorecen la neuroplasticidad a cualquier edad.",
                "https://www.youtube.com/watch?v=LNHBMFCzznE",
                adminId.Value
            ).WithTranslations(
                "After Watching This, Your Brain Will Not Be the Same",
                "Neuroscientist Lara Boyd explains how learning physically changes the brain and what behaviours promote neuroplasticity at any age.",
                "Depois de assistir isso, seu cérebro não será o mesmo",
                "A neurocientista Lara Boyd explica como o aprendizado modifica fisicamente o cérebro e quais comportamentos favorecem a neuroplasticidade em qualquer idade."
            ),
        };

        context.AudiovisualResource.AddRange(resources);
        context.SaveChanges();
    }
}