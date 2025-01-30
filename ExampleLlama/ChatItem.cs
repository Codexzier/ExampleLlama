namespace ExampleLlama;

public class ChatItem(string role, string message)
{
    public string Role { get; } = role;
    public string Message { get; } = message;
}