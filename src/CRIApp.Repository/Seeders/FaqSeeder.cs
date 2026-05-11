using CRIApp.Domain.Entities;

namespace CRIApp.Repository.Seeders;

public static class FaqSeeder
{
    public static void Execute(AppContext context, Guid? adminId)
    {
        if (context.Faq.Any() || adminId is null) return;

        var faqs = new List<Faq>
        {
            new Faq(
                "¿Qué es CRIApp?",
                "CRIApp es una aplicación diseñada para apoyar a familias y cuidadores en el seguimiento del desarrollo infantil temprano. Ofrece cuestionarios de evaluación, recomendaciones personalizadas, recursos audiovisuales y una comunidad de apoyo en los foros.",
                adminId.Value
            ).WithTranslations(
                "What is CRIApp?",
                "CRIApp is an application designed to support families and caregivers in monitoring early childhood development. It offers assessment questionnaires, personalized recommendations, audiovisual resources and a support community in the forums.",
                "O que é o CRIApp?",
                "O CRIApp é uma aplicação concebida para apoiar famílias e cuidadores no acompanhamento do desenvolvimento infantil precoce. Oferece questionários de avaliação, recomendações personalizadas, recursos audiovisuais e uma comunidade de apoio nos fóruns."
            ),

            new Faq(
                "¿Cómo funciona el cuestionario?",
                "El cuestionario utiliza una escala de Likert del 1 al 6, donde debes indicar con qué frecuencia ocurre cada comportamiento en tu hijo. Cuanto más completes los cuestionarios, más precisas serán las recomendaciones que recibirás.",
                adminId.Value
            ).WithTranslations(
                "How does the questionnaire work?",
                "The questionnaire uses a Likert scale from 1 to 6, where you indicate how often each behavior occurs in your child. The more questionnaires you complete, the more accurate the recommendations you will receive.",
                "Como funciona o questionário?",
                "O questionário utiliza uma escala de Likert de 1 a 6, onde deves indicar com que frequência cada comportamento ocorre no teu filho. Quanto mais questionários completares, mais precisas serão as recomendações que receberás."
            ),

            new Faq(
                "¿Qué significan las puntuaciones obtenidas?",
                "La puntuación refleja el porcentaje de respuestas positivas en los cuestionarios completados, en una escala de 0 a 100. Una puntuación más alta indica un mayor número de comportamientos de desarrollo positivos observados.",
                adminId.Value
            ).WithTranslations(
                "What do the scores mean?",
                "The score reflects the percentage of positive responses in the completed questionnaires, on a scale of 0 to 100. A higher score indicates a greater number of positive developmental behaviors observed.",
                "O que significam as pontuações obtidas?",
                "A pontuação reflete a percentagem de respostas positivas nos questionários completados, numa escala de 0 a 100. Uma pontuação mais alta indica um maior número de comportamentos de desenvolvimento positivos observados."
            ),

            new Faq(
                "¿Cómo se obtienen los premios?",
                "Los premios se otorgan automáticamente según tu actividad en la aplicación: responder 5 o más preguntas distintas, completar cuestionarios en días consecutivos, mejorar tu puntuación respecto al día anterior o alcanzar una puntuación superior a 80.",
                adminId.Value
            ).WithTranslations(
                "How are awards obtained?",
                "Awards are granted automatically based on your activity in the application: answering 5 or more different questions, completing questionnaires on consecutive days, improving your score compared to the previous day, or reaching a score above 80.",
                "Como se obtêm os prémios?",
                "Os prémios são atribuídos automaticamente com base na tua atividade na aplicação: responder a 5 ou mais perguntas distintas, completar questionários em dias consecutivos, melhorar a tua pontuação em relação ao dia anterior ou atingir uma pontuação superior a 80."
            ),

            new Faq(
                "¿Para qué sirven las recomendaciones?",
                "Las recomendaciones son sugerencias personalizadas elaboradas por especialistas, basadas en las respuestas que has dado en los cuestionarios. Su objetivo es orientarte con estrategias concretas para favorecer el desarrollo de tu hijo.",
                adminId.Value
            ).WithTranslations(
                "What are recommendations for?",
                "Recommendations are personalized suggestions developed by specialists, based on the answers you have given in the questionnaires. Their goal is to guide you with concrete strategies to support your child's development.",
                "Para que servem as recomendações?",
                "As recomendações são sugestões personalizadas elaboradas por especialistas, com base nas respostas que deste nos questionários. O seu objetivo é orientar-te com estratégias concretas para favorecer o desenvolvimento do teu filho."
            ),

            new Faq(
                "¿Puedo registrar más de un hijo?",
                "Sí, puedes añadir varios hijos en la sección 'Niños'. Cada perfil de niño es independiente, lo que te permite hacer un seguimiento individual del desarrollo de cada uno.",
                adminId.Value
            ).WithTranslations(
                "Can I register more than one child?",
                "Yes, you can add multiple children in the 'Children' section. Each child's profile is independent, allowing you to individually track each child's development.",
                "Posso registar mais do que um filho?",
                "Sim, podes adicionar vários filhos na secção 'Crianças'. Cada perfil de criança é independente, permitindo-te acompanhar individualmente o desenvolvimento de cada um."
            ),

            new Faq(
                "¿Qué son los recursos audiovisuales?",
                "Los recursos audiovisuales son vídeos seleccionados por especialistas sobre temas clave del desarrollo infantil temprano, como el aprendizaje del lenguaje, la neuroplasticidad y el impacto de las experiencias tempranas en el cerebro.",
                adminId.Value
            ).WithTranslations(
                "What are the audiovisual resources?",
                "The audiovisual resources are videos selected by specialists on key topics of early childhood development, such as language learning, neuroplasticity and the impact of early experiences on the brain.",
                "O que são os recursos audiovisuais?",
                "Os recursos audiovisuais são vídeos selecionados por especialistas sobre temas-chave do desenvolvimento infantil precoce, como a aprendizagem da linguagem, a neuroplasticidade e o impacto das experiências precoces no cérebro."
            ),

            new Faq(
                "¿Cómo restablezco mi contraseña?",
                "En la pantalla de inicio de sesión, pulsa '¿Olvidaste tu contraseña?' e introduce tu correo electrónico. Recibirás un código de verificación que deberás introducir para crear una nueva contraseña. El código es válido durante 10 minutos.",
                adminId.Value
            ).WithTranslations(
                "How do I reset my password?",
                "On the login screen, tap 'Forgot your password?' and enter your email address. You will receive a verification code that you must enter to create a new password. The code is valid for 10 minutes.",
                "Como redefino a minha palavra-passe?",
                "No ecrã de início de sessão, toca em 'Esqueceste a tua palavra-passe?' e introduz o teu endereço de e-mail. Receberás um código de verificação que deverás introduzir para criar uma nova palavra-passe. O código é válido durante 10 minutos."
            ),

            new Faq(
                "¿Con qué frecuencia debo responder los cuestionarios?",
                "Recomendamos responder los cuestionarios diariamente para obtener un seguimiento más preciso del desarrollo. La aplicación premia la constancia, por lo que responder en días consecutivos desbloquea logros adicionales.",
                adminId.Value
            ).WithTranslations(
                "How often should I answer the questionnaires?",
                "We recommend answering the questionnaires daily to get a more accurate tracking of development. The application rewards consistency, so answering on consecutive days unlocks additional achievements.",
                "Com que frequência devo responder aos questionários?",
                "Recomendamos responder aos questionários diariamente para obter um acompanhamento mais preciso do desenvolvimento. A aplicação premeia a constância, pelo que responder em dias consecutivos desbloqueia conquistas adicionais."
            ),

            new Faq(
                "¿Cómo puedo participar en los foros?",
                "Accede a la sección 'Fóruns' desde el menú principal. Allí puedes leer publicaciones de otros usuarios, crear nuevos temas y responder a los existentes. Es un espacio de apoyo mutuo entre familias y cuidadores.",
                adminId.Value
            ).WithTranslations(
                "How can I participate in the forums?",
                "Access the 'Forums' section from the main menu. There you can read posts from other users, create new topics and reply to existing ones. It is a space of mutual support between families and caregivers.",
                "Como posso participar nos fóruns?",
                "Acede à secção 'Fóruns' a partir do menu principal. Aí podes ler publicações de outros utilizadores, criar novos tópicos e responder aos existentes. É um espaço de apoio mútuo entre famílias e cuidadores."
            ),
        };

        context.Faq.AddRange(faqs);
        context.SaveChanges();
    }
}