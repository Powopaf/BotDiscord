using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BotDiscordCode;

public class Program
{
    
    
    private DiscordSocketClient _client = null!;
    private CommandService _commands = null!;

    public static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

    private async Task RunBotAsync()
    {
        
        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent,
            LogLevel = LogSeverity.Debug 
        });
        _commands = new CommandService();
        _client.Log += Log;
        _client.Ready += () =>
        {
            Console.WriteLine("Guard is Ready");
            return Task.CompletedTask;
        };
        
        await InstallCommandAsync();
        
        await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DiscordBotGuardToken", EnvironmentVariableTarget.User));
        
        await _client.StartAsync();
        await Task.Delay(-1);
    }

    private async Task InstallCommandAsync()
    {
        _client.MessageReceived += ReadMessageAsync;
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);
    }

    private async Task ReadMessageAsync(SocketMessage socketMessage)
    {
        var message = (SocketUserMessage)socketMessage;
        
        int argPos = 0; //place of the arg of command (!)
        
        if (!message.HasCharPrefix('!', ref argPos)) return; //check if it's a command

        var context = new SocketCommandContext(_client, message);
        var result = await _commands.ExecuteAsync(context, argPos, null);
        
        //error
        if (!result.IsSuccess)
        {
            await context.Channel.SendMessageAsync(result.ErrorReason);
        }
    }

    private Task Log(LogMessage arg)
    {
        Console.WriteLine(arg);
        return Task.CompletedTask;
    }
}