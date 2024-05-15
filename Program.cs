using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using DSharpPlus.SlashCommands.EventArgs;
using FlemingBot.Commands;
using FlemingBot.Config;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FlemingBot
{
    public sealed class Program
    {
        public static DiscordClient Client { get; private set; }
        public static CommandsNextExtension Commands { get; private set; }
        static async Task Main(string[] args)
        {
            //1. Get the details of your config.json file by deserialising it
            var configJsonFile = new JSONReader();
            await configJsonFile.ReadJSON();

            //2. Setting up the Bot Configuration
            var discordConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = configJsonFile.token,
                TokenType = TokenType.Bot,
                AutoReconnect = true
            };

            //3. Apply this config to our DiscordClient
            Client = new DiscordClient(discordConfig);

            //4. Set the default timeout for Commands that use interactivity
            Client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(2)
            });

            //5. Set up the Task Handler Ready event
            Client.Ready += OnClientReady;
            Client.MessageCreated += MessageCreatedHandler;

            //6. Set up the Commands Configuration
            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { configJsonFile.prefix },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = false,
            };

            Commands = Client.UseCommandsNext(commandsConfig);

            var slashCommandsConfiguration = Client.UseSlashCommands();

            slashCommandsConfiguration.SlashCommandErrored += SlashCommandErrorHandler;

            //7. Register your commands

            Commands.RegisterCommands<Basic>();
            slashCommandsConfiguration.RegisterCommands<HeyChris>();

            //8. Connect to get the Bot online
            await Client.ConnectAsync();
            await Task.Delay(-1);
        }

        private static async Task SlashCommandErrorHandler(SlashCommandsExtension sender, SlashCommandErrorEventArgs args)
        {
            if (args.Exception is SlashExecutionChecksFailedException exception)
            {
                string timeLeft = string.Empty;
                foreach (var check in exception.FailedChecks)
                {
                    SlashCooldownAttribute cooldown = (SlashCooldownAttribute)check;
                    timeLeft = cooldown.GetRemainingCooldown(args.Context).ToString(@"hh\:mm\:ss");
                }

                var cooldownMessage = new DiscordEmbedBuilder
                {
                    Color = DiscordColor.Red,
                    Title = "Hold your horses, hoss!",
                    Description = $"Cooldown: {timeLeft} remaining"
                };

                await args.Context.Channel.SendMessageAsync(cooldownMessage);
            }
        }

        private static async Task MessageCreatedHandler(DiscordClient sender, MessageCreateEventArgs args)
        {
            if (args.Message.Content.Contains("boba", StringComparison.CurrentCultureIgnoreCase))
            {
                await args.Message.RespondAsync("https://www.youtube.com/watch?v=uKKSDu_a9ik&pp=ygUOYm9iYSBtYW5pZmVzdG8%3D");
            }

            else if (args.Message.Content.Contains("depiglio", StringComparison.CurrentCultureIgnoreCase))
            {
                FileStream file = new("Config/depiglio.png", FileMode.Open, FileAccess.Read);
                DiscordMessageBuilder builder = new DiscordMessageBuilder().AddFile(file);
                await args.Message.RespondAsync(builder);
            }
        }

        private static Task OnClientReady(DiscordClient sender, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}
