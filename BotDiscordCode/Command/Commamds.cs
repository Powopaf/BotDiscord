using Discord.Interactions;

namespace BotDiscordCode.Command;

public class Commands : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ping", "pong")]
    public async Task Echo(string input)
    {
        await ReplyAsync("Pong");
    }
}