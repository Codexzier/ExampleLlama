using System.Text;
using LLama;
using LLama.Common;
using LLama.Sampling;

namespace ExampleLlama;

public class ChatbotLlama : IDisposable
{
    private readonly string _assistanceName;
    private readonly string _fileHistorie = $"{Environment.CurrentDirectory}/chatHistory_";
    
    private readonly InteractiveExecutor _executor;
    private ChatHistory _chatHistory;
    private InferenceParams _inferenceParams;
    private ChatSession _session;
    private readonly LLamaWeights _model;
    private readonly LLamaContext _context;

    public ChatbotLlama(string modelPath, string assistanceName)
    {
        this._assistanceName = assistanceName;
        var parameters = new ModelParams(modelPath)
        {
            ContextSize = 1024, // The longest length of chat as memory.
            GpuLayerCount = 5, // How many layers to offload to GPU. Please adjust it according to your GPU memory.
        };

        this._model = LLamaWeights.LoadFromFile(parameters);
        this._context = _model.CreateContext(parameters);
        this._executor = new InteractiveExecutor(_context);

        this._inferenceParams = new InferenceParams
        {
            SamplingPipeline = new DefaultSamplingPipeline(), // use default sampling pipeline
            MaxTokens = 256, // maximum number of tokens to generate
            AntiPrompts = new List<string> { "User:" }, // avoid generating prompts that start with "Bob:"
        };
    }
    
    private string GetHistoryFileName()
    {
        return $"{this._fileHistorie}{this._assistanceName}.json";
    }
    
    public List<ChatItem> LoadChatHistory(bool loadFromFile = true, string systemRoleDescription = "")
    {
        if (loadFromFile && File.Exists(this.GetHistoryFileName()))
        {
            var chatHistory = File.ReadAllText(this.GetHistoryFileName());
            this._chatHistory = ChatHistory.FromJson(chatHistory) ?? new ChatHistory();
        }
        else
        {
            this._chatHistory = new ChatHistory();
            this._chatHistory.AddMessage(AuthorRole.System, systemRoleDescription);
        }

        this._session = new ChatSession(this._executor, this._chatHistory); 
        
        var chatItems = this._chatHistory
            .Messages
            .Select(s => new ChatItem(s.AuthorRole.ToString(), s.Content))
            .ToList();
        return chatItems;
    }
    
    public async Task Chat(string userInput)
    {
        await foreach (var text in _session.ChatAsync(new ChatHistory.Message(AuthorRole.User, userInput), _inferenceParams))
        {
            ChatAnswerEvent?.Invoke(text);
        }
    }
    
    public delegate void ChatAnswerEventHandler(string message);
    public event ChatAnswerEventHandler ChatAnswerEvent;

    public void Dispose()
    {
        this._model.Dispose();
        this._context.Dispose();
    }

    public void SaveChatHistory()
    {
        File.WriteAllText(this.GetHistoryFileName(), this._chatHistory.ToJson());
    }
}

public class ChatItem(string role, string message)
{
    public string Role { get; } = role;
    public string Message { get; } = message;
}
