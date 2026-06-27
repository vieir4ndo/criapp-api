using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;

namespace CRIApp.Repository.Seeders;

public static class QuestionRecommendationSeeder
{
    public static void Execute(AppContext context)
    {
        if (context.Question.Any()) return;

        var singleParentFamilyTypes = new List<FamilyType>
        {
            FamilyType.FemaleSingleParent,
            FamilyType.MaleSingleParent,
        };

        var questions = new List<Question>
        {
            // Q0–Q9: CognitiveDevelopment
            new Question(
                "Le animamos a imitar gestos (por ejemplo, aplaudir al ritmo de la música, saludar, tirar besos, etc.).",
                 QuestionType.Agreement, group: QuestionGroup.CognitiveDevelopment)
                .WithTranslations(
                    "We encourage them to imitate gestures (e.g., clapping to music, waving, blowing kisses, etc.).",
                    "Incentivamos a imitar gestos (por exemplo, bater palmas ao ritmo da música, acenar, mandar beijos, etc.)."),
            new Question(
                "Jugamos a mover y acercar objetos ayudándonos, a su vez, de otro objeto y le animamos a que nos imite (por ejemplo, acercar una pelota con un palo largo).",
                 QuestionType.Agreement, group: QuestionGroup.CognitiveDevelopment)
                .WithTranslations(
                    "We play at moving and bringing objects closer using another object, and we encourage them to imitate us (e.g., pulling a ball with a long stick).",
                    "Brincamos de mover e aproximar objetos com a ajuda de outro objeto e incentivamos a imitar (por exemplo, puxar uma bola com um pau comprido)."),
            new Question(
                "Jugamos con juguetes de encaje de piezas (por ejemplo, encajar los objetos de distintas formas en su correspondiente espacio).",
                 QuestionType.Agreement, group: QuestionGroup.CognitiveDevelopment)
                .WithTranslations(
                    "We play with shape-sorting toys (e.g., fitting objects of different shapes into the corresponding space).",
                    "Brincamos com brinquedos de encaixe (por exemplo, encaixar objetos de diferentes formas no espaço correspondente)."),
            new Question(
                "Le respondemos cuando fija la atención en algo (por ejemplo, seguimos con la mirada el objeto que está observando o señalando, lo nombramos y le hablamos sobre ese mismo objeto).",
                 QuestionType.Agreement, group: QuestionGroup.CognitiveDevelopment)
                .WithTranslations(
                    "We respond when they focus their attention on something (e.g., we follow the object they are looking at or pointing to, name it, and talk about it).",
                    "Respondemos quando fixa a atenção em algo (por exemplo, seguimos o objeto que está olhando ou apontando, o nomeamos e falamos sobre ele)."),
            new Question(
                "Jugamos a imitar sonidos de distintos animales y objetos (por ejemplo, vaca, gato, coche, tren, etc.).",
                 QuestionType.Agreement, group: QuestionGroup.CognitiveDevelopment)
                .WithTranslations(
                    "We play at imitating sounds of different animals and objects (e.g., cow, cat, car, train, etc.).",
                    "Brincamos de imitar sons de diferentes animais e objetos (por exemplo, vaca, gato, carro, trem, etc.)."),
            new Question("Le leemos cuentos infantiles.",  QuestionType.Agreement,
                group: QuestionGroup.CognitiveDevelopment)
                .WithTranslations(
                    "We read children's stories to them.",
                    "Lemos histórias infantis para eles."),
            new Question(
                "Le respondemos a sus sonidos, gestos y movimientos (por ejemplo, cuando dice \"ma-ma, ta-ta\" o hace algún otro sonido o gesto como aplaudir, le respondemos y/o lo repetimos).",
                 QuestionType.Agreement, group: QuestionGroup.CognitiveDevelopment)
                .WithTranslations(
                    "We respond to their sounds, gestures, and movements (e.g., when they say \"ma-ma, ta-ta\" or make another sound or gesture like clapping, we respond and/or repeat it).",
                    "Respondemos aos sons, gestos e movimentos (por exemplo, quando diz \"ma-ma, ta-ta\" ou faz outro som ou gesto como bater palmas, respondemos e/ou repetimos)."),
            new Question(
                "Establecemos turnos de comunicación (ahora tú, ahora yo), dejando tiempo para que pueda responder mediante sonidos y/o movimientos.",
                 QuestionType.Agreement, group: QuestionGroup.CognitiveDevelopment)
                .WithTranslations(
                    "We establish communication turns (now you, now me), leaving time for them to respond through sounds and/or movements.",
                    "Estabelecemos turnos de comunicação (agora você, agora eu), deixando tempo para responder por meio de sons e/ou movimentos."),
            new Question(
                "Cuando pronuncia mal una palabra, inmediatamente, la repetimos de forma correcta, lenta y clara para que la diga otra vez.",
                 QuestionType.Agreement, group: QuestionGroup.CognitiveDevelopment)
                .WithTranslations(
                    "When they mispronounce a word, we immediately repeat it correctly, slowly, and clearly so they say it again.",
                    "Quando pronuncia mal uma palavra, a repetimos imediatamente de forma correta, lenta e clara para que diga novamente."),
            new Question(
                "Nombramos los objetos que señala y le enseñamos a pronunciar la palabra.",
                 QuestionType.Agreement, group: QuestionGroup.CognitiveDevelopment)
                .WithTranslations(
                    "We name the objects they point to and teach them to pronounce the word.",
                    "Nomeamos os objetos que aponta e ensinamos a pronunciar a palavra."),
            // Q10–Q21: EmotionalBonding
            new Question(
                "Intentamos reprimir y no expresar nuestras emociones. *",
                 QuestionType.Agreement, QuestionScoringType.Reversed,
                group: QuestionGroup.EmotionalBonding)
                .WithTranslations(
                    "We try to suppress and not express our emotions. *",
                    "Tentamos reprimir e não expressar nossas emoções. *"),
            new Question(
                "Le dejamos llorar para que aprenda a calmarse solo/sola. *",
                 QuestionType.Agreement, QuestionScoringType.Reversed,
                group: QuestionGroup.EmotionalBonding)
                .WithTranslations(
                    "We let them cry so they learn to calm down on their own. *",
                    "Deixamos chorar para que aprenda a se acalmar sozinho/sozinha. *"),
            new Question(
                "Ponemos palabras a las emociones y hablamos sobre cómo nos sentimos (por ejemplo, \"estás enfadado porque tienes mucha hambre\", \"estoy contenta porque vamos a dar un paseo juntos\", etc.).",
                 QuestionType.Agreement, group: QuestionGroup.EmotionalBonding)
                .WithTranslations(
                    "We put words to emotions and talk about how we feel (e.g., \"you're upset because you're very hungry\", \"I'm happy because we're going for a walk together\", etc.).",
                    "Colocamos palavras nas emoções e falamos sobre como nos sentimos (por exemplo, \"você está chateado porque está com muita fome\", \"estou feliz porque vamos dar um passeio juntos\", etc.)."),
            new Question(
                "Le explicamos la razón por la que no le permitimos hacer algo (por ejemplo, decirle que se puede hacer daño).",
                 QuestionType.Agreement, group: QuestionGroup.EmotionalBonding)
                .WithTranslations(
                    "We explain the reason why we don't allow them to do something (e.g., telling them they could get hurt).",
                    "Explicamos a razão pela qual não permitimos fazer algo (por exemplo, dizer que pode se machucar)."),
            new Question(
                "Cuando intenta hacer algo y no le sale (por ejemplo, alcanzar un juguete, sostener un sonajero, tomar el biberón, etc.), lo hacemos nosotros para evitar que sufra o que se sienta mal.",
                 QuestionType.Agreement, group: QuestionGroup.EmotionalBonding)
                .WithTranslations(
                    "When they try to do something and fail (e.g., reach a toy, hold a rattle, take a bottle, etc.), we do it ourselves to prevent them from suffering or feeling bad.",
                    "Quando tenta fazer algo e não consegue (por exemplo, alcançar um brinquedo, segurar um chocalho, tomar mamadeira, etc.), fazemos por ele/ela para evitar sofrimento."),
            new Question(
                "La única manera de que nos haga caso es gritándole. *",
                 QuestionType.Agreement, QuestionScoringType.Reversed,
                group: QuestionGroup.EmotionalBonding)
                .WithTranslations(
                    "The only way to make them listen is to shout at them. *",
                    "A única maneira de fazer com que nos ouça é gritando. *"),
            new Question(
                "Mantenemos un horario regular en los hábitos de organización de la vida cotidiana (por ejemplo, las horas de las comidas, baño, sueño, juego, etc.).",
                 QuestionType.Agreement, group: QuestionGroup.EmotionalBonding)
                .WithTranslations(
                    "We maintain a regular schedule in daily life habits (e.g., mealtimes, bath, sleep, play, etc.).",
                    "Mantemos um horário regular nos hábitos da vida cotidiana (por exemplo, horários das refeições, banho, sono, brincadeiras, etc.)."),
            new Question(
                "Le pegamos cuando hace algo peligroso o inadecuado. *",
                 QuestionType.Agreement, QuestionScoringType.Reversed,
                group: QuestionGroup.EmotionalBonding)
                .WithTranslations(
                    "We hit them when they do something dangerous or inappropriate. *",
                    "Batemos quando faz algo perigoso ou inadequado. *"),
            new Question(
                "Cuando tiene una rabieta, nos mantenemos firmes y no cedemos a sus caprichos.",
                 QuestionType.Agreement, group: QuestionGroup.EmotionalBonding)
                .WithTranslations(
                    "When they have a tantrum, we remain firm and do not give in to their demands.",
                    "Quando tem uma birra, ficamos firmes e não cedemos às suas exigências."),
            new Question(
                "Cuando consigue hacer algo (por ejemplo, coger un objeto, aplaudir, caminar hasta un punto, etc.), se lo reconocemos y hacemos saber mediante expresiones verbales o gestos de aprobación.",
                 QuestionType.Agreement, group: QuestionGroup.EmotionalBonding)
                .WithTranslations(
                    "When they achieve something (e.g., grabbing an object, clapping, walking to a point, etc.), we acknowledge it and let them know through verbal expressions or gestures of approval.",
                    "Quando consegue fazer algo (por exemplo, pegar um objeto, bater palmas, caminhar até um ponto, etc.), reconhecemos e demonstramos por expressões verbais ou gestos de aprovação."),
            new Question(
                "Somos capaces de esperar pacientemente a que haga las cosas por sí mismo/misma (por ejemplo, comer solo/sola, coger un objeto, etc.), ofreciéndole pequeñas ayudas en caso de que las necesite, aunque se tarde mucho más tiempo.",
                 QuestionType.Agreement, group: QuestionGroup.EmotionalBonding)
                .WithTranslations(
                    "We are able to patiently wait for them to do things on their own (e.g., eat alone, pick up an object, etc.), offering small help if needed, even if it takes much longer.",
                    "Somos capazes de esperar pacientemente para que faça as coisas sozinho/sozinha (por exemplo, comer sozinho/sozinha, pegar um objeto, etc.), oferecendo pequenas ajudas se necessário."),
            new Question(
                "Le permitimos hacer cosas por sí mismo/misma (por ejemplo, recoger juguetes, quitarse los calcetines, recoger ropa, comer solo/sola, etc.).",
                 QuestionType.Agreement, group: QuestionGroup.EmotionalBonding)
                .WithTranslations(
                    "We allow them to do things on their own (e.g., pick up toys, take off their socks, put away clothes, eat alone, etc.).",
                    "Permitimos fazer coisas sozinho/sozinha (por exemplo, guardar brinquedos, tirar meias, dobrar roupas, comer sozinho/sozinha, etc.)."),
            // Q22–Q27: DailyRoutines
            new Question(
                "Podemos contar con la ayuda de la familia extensa (abuelos-as, hermanos-as, etc.) cuando lo necesitamos.",
                 QuestionType.Agreement, group: QuestionGroup.DailyRoutines)
                .WithTranslations(
                    "We can count on the help of extended family (grandparents, siblings, etc.) when we need it.",
                    "Podemos contar com a ajuda da família extensa (avós, irmãos/irmãs, etc.) quando precisamos."),
            new Question(
                "Tenemos amigos y amigas con los que podemos contar en caso de necesitar ayuda con las tareas de la crianza.",
                 QuestionType.Agreement, group: QuestionGroup.DailyRoutines)
                .WithTranslations(
                    "We have friends we can count on when we need help with parenting tasks.",
                    "Temos amigos/amigas com quem podemos contar quando precisamos de ajuda com as tarefas de criação."),
            new Question(
                "Consideramos que nuestro pediatra es una persona de confianza que nos atiende adecuadamente.",
                 QuestionType.Agreement, group: QuestionGroup.DailyRoutines)
                .WithTranslations(
                    "We consider our pediatrician to be a trusted person who attends to us properly.",
                    "Consideramos que nosso pediatra é uma pessoa de confiança que nos atende adequadamente."),
            new Question(
                "Conocemos alguna persona profesional de confianza con la que podemos consultar dudas sobre la crianza y la educación de nuestro hijo/hija.",
                 QuestionType.Agreement, group: QuestionGroup.DailyRoutines)
                .WithTranslations(
                    "We know a trusted professional we can consult with about parenting and education doubts.",
                    "Conhecemos algum profissional de confiança com quem podemos tirar dúvidas sobre criação e educação."),
            new Question(
                "Acudimos a las reuniones generales convocadas en la guardería o centro infantil.",
                 QuestionType.Agreement, group: QuestionGroup.DailyRoutines)
                .WithTranslations(
                    "We attend general meetings called at the nursery or childcare centre.",
                    "Participamos das reuniões gerais convocadas na creche ou centro infantil."),
            new Question(
                "Conocemos de manera detallada y tenemos información suficiente sobre lo que nuestro hijo/hija hace en la guardería o centro infantil.",
                 QuestionType.Agreement, group: QuestionGroup.DailyRoutines)
                .WithTranslations(
                    "We know in detail and have sufficient information about what our child does at the nursery or childcare centre.",
                    "Conhecemos detalhadamente e temos informações suficientes sobre o que nosso filho/filha faz na creche ou centro infantil."),
            // Q28–Q44: SocialInteraction
            new Question(
                "Me cuesta decidir qué hacer con mi hijo/hija cuando surge alguna dificultad (por ejemplo, cuando le duele algo, cuando tiene una rabieta, cuando se pone a llorar, etc.) *",
                 QuestionType.Agreement, QuestionScoringType.Reversed,
                group: QuestionGroup.SocialInteraction)
                .WithTranslations(
                    "I find it hard to decide what to do with my child when a difficulty arises (e.g., when something hurts, when they have a tantrum, when they start crying, etc.) *",
                    "Tenho dificuldade em decidir o que fazer com meu filho/filha quando surge alguma dificuldade (por exemplo, quando algo dói, quando tem uma birra, quando começa a chorar, etc.) *"),
            new Question(
                "Siento que no puedo criar a mi hijo/hija como realmente me gustaría. *",
                 QuestionType.Agreement, QuestionScoringType.Reversed,
                group: QuestionGroup.SocialInteraction)
                .WithTranslations(
                    "I feel I cannot raise my child the way I would really like to. *",
                    "Sinto que não consigo criar meu filho/filha como realmente gostaria. *"),
            new Question(
                "Me siento disgustada/disgustado e intranquila/intranquilo con la crianza de mi hijo/hija. *",
                 QuestionType.Agreement, QuestionScoringType.Reversed,
                group: QuestionGroup.SocialInteraction)
                .WithTranslations(
                    "I feel upset and anxious about raising my child. *",
                    "Sinto-me perturbado(a) e ansioso(a) com a criação do meu filho/filha. *"),
            new Question(
                "Siento que la obligación de tener que cuidar a mi hijo/hija me supone más tiempo y energía de la que tengo. *",
                 QuestionType.Agreement, QuestionScoringType.Reversed,
                group: QuestionGroup.SocialInteraction)
                .WithTranslations(
                    "I feel that the obligation of having to care for my child takes more time and energy than I have. *",
                    "Sinto que a obrigação de cuidar do meu filho/filha me exige mais tempo e energia do que tenho. *"),
            new Question(
                "Siento que no consigo relajarme cuando estoy con mi hijo/hija.",
                 QuestionType.Agreement, QuestionScoringType.Reversed,
                group: QuestionGroup.SocialInteraction)
                .WithTranslations(
                    "I feel I cannot relax when I am with my child.",
                    "Sinto que não consigo relaxar quando estou com meu filho/filha."),
            new Question(
                "Tener un hijo/hija me ha supuesto una carga que me ha generado más preocupaciones de las que esperaba.",
                 QuestionType.Agreement, QuestionScoringType.Reversed,
                group: QuestionGroup.SocialInteraction)
                .WithTranslations(
                    "Having a child has been a burden that has caused me more worries than I expected.",
                    "Ter um filho/filha foi um fardo que me gerou mais preocupações do que eu esperava."),
            new Question(
                "Compartimos de manera equilibrada las tareas de cuidado de nuestro hijo/hija (alimentación, baño, juego, etc.).",
                 QuestionType.Agreement, excludedFamilyTypes: singleParentFamilyTypes,
                group: QuestionGroup.SocialInteraction)
                .WithTranslations(
                    "We share the childcare tasks equally (feeding, bathing, playing, etc.).",
                    "Compartilhamos de forma equilibrada as tarefas de cuidado do nosso filho/filha (alimentação, banho, brincadeiras, etc.)."),
            new Question(
                "Compartimos de manera equilibrada las tareas domésticas (limpiar, cocinar, hacer la compra, etc.).",
                 QuestionType.Agreement, excludedFamilyTypes: singleParentFamilyTypes,
                group: QuestionGroup.SocialInteraction)
                .WithTranslations(
                    "We share household tasks equally (cleaning, cooking, shopping, etc.).",
                    "Compartilhamos de forma equilibrada as tarefas domésticas (limpar, cozinhar, fazer compras, etc.)."),
            new Question(
                "Ambos reconocemos el esfuerzo del otro en la realización de las tareas domésticas y de crianza.",
                 QuestionType.Agreement, excludedFamilyTypes: singleParentFamilyTypes,
                group: QuestionGroup.SocialInteraction)
                .WithTranslations(
                    "We both recognise each other's effort in carrying out household and parenting tasks.",
                    "Ambos reconhecemos o esforço do outro na realização das tarefas domésticas e de criação."),
            new Question(
                "Nuestras discusiones a veces alcanzan los insultos y amenazas. *",
                 QuestionType.Agreement, QuestionScoringType.Reversed,
                excludedFamilyTypes: singleParentFamilyTypes, group: QuestionGroup.SocialInteraction)
                .WithTranslations(
                    "Our arguments sometimes reach insults and threats. *",
                    "Nossas discussões às vezes chegam a insultos e ameaças. *"),
            new Question(
                "El/la bebé ha presenciado una discusión en la que nos hemos insultado. *",
                 QuestionType.Agreement, QuestionScoringType.Reversed,
                excludedFamilyTypes: singleParentFamilyTypes, group: QuestionGroup.SocialInteraction)
                .WithTranslations(
                    "The baby has witnessed an argument in which we insulted each other. *",
                    "O bebê presenciou uma discussão em que nos insultamos. *"),
            new Question(
                "El bebé tiene contacto con algún dispositivo electrónico (ej. teléfono, Tablet, TV) mientras las personas adultas están haciendo otras tareas. *",
                 QuestionType.Agreement, QuestionScoringType.Reversed,
                excludedFamilyTypes: singleParentFamilyTypes, group: QuestionGroup.SocialInteraction)
                .WithTranslations(
                    "The baby has contact with an electronic device (e.g., phone, tablet, TV) while adults are doing other tasks. *",
                    "O bebê tem contato com algum dispositivo eletrônico (ex. celular, tablet, TV) enquanto os adultos estão fazendo outras tarefas. *"),
            new Question(
                "El bebé tiene contacto con algún dispositivo electrónico (ej. teléfono, Tablet, TV) para conseguir que haga otras tareas (ej. comer, esperar, dormir, cuando tiene berrinches, etc.) *",
                 QuestionType.Agreement, QuestionScoringType.Reversed,
                excludedFamilyTypes: singleParentFamilyTypes, group: QuestionGroup.SocialInteraction)
                .WithTranslations(
                    "The baby has contact with an electronic device (e.g., phone, tablet, TV) to get them to do other things (e.g., eat, wait, sleep, when they have tantrums, etc.) *",
                    "O bebê tem contato com algum dispositivo eletrônico (ex. celular, tablet, TV) para conseguir que faça outras tarefas (ex. comer, esperar, dormir, quando tem birras, etc.) *"),
            new Question(
                "Hacemos un uso excesivo de dispositivos electrónicos ante la presencia de nuestro hijo/hija.",
                 QuestionType.Agreement, QuestionScoringType.Reversed,
                excludedFamilyTypes: singleParentFamilyTypes, group: QuestionGroup.SocialInteraction)
                .WithTranslations(
                    "We make excessive use of electronic devices in the presence of our child.",
                    "Fazemos uso excessivo de dispositivos eletrônicos na presença do nosso filho/filha."),
            new Question(
                "Creemos que la utilización y exposición a los dispositivos perjudica el desarrollo de su bebé.",
                 QuestionType.Agreement, QuestionScoringType.Reversed,
                excludedFamilyTypes: singleParentFamilyTypes, group: QuestionGroup.SocialInteraction)
                .WithTranslations(
                    "We believe that the use and exposure to devices harms the development of our baby.",
                    "Acreditamos que o uso e a exposição a dispositivos prejudica o desenvolvimento do nosso bebê."),
            new Question(
                "Le llevamos a eventos o espectáculos adecuados para su edad (teatro, cine, marionetas, etc.).",
                 QuestionType.Agreement, excludedFamilyTypes: singleParentFamilyTypes,
                group: QuestionGroup.SocialInteraction)
                .WithTranslations(
                    "We take them to events or shows appropriate for their age (theatre, cinema, puppets, etc.).",
                    "Levamos a eventos ou espetáculos adequados para sua idade (teatro, cinema, fantoches, etc.)."),
            new Question(
                "Realizamos habitualmente algún viaje o excursión en familia.",
                 QuestionType.Agreement, excludedFamilyTypes: singleParentFamilyTypes,
                group: QuestionGroup.SocialInteraction)
                .WithTranslations(
                    "We regularly go on family trips or outings.",
                    "Realizamos habitualmente alguma viagem ou passeio em família."),
        };

        var recommendations = new List<Recommendation>        {
            new Recommendation("Jugar a imitar gestos (ej. aplaudir al ritmo de la música, saludar, etc.).",
                 questions[0].Id)
                .WithTranslations(
                    "Play at imitating gestures (e.g., clapping to music, waving, etc.).",
                    "Brincar de imitar gestos (ex. bater palmas ao ritmo da música, acenar, etc.)."),
            new Recommendation(
                "Acercar objetos sirviéndose, a su vez, de otro objeto animándole a que los imite (ej. acercar una pelota con un palo largo).",
                 questions[1].Id)
                .WithTranslations(
                    "Bring objects closer using another object, encouraging them to imitate (e.g., pulling a ball with a long stick).",
                    "Aproximar objetos com a ajuda de outro objeto, incentivando a imitar (ex. puxar uma bola com um pau comprido)."),
            new Recommendation(
                "Jugar con juguetes de encaje de piezas (ej. cajas en las que se deben encajar objetos de distinta forma). Además de mejorar sus habilidades motrices, esto le permitirá desarrollar su concentración y la capacidad de resolver problemas.",
                 questions[2].Id)
                .WithTranslations(
                    "Play with shape-sorting toys (e.g., boxes where objects of different shapes must be fitted). Besides improving motor skills, this develops concentration and problem-solving ability.",
                    "Brincar com brinquedos de encaixe (ex. caixas onde objetos de diferentes formas devem ser encaixados). Além de melhorar as habilidades motoras, isso desenvolve concentração e capacidade de resolver problemas."),
            new Recommendation(
                "Responderle cuando fija la atención en algo (ej. nombrar el objeto que está mirando, entregarle el juguete, hablarle del mismo, etc.).",
                 questions[3].Id)
                .WithTranslations(
                    "Respond when they focus on something (e.g., name the object they are looking at, hand them the toy, talk about it, etc.).",
                    "Responder quando fixa a atenção em algo (ex. nomear o objeto que está olhando, entregar o brinquedo, falar sobre ele, etc.)."),
            new Recommendation(
                "Jugar a imitar sonidos de animales y objetos (ej. cómo hace la vaca, el perro, cómo es el ruido de los coches, del tren, etc.).",
                 questions[4].Id)
                .WithTranslations(
                    "Play at imitating animal and object sounds (e.g., how the cow moos, the dog barks, car sounds, train, etc.).",
                    "Brincar de imitar sons de animais e objetos (ex. como a vaca faz, o cachorro late, como é o barulho dos carros, do trem, etc.)."),
            new Recommendation(
                "Leerle cuentos infantiles diariamente. Aunque parezca que no os entiendan, es una manera importante de estimular el lenguaje y el desarrollo de la atención. Podéis hablar sobre los personajes, mencionar los objetos que aparecen, etc.",
                 questions[5].Id)
                .WithTranslations(
                    "Read children's stories daily. Even if it seems they do not understand, it is an important way to stimulate language and attention development. You can talk about characters, mention objects that appear, etc.",
                    "Ler histórias infantis diariamente. Mesmo que pareça que não entendem, é uma forma importante de estimular a linguagem e o desenvolvimento da atenção. Você pode falar sobre os personagens, mencionar objetos que aparecem, etc."),
            new Recommendation(
                "Responder a sus señales, vocalizaciones, movimientos, etc. (ej. cuando hace ma-ma, ta-ta, etc., responder con un sonido similar).",
                 questions[6].Id)
                .WithTranslations(
                    "Respond to their signals, vocalisations, movements, etc. (e.g., when they say ma-ma, ta-ta, etc., respond with a similar sound).",
                    "Responder aos seus sinais, vocalizações, movimentos, etc. (ex. quando diz ma-ma, ta-ta, etc., responder com um som semelhante)."),
            new Recommendation(
                "Practicar turnos de comunicación, haciendo pausas para que el/la bebé pueda responder mediante sonidos y/o movimientos.",
                 questions[7].Id)
                .WithTranslations(
                    "Practise communication turns, pausing so the baby can respond through sounds and/or movements.",
                    "Praticar turnos de comunicação, fazendo pausas para que o bebê possa responder por meio de sons e/ou movimentos."),
            new Recommendation(
                "Cuando empiece a hablar y pronuncie mal una palabra, repetirla de forma correcta y lenta, para que la diga nuevamente.",
                 questions[8].Id)
                .WithTranslations(
                    "When they start talking and mispronounce a word, repeat it correctly and slowly so they say it again.",
                    "Quando começar a falar e pronunciar mal uma palavra, repeti-la corretamente e devagar para que diga novamente."),
            new Recommendation("Nombrar los objetos que señala y enseñarle a pronunciar las palabras.", 
                questions[9].Id)
                .WithTranslations(
                    "Name the objects they point to and teach them to pronounce the words.",
                    "Nomear os objetos que aponta e ensinar a pronunciar as palavras."),
            new Recommendation(
                "Expresar las emociones de manera controlada, mostrando lo que pensáis, sentís y hacéis, y poniéndole nombre a las mismas.",
                 questions[10].Id)
                .WithTranslations(
                    "Express emotions in a controlled way, showing what you think, feel, and do, and naming them.",
                    "Expressar as emoções de forma controlada, mostrando o que você pensa, sente e faz, e dando nome a elas."),
            new Recommendation(
                "Calmarle cada vez que esté llorando. El/la bebé no es capaz de calmarse sin ayuda; las personas adultas son las encargadas de autorregular su comportamiento durante la primera etapa de vida.",
                 questions[11].Id)
                .WithTranslations(
                    "Comfort them every time they cry. The baby cannot calm down without help; adults are responsible for regulating their behaviour during the first stage of life.",
                    "Acalmá-lo/la toda vez que estiver chorando. O bebê não consegue se acalmar sem ajuda; os adultos são responsáveis por regular seu comportamento durante a primeira fase de vida."),
            new Recommendation(
                "Poner palabras a las emociones y hablar sobre cómo os sentís (ej. estás enfadado/a porque tienes mucha hambre, estoy contenta porque vamos a dar un paseo juntos, etc.).",
                 questions[12].Id)
                .WithTranslations(
                    "Put words to emotions and talk about how you feel (e.g., you are upset because you are very hungry, I am happy because we are going for a walk together, etc.).",
                    "Colocar palavras nas emoções e falar sobre como vocês se sentem (ex. você está chateado/a porque está com muita fome, estou feliz porque vamos dar um passeio juntos, etc.)."),
            new Recommendation(
                "Explicarle inmediatamente la razón de la prohibición cuando hace algo peligroso o inadecuado (ej. decirle que hace daño y duele).",
                 questions[13].Id)
                .WithTranslations(
                    "Immediately explain the reason for the prohibition when they do something dangerous or inappropriate (e.g., tell them it hurts).",
                    "Explicar imediatamente o motivo da proibição quando faz algo perigoso ou inadequado (ex. dizer que dói e machuca)."),
            new Recommendation(
                "Cuando algo no le sale bien la primera vez, dejarle el tiempo que necesite para lograrlo (ej. coger un objeto, subir un escalón, encajar una pieza de un juguete, etc.). Es importante no hacer las cosas por él o ella, lo cual no le permitiría aprender nada nuevo, pero sí que es importante dar ayuda cuando la pide o la necesita.",
                 questions[14].Id)
                .WithTranslations(
                    "When something does not work out the first time, give them all the time they need to achieve it. It is important not to do things for them, but it is important to help when they ask or need it.",
                    "Quando algo não dá certo da primeira vez, dar todo o tempo necessário para conseguir. É importante não fazer as coisas por ele/ela, mas é importante ajudar quando pede ou precisa."),
            new Recommendation(
                "No gritarle para impedir o conseguir que haga algo. Recordar que sois modelos de comportamiento y si le manifestáis vuestro malestar gritando, posiblemente tienda a repetir ese comportamiento en otros contextos.",
                 questions[15].Id)
                .WithTranslations(
                    "Do not shout to stop or get them to do something. Remember that you are role models.",
                    "Não gritar para impedir ou fazer com que faça algo. Lembrar que vocês são modelos de comportamento."),
            new Recommendation(
                "Mantener un horario regular en los hábitos de organización de la vida cotidiana (ej. comidas, baño, sueño, juego, etc.). Esto le ayuda a crear un ambiente estructurado y predecible que genera un sentimiento de seguridad.",
                 questions[16].Id)
                .WithTranslations(
                    "Maintain a regular schedule in daily life habits (e.g., meals, bath, sleep, play, etc.). This helps create a structured and predictable environment that fosters a sense of security.",
                    "Manter um horário regular nos hábitos da vida cotidiana (ex. refeições, banho, sono, brincadeiras, etc.). Isso ajuda a criar um ambiente estruturado e previsível que gera um sentimento de segurança."),
            new Recommendation(
                "No pegarle cuando hace algo peligroso o inadecuado. Pegar a tu hijo o hija ofrece un modelo de comportamiento violento para actuar con otras personas, afecta a la autoestima, genera sentimientos de tristeza, rabia, rencor y abandono, y bloquea su capacidad para resolver problemas y continuar su aprendizaje.",
                 questions[17].Id)
                .WithTranslations(
                    "Do not hit them when they do something dangerous or inappropriate. Hitting your child provides a model of violent behaviour, affects self-esteem, and blocks their ability to solve problems.",
                    "Não bater quando faz algo perigoso ou inadequado. Bater no seu filho/filha oferece um modelo de comportamento violento, afeta a autoestima e bloqueia a capacidade de resolver problemas."),
            new Recommendation("Manteneros firmes y no ceder a su(s) demanda(s) cuando coge una rabieta.",
                 questions[18].Id)
                .WithTranslations(
                    "Stay firm and do not give in to their demand(s) when they have a tantrum.",
                    "Ficar firme e não ceder às exigências quando tem uma birra."),
            new Recommendation(
                "Manifestarle cómo os sentís respecto a su buen comportamiento, lo cual reforzará su buena conducta (ej. lo has conseguido y estoy muy orgullosa/o de ti, se puede confiar en ti, que bien te has portado).",
                 questions[19].Id)
                .WithTranslations(
                    "Express how you feel about their good behaviour, which will reinforce it.",
                    "Expressar como se sentem em relação ao bom comportamento, o que reforçará a boa conduta."),
            new Recommendation(
                "Tener paciencia para no hacer las cosas en su lugar en aras de la rapidez. La única forma de aprender es \"haciendo\"; por lo tanto, no le prives de oportunidades de aprendizaje.",
                 questions[20].Id)
                .WithTranslations(
                    "Be patient and do not do things in their place for the sake of speed. The only way to learn is by doing.",
                    "Ter paciência e não fazer as coisas no lugar dele/dela em nome da rapidez. A única forma de aprender é fazendo."),
            new Recommendation(
                "Permitirle hacer cosas por sí mismo y asignarle alguna tarea (ej. permitirle comer por sí sólo/a, quitarse los calcetines, recoger juguetes, llevar el pan a la mesa, etc.). Esto le ayudará a ser más independiente, a tener más confianza en sí mismo/a, y reforzará su autoestima.",
                 questions[21].Id)
                .WithTranslations(
                    "Allow them to do things on their own and assign them a task. This will help them be more independent and strengthen their self-esteem.",
                    "Permitir fazer coisas sozinho/sozinha e atribuir alguma tarefa. Isso ajudará a ser mais independente e reforçará a autoestima."),
            new Recommendation(
                "Cultivar la relación positiva y de ayuda mutua con familiares: madres, padres, hermanos/as. Esto es saludable tanto para las personas adultas como para los niños y niñas.",
                 questions[22].Id)
                .WithTranslations(
                    "Cultivate a positive and mutually supportive relationship with family members: mothers, fathers, brothers/sisters.",
                    "Cultivar a relação positiva e de ajuda mútua com familiares: mães, pais, irmãos/irmãs."),
            new Recommendation(
                "Cultivar relaciones de confianza y ayuda mutua en las tareas de crianza con amigos/as. La disponibilidad y la ayuda de otras personas es importante para ayudarnos a disminuir el estrés relacionado con la gestión de las tareas de crianza.",
                 questions[23].Id)
                .WithTranslations(
                    "Cultivate trusting and mutually supportive relationships in parenting tasks with friends.",
                    "Cultivar relações de confiança e ajuda mútua nas tarefas de criação com amigos/amigas."),
            new Recommendation(
                "Mantener una relación de confianza con vuestro o vuestra pediatra. Es importante que os sintáis seguros y acompañados para resolver cualquier duda relacionada con la crianza y el desarrollo infantil.",
                 questions[24].Id)
                .WithTranslations(
                    "Maintain a trusting relationship with your paediatrician.",
                    "Manter uma relação de confiança com o/a pediatra."),
            new Recommendation(
                "Contactar con alguna persona profesional de confianza (ej. psicólogas, pedagogas, educadoras, etc.) que os oriente en las dudas sobre la crianza de vuestros hijo o hija.",
                 questions[25].Id)
                .WithTranslations(
                    "Contact a trusted professional who can guide you on doubts about raising your child.",
                    "Entrar em contato com algum profissional de confiança que oriente nas dúvidas sobre a criação do filho/filha."),
            new Recommendation("Acudir habitualmente a las reuniones generales convocadas en la guardería.",
                 questions[26].Id)
                .WithTranslations(
                    "Regularly attend general meetings called at the nursery.",
                    "Participar habitualmente das reuniões gerais convocadas na creche."),
            new Recommendation("Conocer con detalle lo que el/la bebé hace en la guardería.", 
                questions[27].Id)
                .WithTranslations(
                    "Know in detail what the baby does at the nursery.",
                    "Conhecer detalhadamente o que o/a bebê faz na creche."),
            new Recommendation(
                "Consultar a personas expertas que os puedan orientar sobre vuestras dudas en relación a la crianza: educadoras, psicólogas, pedagogas, pediatras, etc.",
                 questions[28].Id)
                .WithTranslations(
                    "Consult experts who can guide you on your parenting doubts.",
                    "Consultar especialistas que possam orientar sobre dúvidas de criação."),
            new Recommendation(
                "Establecer rutinas en la vida diaria para que os aporten un sentimiento de seguridad y sirvan de referencia a vuestro hijo/hija.",
                 questions[29].Id)
                .WithTranslations(
                    "Establish daily routines to give you a sense of security and serve as a reference point for your child.",
                    "Estabelecer rotinas na vida diária para que ofereçam um sentimento de segurança."),
            new Recommendation("Compartir las preocupaciones con la pareja, con amigos/as o personas de confianza.",
                 questions[30].Id)
                .WithTranslations(
                    "Share worries with your partner, friends, or trusted people.",
                    "Compartilhar as preocupações com o/a parceiro/a, amigos/amigas ou pessoas de confiança."),
            new Recommendation(
                "Asistir a grupos de apoyo para la crianza. El contacto con otros padres y madres y profesionales os permitirá compartir vuestras experiencias, frustraciones y preocupaciones. A la vez os ayudará a conocer otras estrategias para afrontar situaciones diarias.",
                 questions[31].Id)
                .WithTranslations(
                    "Attend parenting support groups. Contact with other parents will allow you to share experiences and discover other strategies.",
                    "Participar de grupos de apoio à criação. O contato com outros pais e mães permitirá compartilhar experiências e conhecer outras estratégias."),
            new Recommendation(
                "Dedicaros tiempo a vosotras y vosotros, buscando momentos diarios para disfrutar y desconectar de la rutina. Si os sentís tranquilos/as y relajados/as, también podréis disfrutar del tiempo con vuestro hijo o hija.",
                 questions[32].Id)
                .WithTranslations(
                    "Make time for yourselves, finding daily moments to enjoy and disconnect from routine.",
                    "Dedicar tempo a si mesmos/mesmas, buscando momentos diários para aproveitar e desconectar da rotina."),
            new Recommendation("Evitar que las situaciones de estrés alteren las rutinas diarias de vuestro hijo/hija.",
                 questions[33].Id)
                .WithTranslations(
                    "Avoid stressful situations disrupting your child's daily routines.",
                    "Evitar que situações de estresse alterem as rotinas diárias do filho/filha."),
            new Recommendation(
                "Siempre que sea posible, asistir ambos o por turnos a las consultas médicas de vuestros hijos e hijas.",
                 questions[34].Id)
                .WithTranslations(
                    "Whenever possible, both attend or take turns attending your child's medical appointments.",
                    "Sempre que possível, ambos participarem ou se revezarem nas consultas médicas dos filhos/filhas."),
            new Recommendation(
                "Compartir de manera equilibrada las tareas domésticas y las tareas de cuidado diarias: juego, alimentación y baño. Esto mejorará vuestra relación de pareja, os ayudará a reducir el estrés y, por lo tanto, mejorará vuestro bienestar y el de vuestro/a hijo o hija.",
                 questions[35].Id)
                .WithTranslations(
                    "Share household and daily care tasks equally: play, feeding, and bathing.",
                    "Compartilhar de forma equilibrada as tarefas domésticas e as tarefas de cuidado diárias: brincadeiras, alimentação e banho."),
            new Recommendation(
                "Reconocer el esfuerzo del otro en la realización de las tareas domésticas y de crianza.",
                 questions[36].Id)
                .WithTranslations(
                    "Acknowledge each other's effort in carrying out household and parenting tasks.",
                    "Reconhecer o esforço do outro na realização das tarefas domésticas e de criação."),
            new Recommendation(
                "Evitar que vuestras discusiones alcancen las faltas de respeto. Expresar los sentimientos, sin culpar ni herir a nadie y razonando lo que se dice.",
                 questions[37].Id)
                .WithTranslations(
                    "Avoid your arguments turning into disrespect. Express feelings without blaming or hurting anyone.",
                    "Evitar que as discussões cheguem a faltas de respeito. Expressar os sentimentos sem culpar nem machucar ninguém."),
            new Recommendation(
                "Evitar, por encima de todo, discutir delante del/la bebé. Buscar espacios y momentos para hablar de los problemas, en los que el/la bebé no esté presente.",
                 questions[38].Id)
                .WithTranslations(
                    "Above all, avoid arguing in front of the baby. Find spaces and moments to discuss problems when the baby is not present.",
                    "Evitar acima de tudo discutir na frente do/da bebê. Buscar espaços e momentos para falar dos problemas quando o/a bebê não esteja presente."),
            new Recommendation(
                "Evitar la exposición a pantallas hasta los 24 meses. La Organización Mundial de la Salud (OMS) recomienda que no debe existir la exposición a pantallas (ej.: TV, Tablet, Móviles, etc.) entre los 0 y 2 años.",
                 questions[39].Id)
                .WithTranslations(
                    "Avoid screen exposure up to 24 months. The WHO recommends no screen exposure between 0 and 2 years.",
                    "Evitar a exposição a telas até os 24 meses. A OMS recomenda que não deve existir exposição a telas entre 0 e 2 anos."),
            new Recommendation(
                "Evitar el uso de pantallas para conseguir que nos haga caso y/o esté tranquilo/a. Mediante los juegos de interacción, cantándole y hablándole, también podremos captar su atención y desarrollar otras habilidades. Utilizar las pantallas como un distractor les crea dependencia y les impide desarrollar la tolerancia a la frustración.",
                 questions[40].Id)
                .WithTranslations(
                    "Avoid using screens to get them to behave. Through interactive games you can also capture their attention.",
                    "Evitar o uso de telas para conseguir que nos ouça. Por meio de jogos de interação também podemos captar sua atenção."),
            new Recommendation(
                "Hacer un uso responsable de dispositivos electrónicos ante la presencia de vuestro hijo/hija. Recordar que sois modelos de aprendizaje, todo lo que hagáis vuestro hijo o hija tenderá a repetirlo.",
                 questions[41].Id)
                .WithTranslations(
                    "Make responsible use of electronic devices in your child's presence. Remember that you are learning models.",
                    "Fazer uso responsável de dispositivos eletrônicos na presença do filho/filha. Lembrar que são modelos de aprendizado."),
            new Recommendation(
                "Recordar que los estudios muestran que no existen efectos positivos de la exposición a pantallas entre los 0 y 24 meses para el desarrollo de vuestro hijo o hija.",
                 questions[42].Id)
                .WithTranslations(
                    "Remember that studies show there are no positive effects of screen exposure between 0 and 24 months.",
                    "Lembrar que os estudos mostram que não há efeitos positivos da exposição a telas entre 0 e 24 meses."),
            new Recommendation(
                "Llevar a vuestra hija/o a eventos o espectáculos dirigidos a su edad (Teatro, cine, marionetas, parques, etc.).",
                 questions[43].Id)
                .WithTranslations(
                    "Take your child to events or shows aimed at their age (theatre, cinema, puppets, parks, etc.).",
                    "Levar a eventos ou espetáculos voltados para sua idade (teatro, cinema, fantoches, parques, etc.)."),
            new Recommendation("Realizar habitualmente algún viaje o excursión en familia.", 
                questions[44].Id)
                .WithTranslations(
                    "Regularly go on family trips or outings.",
                    "Realizar habitualmente alguma viagem ou passeio em família."),
        };

        context.Question.AddRange(questions);
        context.Recommendation.AddRange(recommendations);
        context.SaveChanges();
    }
}