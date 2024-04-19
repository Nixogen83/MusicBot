using MusicBot.Commands;
using MusicBot.Config;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;
using System;
using System.Threading.Tasks;

namespace MusicBot
{
    public sealed class Program
    {
        public static DiscordClient Client { get; private set; }
        public static CommandsNextExtension Commands { get; private set; }
        static async Task Main(string[] args)
        {
            // Get the details config.json file by deserialising it
            var configJsonFile = new JSONReader();
            await configJsonFile.ReadJSON();

            // Setting up the Bot Configuration
            var discordConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = configJsonFile.token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
            };

            // Apply this config  DiscordClient
            Client = new DiscordClient(discordConfig);

            // Set the default timeout for Commands that use interactivity
            Client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(2)
            });

            // Set up the Task Handler Ready event
            Client.Ready += OnClientReady;

            // Set up the Commands Configuration
            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { configJsonFile.prefix },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = false,
            };

            Commands = Client.UseCommandsNext(commandsConfig);


            // Register commands

            Commands.RegisterCommands<MusicCommands>();

            // Lavalink Configuration

            var endpoint = new ConnectionEndpoint
            {
                Hostname = "lavalink-v3.oryzen.xyz",
                Port = 80,
                Secured = false
            };

            var lavalinkConfig = new LavalinkConfiguration
            {
                Password = "oryzen.xyz",
                RestEndpoint = endpoint,
                SocketEndpoint = endpoint
            };

            var lavalink = Client.UseLavalink();

            // Connect to get the Bot online
            await Client.ConnectAsync();
            await lavalink.ConnectAsync(lavalinkConfig);

            await Task.Delay(-1); //Infinite loop to prevent application from closing
        }

        private static Task OnClientReady(DiscordClient sender, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}
