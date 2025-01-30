using System.Text;

namespace ExampleLlama
{
    internal class Program
    {
        private static ChatbotLlama _chatbotLlamaType2;
        private static ChatbotLlama _chatbotLlamaType1;

        private const string ModelPath1 = "C:/Users/Codexzier/.lmstudio/models/lmstudio-community/Phi-3.1-mini-128k-instruct-GGUF/Phi-3.1-mini-128k-instruct-Q4_K_M.gguf";
        private const string ModelPath= "C:/Users/JohannesPaulLangner/.lmstudio/models/lmstudio-community/Phi-3.1-mini-128k-instruct-GGUF/Phi-3.1-mini-128k-instruct-Q4_K_M.gguf";

        static async Task Main(string[] args)
        {
           Console.WriteLine("Welcome to the ChatbotLlama example!");
           Console.WriteLine("Choose chatbot type: ");
              Console.WriteLine("1. Aouda");
                Console.WriteLine("2. Titus");
                Console.WriteLine("3. Two chatbots talking to each other");
                Console.WriteLine("4 .Exit");
              Console.WriteLine("Type 'exit' to close the chat session.");
            Console.WriteLine();
            
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    await ChatWithBot();
                    break;
                case "3":
                    await ChatTalkToEachOther();
                    break;
                case "4":
                    break;
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static async Task ChatTalkToEachOther()
        {
            _chatbotLlamaType1 = new ChatbotLlama(ModelPath, "Remus1");
            var systemRoleDescription = new StringBuilder(); 
            systemRoleDescription.AppendLine("Transcript of a dialog, where the User interacts with an Assistant named Remus. ");
            systemRoleDescription.AppendLine("Remus ist ein Geschichtenerzähler und verwendet Historische Elemente aus dem Mittelalter und der zweiten Industriellen Revolution.");
            systemRoleDescription.AppendLine("Die Antworten, die ein Teil der Geschichte sind, werden in ein bis zwei Sätzen wieder gegeben.");
            // systemRoleDescription.AppendLine("Remus ist technisch versiert und spricht sich offen über seine Gedanken aus.");
            // systemRoleDescription.AppendLine("Remus stellt auch Fragen, um mehr über die Anforderung zu erfahren.");
            // systemRoleDescription.AppendLine("Remus ist ein Entwickler und hat Erfahrung in der Entwicklung von Komponenten.");
            // systemRoleDescription.AppendLine("Remus antwortet nicht mit Quellcode, sondern erklärt die Lösung in einfachen Worten."); 
            
            var history1 = _chatbotLlamaType1.LoadChatHistory(true, systemRoleDescription.ToString());
            string userInput1 = string.Empty;
            _chatbotLlamaType1.ChatAnswerEvent += (text) =>
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(text);
                userInput1 = text;
            };

            
            _chatbotLlamaType2 = new ChatbotLlama(ModelPath, "Romulus2");
            var systemRoleDescription2 = new StringBuilder();
            systemRoleDescription2.AppendLine("Transcript of a dialog, where the User interacts with an Assistant named Romulus. ");
            systemRoleDescription2.AppendLine("Romulus fragt nach Geschichten und was als nächstes kommt.");
            systemRoleDescription2.AppendLine("Romulus nach 5 bis 7 erhaltenden Antworten, gibt ein Ziel zu der Geschichte hinzu.");
            // systemRoleDescription2.AppendLine("Romulus ist Anforderungsmanager.");
            // systemRoleDescription2.AppendLine("Romulus stellt das Thema Komponenten Entwicklung für EUDR vor.");
            // systemRoleDescription2.AppendLine("Das Thema EUDR muss in das bestehende System integriert werden.");
            // systemRoleDescription2.AppendLine("Das Thema EUDR soll als Komponente funktionieren, so das später eine Migration nur wenig Aufwand besteht.");
            // systemRoleDescription2.AppendLine("Ziel des Thema ist, eine Lösung zu finden, die sich in 10 Sätzen erklären lässt.");
            
            var history = _chatbotLlamaType2.LoadChatHistory(true, systemRoleDescription.ToString());
            foreach (var chatItem in history)
            {
                Console.ForegroundColor = chatItem.Role switch
                {
                    "User" => ConsoleColor.Green,
                    "System" => ConsoleColor.Blue,
                    _ => ConsoleColor.White
                };
                Console.WriteLine($"{chatItem.Role}: {chatItem.Message}");
            }
            
            string userInput2 = string.Empty;
            _chatbotLlamaType2.ChatAnswerEvent += (text) =>
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write(text);
                userInput2 = text;
            };
            
