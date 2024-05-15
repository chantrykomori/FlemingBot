using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DiscordBotTemplate.Commands
{
    public class HeyChris : ApplicationCommandModule
    {
        [SlashCommand("heychris", "Get a random Chris Fleming quote")]
        public async Task HeyChrisCommand(InteractionContext context)
        {
            await context.DeferAsync();

            string randomQuote;
            using (StreamReader sr = new StreamReader("quotes.txt"))
            {
                string txt = await sr.ReadToEndAsync();
                string[] brokenList = txt.Split(',');
                Random random = new Random();
                int randomIndex = random.Next(brokenList.Length);
                randomQuote = brokenList[randomIndex];
            }

            var embed = new DiscordEmbedBuilder
            {
                Title = randomQuote,
                Color = DiscordColor.Purple
            };

            await context.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
        }
    }
}
