using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FlemingBot.Commands
{
    public class HeyChris : ApplicationCommandModule
    {
        [SlashCommand("heychris", "Get a random Chris Fleming quote")]
        [SlashCooldown(1, 5, SlashCooldownBucketType.Channel)]
        public static async Task HeyChrisCommand(InteractionContext context)
        {
            await context.DeferAsync();

            string randomQuote;
            using (StreamReader sr = new("Config/quotes.txt"))
            {
                string txt = await sr.ReadToEndAsync();
                string[] brokenList = txt.Split('\n');
                Random random = new();
                int randomIndex = random.Next(brokenList.Length);
                randomQuote = brokenList[randomIndex];
            }

            var embed = new DiscordEmbedBuilder
            {
                Title = $"{randomQuote}",
                Color = DiscordColor.Purple
            };

            await context.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
        }
    }
}
