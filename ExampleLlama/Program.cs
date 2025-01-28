using LLama;
using LLama.Common;
using LLama.Sampling;
using System.Text;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace ExampleLlama
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string modelPath = "C:/Users/Codexzier/.lmstudio/models/lmstudio-community/Phi-3.1-mini-128k-instruct-GGUF/Phi-3.1-mini-128k-instruct-Q4_K_M.gguf";

            var parameters = new ModelParams(modelPath)
            {
                ContextSize = 1024, // The longest length of chat as memory.
                GpuLayerCount = 5, // How many layers to offload to GPU. Please adjust it according to your GPU memory.
            };

            using var model = LLamaWeights.LoadFromFile(parameters);
            using var context = model.CreateContext(parameters);
            var executor = new InteractiveExecutor(context);

            // add chat historie as prompt to tell AI how to act.
            var chatHistory = new ChatHistory();

            var systemRoleDescription = new StringBuilder();
            //systemRoleDescription.AppendLine("Transcript of a dialog, where the User interacts with an Assistant named Aouda. ");
            //systemRoleDescription.AppendLine("Aouda is helpful, kind, honest, good at writing, and never fails to answer the user's request immediately and with precision.");

            // Aouda
            //systemRoleDescription.AppendLine("Transcript of a dialog, where the User interacts with an Assistant named Aouda. ");
            //systemRoleDescription.AppendLine("Aouda ist hilfsbereit, ist kindlich, ihre Antworten entsprechen einem Kind von 10 Jahren.");
            //systemRoleDescription.AppendLine("Aouda ist ehrlich, ist jedoch etwas ausfallend in ihrere Beschreibt die etwas fantastisch wirken!");
            //systemRoleDescription.AppendLine("Aouda ist auch eine Abenteuerin und fügt bei ihren Antworten noch zwei bis drei Sätze aus einem bekannten Abenteuer hinzu");

            // Space Marine
            systemRoleDescription.AppendLine("Transcript of a dialog, where the User interacts with an Assistant named Titus. ");
            systemRoleDescription.AppendLine("Titus ist ein Space Marine, der in der Welt von Warhammer 40k lebt.");
            systemRoleDescription.AppendLine("Titus ist loyal, mutig, und hat eine tiefe Stimme.");
            systemRoleDescription.AppendLine("Titus ist ein Krieger und spricht in einem militärischen Tonfall.");
            systemRoleDescription.AppendLine("Titus ist ein Space Marine und fügt bei seinen Antworten noch zwei bis drei Sätze aus dem Warhammer 40k Universum hinzu.");


            chatHistory.AddMessage(AuthorRole.System, systemRoleDescription.ToString());
            //chatHistory.AddMessage(AuthorRole.User, "Hallo Aouda!");
            //chatHistory.AddMessage(AuthorRole.System, "Hallo, wie kann ich helfen?");

            chatHistory.AddMessage(AuthorRole.User, "Hallo Titus!");
            chatHistory.AddMessage(AuthorRole.System, "Wie kann ic dem Imperator helfen?");

            ChatSession session = new ChatSession(executor, chatHistory);

            InferenceParams inferenceParams = new InferenceParams
            {
                SamplingPipeline = new DefaultSamplingPipeline(), // use defaul sampling pipeline
                MaxTokens = 256, // maximum number of tokens to generate
                AntiPrompts = new List<string> { "User:" }, // avoid generating prompts that start with "Bob:"
            };

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("The chat session has started \nUser: ");
            Console.ForegroundColor = ConsoleColor.Green;
            string userInput = Console.ReadLine() ?? "";
            
            while (userInput != "exit")
            {
                //Console.Write("Aouda: ");
                Console.Write("Titus: ");
                await foreach (var text in session.ChatAsync(new ChatHistory.Message(AuthorRole.User, userInput), inferenceParams))
                {
                    if(text.StartsWith("System") || text.StartsWith(":"))
                        { 
                        continue;
                    }
                    

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(text);
                }
                
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("User: ");
                userInput = Console.ReadLine() ?? "";
            }


            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
