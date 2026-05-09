using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;

namespace CRIApp.Repository.Seeders;

public static class QuestionSeeder
{
    public static void Execute(AppContext context, Guid? adminId)
    {
        if (context.Question.Any() || adminId is null) return;
        
        var singleParentFamilyTypes = new List<FamilyType>()
        {
            FamilyType.FemaleSingleParent,
            FamilyType.MaleSingleParent,
        };
            
        context.Question.AddRange(
            new Question(
                "Le animamos a imitar gestos (por ejemplo, aplaudir al ritmo de la música, saludar, tirar besos,  etc.).",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "Jugamos a mover y acercar objetos ayudándonos, a su vez, de otro objeto y le animamos a que  nos imite (por ejemplo, acercar una pelota con un palo largo).",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "Jugamos con juguetes de encaje de piezas (por ejemplo, encajar los objetos de distintas formas en  su correspondiente espacio).",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "Le respondemos cuando fija la atención en algo (por ejemplo, seguimos con la mirada el objeto  que está observando o señalando, lo nombramos y le hablamos sobre ese mismo objeto).",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "Jugamos a imitar sonidos de distintos animales y objetos (por ejemplo, vaca, gato, coche, tren,  etc.).",
                adminId.Value, QuestionType.Agreement),
            new Question("Le leemos cuentos infantiles.", adminId.Value, QuestionType.Agreement),
            new Question(
                "Le respondemos a sus sonidos, gestos y movimientos (por ejemplo, cuando dice “ma-ma, ta-ta” o  hace algún otro sonido o gesto como aplaudir, le respondemos y/o lo repetimos).",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "Establecemos turnos de comunicación (ahora tú, ahora yo), dejando tiempo para que pueda  responder mediante sonidos y/o movimientos.",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "Cuando pronuncia mal una palabra, inmediatamente, la repetimos de forma correcta, lenta y clara  para que la diga otra vez.",
                adminId.Value, QuestionType.Agreement),
            new Question("Nombramos los objetos que señala y le enseñamos a pronunciar la palabra.", adminId.Value,
                QuestionType.Agreement),
            new Question("Intentamos reprimir y no expresar nuestras emociones. *", adminId.Value,
                QuestionType.Agreement, QuestionScoringType.Reversed),
            new Question("Le dejamos llorar para que aprenda a calmarse solo/sola. *", adminId.Value,
                QuestionType.Agreement, QuestionScoringType.Reversed),
            new Question(
                "Ponemos palabras a las emociones y hablamos sobre cómo nos sentimos (por ejemplo, “estás  enfadado porque tienes mucha hambre”, “estoy contenta porque vamos a dar un paseo juntos”,  etc.).",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "Le explicamos la razón por la que no le permitimos hacer algo (por ejemplo, decirle que se puede  hacer daño).",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "Cuando intenta hacer algo y no le sale (por ejemplo, alcanzar un juguete, sostener un sonajero,  tomar el biberón, etc.), lo hacemos nosotros para evitar que sufra o que se sienta mal.",
                adminId.Value, QuestionType.Agreement),
            new Question("La única manera de que nos haga caso es gritándole. *", adminId.Value, QuestionType.Agreement, QuestionScoringType.Reversed),
            new Question(
                "Mantenemos un horario regular en los hábitos de organización de la vida cotidiana (por ejemplo,  las horas de las comidas, baño, sueño, juego, etc.).",
                adminId.Value, QuestionType.Agreement),
            new Question("Le pegamos cuando hace algo peligroso o inadecuado. *", adminId.Value, QuestionType.Agreement, QuestionScoringType.Reversed),
            new Question("Cuando tiene una rabieta, nos mantenemos firmes y no cedemos a sus caprichos. ", adminId.Value,
                QuestionType.Agreement),
            new Question(
                "Cuando consigue hacer algo (por ejemplo, coger un objeto, aplaudir, caminar hasta un punto, etc.),  se lo reconocemos y hacemos saber mediante expresiones verbales o gestos de aprobación.",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "Somos capaces de esperar pacientemente a que haga las cosas por sí mismo/misma (por ejemplo,  comer solo/sola, coger un objeto, etc.), ofreciéndole pequeñas ayudas en caso de que las necesite,  aunque se tarde mucho más tiempo.",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "Le permitimos hacer cosas por sí mismo/misma (por ejemplo, recoger juguetes, quitarse los  calcetines, recoger ropa, comer solo/sola, etc.).",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "Podemos contar con la ayuda de la familia extensa (abuelos-as, hermanos-as, etc.) cuando lo  necesitamos.",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "Tenemos amigos y amigas con los que podemos contar en caso de necesitar ayuda con las tareas  de la crianza.",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "Consideramos que nuestro pediatra es una persona de confianza que nos atiende  adecuadamente.",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "Conocemos alguna persona profesional de confianza con la que podemos consultar dudas sobre  la crianza y la educación de nuestro hijo/hija.",
                adminId.Value, QuestionType.Agreement),
            new Question("Acudimos a las reuniones generales convocadas en la guardería o centro infantil.",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "Conocemos de manera detallada y tenemos información suficiente sobre lo que nuestro hijo/hija  hace en la guardería o centro infantil.",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "Me cuesta decidir qué hacer con mi hijo/hija cuando surge alguna dificultad (por ejemplo, cuando  le duele algo, cuando tiene una rabieta, cuando se pone a llorar, etc.) *",
                adminId.Value, QuestionType.Agreement, QuestionScoringType.Reversed),
            new Question("Siento que no puedo criar a mi hijo/hija como realmente me gustaría. *", adminId.Value,
                QuestionType.Agreement, QuestionScoringType.Reversed),
            new Question(
                "Me siento disgustada/disgustado e intranquila/intranquilo con la crianza de mi hijo/hija. *",
                adminId.Value, QuestionType.Agreement, QuestionScoringType.Reversed),
            new Question(
                "Siento que la obligación de tener que cuidar a mi hijo/hija me supone más tiempo y energía de la  que tengo. *",
                adminId.Value, QuestionType.Agreement, QuestionScoringType.Reversed),
            new Question("Siento que no consigo relajarme cuando estoy con mi hijo/hija.", adminId.Value,
                QuestionType.Agreement, QuestionScoringType.Reversed),
            new Question(
                "Tener un hijo/hija me ha supuesto una carga que me ha generado más preocupaciones de las que  esperaba.",
                adminId.Value, QuestionType.Agreement, QuestionScoringType.Reversed),
            new Question(
                "Compartimos de manera equilibrada las tareas de cuidado de nuestro hijo/hija (alimentación,  baño, juego, etc.).",
                adminId.Value, QuestionType.Agreement, excludedFamilyTypes: singleParentFamilyTypes),
            new Question(
                "Compartimos de manera equilibrada las tareas domésticas (limpiar, cocinar, hacer la compra,  etc.).",
                adminId.Value, QuestionType.Agreement, excludedFamilyTypes: singleParentFamilyTypes),
            new Question(
                "Ambos reconocemos el esfuerzo del otro en la realización de las tareas domésticas y de crianza.",
                adminId.Value, QuestionType.Agreement, excludedFamilyTypes: singleParentFamilyTypes),
            new Question("Nuestras discusiones a veces alcanzan los insultos y amenazas. *", adminId.Value,
                QuestionType.Agreement,  QuestionScoringType.Reversed, excludedFamilyTypes: singleParentFamilyTypes),
            new Question("El/la bebé ha presenciado una discusión en la que nos hemos insultado. *", adminId.Value,
                QuestionType.Agreement, QuestionScoringType.Reversed, excludedFamilyTypes: singleParentFamilyTypes),
            new Question(
                "El bebé tiene contacto con algún dispositivo electrónico (ej. teléfono, Tablet, TV) mientras las  personas adultas están haciendo otras tareas. *",
                adminId.Value, QuestionType.Agreement, QuestionScoringType.Reversed, excludedFamilyTypes: singleParentFamilyTypes),
            new Question(
                "El bebé tiene contacto con algún dispositivo electrónico (ej. teléfono, Tablet, TV) para conseguir  que haga otras tareas (ej. comer, esperar, dormir, cuando tiene berrinches, etc.) *",
                adminId.Value, QuestionType.Agreement, QuestionScoringType.Reversed, excludedFamilyTypes: singleParentFamilyTypes),
            new Question(
                "Hacemos un uso excesivo de dispositivos electrónicos ante la presencia de nuestro hijo/hija.",
                adminId.Value, QuestionType.Agreement,  QuestionScoringType.Reversed, excludedFamilyTypes: singleParentFamilyTypes),
            new Question(
                "Creemos que la utilización y exposición a los dispositivos perjudica el desarrollo de su bebé.",
                adminId.Value, QuestionType.Agreement, QuestionScoringType.Reversed, excludedFamilyTypes: singleParentFamilyTypes),
            new Question(
                "Le llevamos a eventos o espectáculos adecuados para su edad (teatro, cine, marionetas,etc.).",
                adminId.Value, QuestionType.Agreement, excludedFamilyTypes: singleParentFamilyTypes),
            new Question("Realizamos habitualmente algún viaje o excusión en familia.", adminId.Value,
                QuestionType.Agreement, excludedFamilyTypes: singleParentFamilyTypes)
        );
        context.SaveChanges();
    }
}