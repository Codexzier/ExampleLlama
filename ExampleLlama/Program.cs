using System.Speech.Synthesis;
using System.Text;

namespace ExampleLlama
{
    internal class Program
    {
        private static ChatbotLlama _chatbotLlamaType2;
        private static ChatbotLlama _chatbotLlamaType1;

        private static SpeechSynthesizer _synthesizer = new SpeechSynthesizer();
        
        static async Task Main(string[] args)
        {
            string modelPath = "C:/Users/Codexzier/.lmstudio/models/lmstudio-community/Phi-3.1-mini-128k-instruct-GGUF/Phi-3.1-mini-128k-instruct-Q4_K_M.gguf";

            StartChatBotTyp1(modelPath);
            
            
           
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

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
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
            
            var historie = _chatbotLlamaType1.LoadChatHistory(true, systemRoleDescription.ToString());
            foreach (var chatItem in historie)
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
                
                // 
                _synthesizer.Speak(text);
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
