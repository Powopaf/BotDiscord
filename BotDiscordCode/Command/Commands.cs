using Discord.Commands;
namespace BotDiscordCode.Command;
public class CommandTest : ModuleBase<SocketCommandContext>
{
    [Command("ping")]
    public async Task PingPongAsync()
    {
        await ReplyAsync("Pong");
    }

}