            userInput1 = "Heute möchte ich eine Geschichte über eine Fantastisches Abenteuer im 19. Jahrhundert.";

            while (userInput2 != "exit")
            {
                await _chatbotLlamaType2.Chat(userInput1);
                Console.WriteLine();
                
                await Task.Delay(3000);
                
                await _chatbotLlamaType1.Chat(userInput2);
                Console.WriteLine();
                
                await Task.Delay(3000);

            }
            
            _chatbotLlamaType1.SaveChatHistory();
            _chatbotLlamaType2.SaveChatHistory();
        }

        private static async Task ChatWithBot()
        {
            StartChatBotTyp1(ModelPath);
            //StartChatBotTyp2(modelPath);
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("The chat session has started \nUser: ");
            Console.ForegroundColor = ConsoleColor.Green;
            string userInput = Console.ReadLine() ?? "";
            
            while (userInput != "exit")
            {
                Console.ForegroundColor = ConsoleColor.White;
                await _chatbotLlamaType1.Chat(userInput);
                
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("User: ");
                userInput = Console.ReadLine() ?? "";
            }
            
            _chatbotLlamaType1.SaveChatHistory();
        }

        private static void StartChatBotTyp1(string modelPath)
        {
            _chatbotLlamaType1 = new ChatbotLlama(modelPath, "Aouda");
            
            var systemRoleDescription = new StringBuilder();
            // Aouda
            systemRoleDescription.AppendLine("Transcript of a dialog, where the User interacts with an Assistant named Aouda. ");
            systemRoleDescription.AppendLine("Aouda ist hilfsbereit, ist kindlich, ihre Antworten entsprechen einem Kind von 10 Jahren.");
            systemRoleDescription.AppendLine("Aouda ist ehrlich, ist jedoch etwas ausfallend in ihrere Beschreibt die etwas fantastisch wirken!");
            systemRoleDescription.AppendLine("Aouda ist auch eine Abenteuerin und fügt bei ihren Antworten noch zwei bis drei Sätze aus einem bekannten Abenteuer hinzu");
            
            var history = _chatbotLlamaType1.LoadChatHistory(true, systemRoleDescription.ToString());
            foreach (var chatItem in history)
            {
                Console.ForegroundColor = chatItem.Role switch
                {
                    "User" => ConsoleColor.Green,
                    "System" => ConsoleColor.Yellow,
                    _ => ConsoleColor.White
                };
                Console.WriteLine($"{chatItem.Role}: {chatItem.Message}");
            }

            _chatbotLlamaType1.ChatAnswerEvent += (text) =>
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(text);
            };
        }
        
        // private static void StartChatBotTyp2(string modelPath)
        // {
        //     _chatbotLlamaType2 = new ChatbotLlama(modelPath);
        //     var systemRoleDescription = new StringBuilder();
        //
        //     // Space Marine
        //     systemRoleDescription.AppendLine("Transcript of a dialog, where the User interacts with an Assistant named Titus. ");
        //     systemRoleDescription.AppendLine("Titus ist ein Space Marine, der in der Welt von Warhammer 40k lebt.");
        //     systemRoleDescription.AppendLine("Titus ist loyal, mutig, und hat eine tiefe Stimme.");
        //     systemRoleDescription.AppendLine("Titus ist ein Krieger und spricht in einem militärischen Tonfall.");
        //     systemRoleDescription.AppendLine("Titus ist ein Space Marine und fügt bei seinen Antworten noch zwei bis drei Sätze aus dem Warhammer 40k Universum hinzu.");
        // }
    }
}
