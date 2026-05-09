using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;

namespace CRIApp.Repository.Seeders;

public static class QuestionRecommendationSeeder
{
    public static void Execute(AppContext context, Guid? adminId)
    {
        if (context.Question.Any() || adminId is null) return;

        var singleParentFamilyTypes = new List<FamilyType>
        {
            FamilyType.FemaleSingleParent,
            FamilyType.MaleSingleParent,
        };

        var questions = new List<Question>
        {
            new Question(
                "Le animamos a imitar gestos (por ejemplo, aplaudir al ritmo de la música, saludar, tirar besos, etc.).",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "Jugamos a mover y acercar objetos ayudándonos, a su vez, de otro objeto y le animamos a que nos imite (por ejemplo, acercar una pelota con un palo largo).",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "Jugamos con juguetes de encaje de piezas (por ejemplo, encajar los objetos de distintas formas en su correspondiente espacio).",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "Le respondemos cuando fija la atención en algo (por ejemplo, seguimos con la mirada el objeto que está observando o señalando, lo nombramos y le hablamos sobre ese mismo objeto).",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "Jugamos a imitar sonidos de distintos animales y objetos (por ejemplo, vaca, gato, coche, tren, etc.).",
                adminId.Value, QuestionType.Agreement),
            new Question("Le leemos cuentos infantiles.", adminId.Value, QuestionType.Agreement),
            new Question(
                "Le respondemos a sus sonidos, gestos y movimientos (por ejemplo, cuando dice \"ma-ma, ta-ta\" o hace algún otro sonido o gesto como aplaudir, le respondemos y/o lo repetimos).",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "Establecemos turnos de comunicación (ahora tú, ahora yo), dejando tiempo para que pueda responder mediante sonidos y/o movimientos.",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "Cuando pronuncia mal una palabra, inmediatamente, la repetimos de forma correcta, lenta y clara para que la diga otra vez.",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "Nombramos los objetos que señala y le enseñamos a pronunciar la palabra.",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "Intentamos reprimir y no expresar nuestras emociones. *",
                adminId.Value, QuestionType.Agreement, QuestionScoringType.Reversed),
            new Question(
                "Le dejamos llorar para que aprenda a calmarse solo/sola. *",
                adminId.Value, QuestionType.Agreement, QuestionScoringType.Reversed),
            new Question(
                "Ponemos palabras a las emociones y hablamos sobre cómo nos sentimos (por ejemplo, \"estás enfadado porque tienes mucha hambre\", \"estoy contenta porque vamos a dar un paseo juntos\", etc.).",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "Le explicamos la razón por la que no le permitimos hacer algo (por ejemplo, decirle que se puede hacer daño).",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "Cuando intenta hacer algo y no le sale (por ejemplo, alcanzar un juguete, sostener un sonajero, tomar el biberón, etc.), lo hacemos nosotros para evitar que sufra o que se sienta mal.",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "La única manera de que nos haga caso es gritándole. *",
                adminId.Value, QuestionType.Agreement, QuestionScoringType.Reversed),
            new Question(
                "Mantenemos un horario regular en los hábitos de organización de la vida cotidiana (por ejemplo, las horas de las comidas, baño, sueño, juego, etc.).",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "Le pegamos cuando hace algo peligroso o inadecuado. *",
                adminId.Value, QuestionType.Agreement, QuestionScoringType.Reversed),
            new Question(
                "Cuando tiene una rabieta, nos mantenemos firmes y no cedemos a sus caprichos.",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "Cuando consigue hacer algo (por ejemplo, coger un objeto, aplaudir, caminar hasta un punto, etc.), se lo reconocemos y hacemos saber mediante expresiones verbales o gestos de aprobación.",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "Somos capaces de esperar pacientemente a que haga las cosas por sí mismo/misma (por ejemplo, comer solo/sola, coger un objeto, etc.), ofreciéndole pequeñas ayudas en caso de que las necesite, aunque se tarde mucho más tiempo.",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "Le permitimos hacer cosas por sí mismo/misma (por ejemplo, recoger juguetes, quitarse los calcetines, recoger ropa, comer solo/sola, etc.).",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "Podemos contar con la ayuda de la familia extensa (abuelos-as, hermanos-as, etc.) cuando lo necesitamos.",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "Tenemos amigos y amigas con los que podemos contar en caso de necesitar ayuda con las tareas de la crianza.",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "Consideramos que nuestro pediatra es una persona de confianza que nos atiende adecuadamente.",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "Conocemos alguna persona profesional de confianza con la que podemos consultar dudas sobre la crianza y la educación de nuestro hijo/hija.",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "Acudimos a las reuniones generales convocadas en la guardería o centro infantil.",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "Conocemos de manera detallada y tenemos información suficiente sobre lo que nuestro hijo/hija hace en la guardería o centro infantil.",
                adminId.Value, QuestionType.Agreement),
            new Question(
                "Me cuesta decidir qué hacer con mi hijo/hija cuando surge alguna dificultad (por ejemplo, cuando le duele algo, cuando tiene una rabieta, cuando se pone a llorar, etc.) *",
                adminId.Value, QuestionType.Agreement, QuestionScoringType.Reversed),
            new Question(
                "Siento que no puedo criar a mi hijo/hija como realmente me gustaría. *",
                adminId.Value, QuestionType.Agreement, QuestionScoringType.Reversed),
            new Question(
                "Me siento disgustada/disgustado e intranquila/intranquilo con la crianza de mi hijo/hija. *",
                adminId.Value, QuestionType.Agreement, QuestionScoringType.Reversed),
            new Question(
                "Siento que la obligación de tener que cuidar a mi hijo/hija me supone más tiempo y energía de la que tengo. *",
                adminId.Value, QuestionType.Agreement, QuestionScoringType.Reversed),
            new Question(
                "Siento que no consigo relajarme cuando estoy con mi hijo/hija.",
                adminId.Value, QuestionType.Agreement, QuestionScoringType.Reversed),
            new Question(
                "Tener un hijo/hija me ha supuesto una carga que me ha generado más preocupaciones de las que esperaba.",
                adminId.Value, QuestionType.Agreement, QuestionScoringType.Reversed),
            new Question(
                "Compartimos de manera equilibrada las tareas de cuidado de nuestro hijo/hija (alimentación, baño, juego, etc.).",
                adminId.Value, QuestionType.Agreement, excludedFamilyTypes: singleParentFamilyTypes),
            new Question(
                "Compartimos de manera equilibrada las tareas domésticas (limpiar, cocinar, hacer la compra, etc.).",
                adminId.Value, QuestionType.Agreement, excludedFamilyTypes: singleParentFamilyTypes),
            new Question(
                "Ambos reconocemos el esfuerzo del otro en la realización de las tareas domésticas y de crianza.",
                adminId.Value, QuestionType.Agreement, excludedFamilyTypes: singleParentFamilyTypes),
            new Question(
                "Nuestras discusiones a veces alcanzan los insultos y amenazas. *",
                adminId.Value, QuestionType.Agreement, QuestionScoringType.Reversed,
                excludedFamilyTypes: singleParentFamilyTypes),
            new Question(
                "El/la bebé ha presenciado una discusión en la que nos hemos insultado. *",
                adminId.Value, QuestionType.Agreement, QuestionScoringType.Reversed,
                excludedFamilyTypes: singleParentFamilyTypes),
            new Question(
                "El bebé tiene contacto con algún dispositivo electrónico (ej. teléfono, Tablet, TV) mientras las personas adultas están haciendo otras tareas. *",
                adminId.Value, QuestionType.Agreement, QuestionScoringType.Reversed,
                excludedFamilyTypes: singleParentFamilyTypes),
            new Question(
                "El bebé tiene contacto con algún dispositivo electrónico (ej. teléfono, Tablet, TV) para conseguir que haga otras tareas (ej. comer, esperar, dormir, cuando tiene berrinches, etc.) *",
                adminId.Value, QuestionType.Agreement, QuestionScoringType.Reversed,
                excludedFamilyTypes: singleParentFamilyTypes),
            new Question(
                "Hacemos un uso excesivo de dispositivos electrónicos ante la presencia de nuestro hijo/hija.",
                adminId.Value, QuestionType.Agreement, QuestionScoringType.Reversed,
                excludedFamilyTypes: singleParentFamilyTypes),
            new Question(
                "Creemos que la utilización y exposición a los dispositivos perjudica el desarrollo de su bebé.",
                adminId.Value, QuestionType.Agreement, QuestionScoringType.Reversed,
                excludedFamilyTypes: singleParentFamilyTypes),
            new Question(
                "Le llevamos a eventos o espectáculos adecuados para su edad (teatro, cine, marionetas, etc.).",
                adminId.Value, QuestionType.Agreement, excludedFamilyTypes: singleParentFamilyTypes),
            new Question(
                "Realizamos habitualmente algún viaje o excursión en familia.",
                adminId.Value, QuestionType.Agreement, excludedFamilyTypes: singleParentFamilyTypes),
        };

        var recommendations = new List<Recommendation>
        {
            new Recommendation("Jugar a imitar gestos (ej. aplaudir al ritmo de la música, saludar, etc.).",
                adminId.Value, questions[0].Id),
            new Recommendation(
                "Acercar objetos sirviéndose, a su vez, de otro objeto animándole a que los imite (ej. acercar una pelota con un palo largo).",
                adminId.Value, questions[1].Id),
            new Recommendation(
                "Jugar con juguetes de encaje de piezas (ej. cajas en las que se deben encajar objetos de distinta forma). Además de mejorar sus habilidades motrices, esto le permitirá desarrollar su concentración y la capacidad de resolver problemas.",
                adminId.Value, questions[2].Id),
            new Recommendation(
                "Responderle cuando fija la atención en algo (ej. nombrar el objeto que está mirando, entregarle el juguete, hablarle del mismo, etc.).",
                adminId.Value, questions[3].Id),
            new Recommendation(
                "Jugar a imitar sonidos de animales y objetos (ej. cómo hace la vaca, el perro, cómo es el ruido de los coches, del tren, etc.).",
                adminId.Value, questions[4].Id),
            new Recommendation(
                "Leerle cuentos infantiles diariamente. Aunque parezca que no os entiendan, es una manera importante de estimular el lenguaje y el desarrollo de la atención. Podéis hablar sobre los personajes, mencionar los objetos que aparecen, etc.",
                adminId.Value, questions[5].Id),
            new Recommendation(
                "Responder a sus señales, vocalizaciones, movimientos, etc. (ej. cuando hace ma-ma, ta-ta, etc., responder con un sonido similar).",
                adminId.Value, questions[6].Id),
            new Recommendation(
                "Practicar turnos de comunicación, haciendo pausas para que el/la bebé pueda responder mediante sonidos y/o movimientos.",
                adminId.Value, questions[7].Id),
            new Recommendation(
                "Cuando empiece a hablar y pronuncie mal una palabra, repetirla de forma correcta y lenta, para que la diga nuevamente.",
                adminId.Value, questions[8].Id),
            new Recommendation("Nombrar los objetos que señala y enseñarle a pronunciar las palabras.", adminId.Value,
                questions[9].Id),
            new Recommendation(
                "Expresar las emociones de manera controlada, mostrando lo que pensáis, sentís y hacéis, y poniéndole nombre a las mismas.",
                adminId.Value, questions[10].Id),
            new Recommendation(
                "Calmarle cada vez que esté llorando. El/la bebé no es capaz de calmarse sin ayuda; las personas adultas son las encargadas de autorregular su comportamiento durante la primera etapa de vida.",
                adminId.Value, questions[11].Id),
            new Recommendation(
                "Poner palabras a las emociones y hablar sobre cómo os sentís (ej. estás enfadado/a porque tienes mucha hambre, estoy contenta porque vamos a dar un paseo juntos, etc.).",
                adminId.Value, questions[12].Id),
            new Recommendation(
                "Explicarle inmediatamente la razón de la prohibición cuando hace algo peligroso o inadecuado (ej. decirle que hace daño y duele).",
                adminId.Value, questions[13].Id),
            new Recommendation(
                "Cuando algo no le sale bien la primera vez, dejarle el tiempo que necesite para lograrlo (ej. coger un objeto, subir un escalón, encajar una pieza de un juguete, etc.). Es importante no hacer las cosas por él o ella, lo cual no le permitiría aprender nada nuevo, pero sí que es importante dar ayuda cuando la pide o la necesita.",
                adminId.Value, questions[14].Id),
            new Recommendation(
                "No gritarle para impedir o conseguir que haga algo. Recordar que sois modelos de comportamiento y si le manifestáis vuestro malestar gritando, posiblemente tienda a repetir ese comportamiento en otros contextos.",
                adminId.Value, questions[15].Id),
            new Recommendation(
                "Mantener un horario regular en los hábitos de organización de la vida cotidiana (ej. comidas, baño, sueño, juego, etc.). Esto le ayuda a crear un ambiente estructurado y predecible que genera un sentimiento de seguridad.",
                adminId.Value, questions[16].Id),
            new Recommendation(
                "No pegarle cuando hace algo peligroso o inadecuado. Pegar a tu hijo o hija ofrece un modelo de comportamiento violento para actuar con otras personas, afecta a la autoestima, genera sentimientos de tristeza, rabia, rencor y abandono, y bloquea su capacidad para resolver problemas y continuar su aprendizaje.",
                adminId.Value, questions[17].Id),
            new Recommendation("Manteneros firmes y no ceder a su(s) demanda(s) cuando coge una rabieta.",
                adminId.Value, questions[18].Id),
            new Recommendation(
                "Manifestarle cómo os sentís respecto a su buen comportamiento, lo cual reforzará su buena conducta (ej. lo has conseguido y estoy muy orgullosa/o de ti, se puede confiar en ti, que bien te has portado).",
                adminId.Value, questions[19].Id),
            new Recommendation(
                "Tener paciencia para no hacer las cosas en su lugar en aras de la rapidez. La única forma de aprender es \"haciendo\"; por lo tanto, no le prives de oportunidades de aprendizaje.",
                adminId.Value, questions[20].Id),
            new Recommendation(
                "Permitirle hacer cosas por sí mismo y asignarle alguna tarea (ej. permitirle comer por sí sólo/a, quitarse los calcetines, recoger juguetes, llevar el pan a la mesa, etc.). Esto le ayudará a ser más independiente, a tener más confianza en sí mismo/a, y reforzará su autoestima.",
                adminId.Value, questions[21].Id),
            new Recommendation(
                "Cultivar la relación positiva y de ayuda mutua con familiares: madres, padres, hermanos/as. Esto es saludable tanto para las personas adultas como para los niños y niñas.",
                adminId.Value, questions[22].Id),
            new Recommendation(
                "Cultivar relaciones de confianza y ayuda mutua en las tareas de crianza con amigos/as. La disponibilidad y la ayuda de otras personas es importante para ayudarnos a disminuir el estrés relacionado con la gestión de las tareas de crianza.",
                adminId.Value, questions[23].Id),
            new Recommendation(
                "Mantener una relación de confianza con vuestro o vuestra pediatra. Es importante que os sintáis seguros y acompañados para resolver cualquier duda relacionada con la crianza y el desarrollo infantil.",
                adminId.Value, questions[24].Id),
            new Recommendation(
                "Contactar con alguna persona profesional de confianza (ej. psicólogas, pedagogas, educadoras, etc.) que os oriente en las dudas sobre la crianza de vuestros hijo o hija.",
                adminId.Value, questions[25].Id),
            new Recommendation("Acudir habitualmente a las reuniones generales convocadas en la guardería.",
                adminId.Value, questions[26].Id),
            new Recommendation("Conocer con detalle lo que el/la bebé hace en la guardería.", adminId.Value,
                questions[27].Id),
            new Recommendation(
                "Consultar a personas expertas que os puedan orientar sobre vuestras dudas en relación a la crianza: educadoras, psicólogas, pedagogas, pediatras, etc.",
                adminId.Value, questions[28].Id),
            new Recommendation(
                "Establecer rutinas en la vida diaria para que os aporten un sentimiento de seguridad y sirvan de referencia a vuestro hijo/hija.",
                adminId.Value, questions[29].Id),
            new Recommendation("Compartir las preocupaciones con la pareja, con amigos/as o personas de confianza.",
                adminId.Value, questions[30].Id),
            new Recommendation(
                "Asistir a grupos de apoyo para la crianza. El contacto con otros padres y madres y profesionales os permitirá compartir vuestras experiencias, frustraciones y preocupaciones. A la vez os ayudará a conocer otras estrategias para afrontar situaciones diarias.",
                adminId.Value, questions[31].Id),
            new Recommendation(
                "Dedicaros tiempo a vosotras y vosotros, buscando momentos diarios para disfrutar y desconectar de la rutina. Si os sentís tranquilos/as y relajados/as, también podréis disfrutar del tiempo con vuestro hijo o hija.",
                adminId.Value, questions[32].Id),
            new Recommendation("Evitar que las situaciones de estrés alteren las rutinas diarias de vuestro hijo/hija.",
                adminId.Value, questions[33].Id),
            new Recommendation(
                "Siempre que sea posible, asistir ambos o por turnos a las consultas médicas de vuestros hijos e hijas.",
                adminId.Value, questions[34].Id),
            new Recommendation(
                "Compartir de manera equilibrada las tareas domésticas y las tareas de cuidado diarias: juego, alimentación y baño. Esto mejorará vuestra relación de pareja, os ayudará a reducir el estrés y, por lo tanto, mejorará vuestro bienestar y el de vuestro/a hijo o hija.",
                adminId.Value, questions[35].Id),
            new Recommendation(
                "Reconocer el esfuerzo del otro en la realización de las tareas domésticas y de crianza.",
                adminId.Value, questions[36].Id),
            new Recommendation(
                "Evitar que vuestras discusiones alcancen las faltas de respeto. Expresar los sentimientos, sin culpar ni herir a nadie y razonando lo que se dice.",
                adminId.Value, questions[37].Id),
            new Recommendation(
                "Evitar, por encima de todo, discutir delante del/la bebé. Buscar espacios y momentos para hablar de los problemas, en los que el/la bebé no esté presente.",
                adminId.Value, questions[38].Id),
            new Recommendation(
                "Evitar la exposición a pantallas hasta los 24 meses. La Organización Mundial de la Salud (OMS) recomienda que no debe existir la exposición a pantallas (ej.: TV, Tablet, Móviles, etc.) entre los 0 y 2 años.",
                adminId.Value, questions[39].Id),
            new Recommendation(
                "Evitar el uso de pantallas para conseguir que nos haga caso y/o esté tranquilo/a. Mediante los juegos de interacción, cantándole y hablándole, también podremos captar su atención y desarrollar otras habilidades. Utilizar las pantallas como un distractor les crea dependencia y les impide desarrollar la tolerancia a la frustración.",
                adminId.Value, questions[40].Id),
            new Recommendation(
                "Hacer un uso responsable de dispositivos electrónicos ante la presencia de vuestro hijo/hija. Recordar que sois modelos de aprendizaje, todo lo que hagáis vuestro hijo o hija tenderá a repetirlo.",
                adminId.Value, questions[41].Id),
            new Recommendation(
                "Recordar que los estudios muestran que no existen efectos positivos de la exposición a pantallas entre los 0 y 24 meses para el desarrollo de vuestro hijo o hija.",
                adminId.Value, questions[42].Id),
            new Recommendation(
                "Llevar a vuestra hija/o a eventos o espectáculos dirigidos a su edad (Teatro, cine, marionetas, parques, etc.).",
                adminId.Value, questions[43].Id),
            new Recommendation("Realizar habitualmente algún viaje o excursión en familia.", adminId.Value,
                questions[44].Id),
        };

        context.Question.AddRange(questions);
        context.Recommendation.AddRange(recommendations);
        context.SaveChanges();
    }
}