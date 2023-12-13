using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace BotDiscordCode;

public class Program
{
    
    
    private DiscordSocketClient _client = null!;
    private IServiceProvider _serviceProvider = null!;
    private InteractionService _interactionService;

    public static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

    private async Task RunBotAsync()
    {
        
        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent,
            LogLevel = LogSeverity.Debug 
        });
        _serviceProvider = CreateServices();
        const ulong id = 1049727057875177472; // id of Powopaf's serveur
        _client.Log += Log;
        _client.Ready += () =>
        {
            Console.WriteLine("Guard is Ready");
            return Task.CompletedTask;
        };
        _interactionService = new InteractionService(_client);
        _serviceProvider = CreateServices();
        // token only work on windows if the token is in the environmental variable 
        await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DiscordBotGuardToken", EnvironmentVariableTarget.User));
        
        var interactionService = new InteractionService(_client);
        await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
        await interactionService.RegisterCommandsToGuildAsync(id);
        _client.InteractionCreated += async interaction =>
        {
            var scope = _serviceProvider.CreateScope();
            var ctx = new SocketInteractionContext(_client, interaction);
            await _interactionService.ExecuteCommandAsync(ctx, scope.ServiceProvider);
        };
        await _client.StartAsync();
        await Task.Delay(-1);
    }
    
    static IServiceProvider CreateServices()
    {
        var collection = new ServiceCollection();
        return collection.BuildServiceProvider();
    }
    private Task Log(LogMessage arg)
    {
        Console.WriteLine(arg);
        return Task.CompletedTask;
    }
}