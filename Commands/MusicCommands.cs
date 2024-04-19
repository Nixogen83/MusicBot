using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using System.Linq;
using System.Threading.Tasks;

namespace MusicBot.Commands
{
    class MusicCommands : BaseCommandModule
    {
        [Command("test")]
        public async Task TestCommand(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Test Message");
        }

        [Command("help")]
        public async Task Help(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Use prefix ! before every command. Don't write []\n" +
                "DONT FORGET TO JOIN VC \n" +
                "Commands list:\n" +
                "!play [track name] - Starts the named track\n" +
                "!pause - pause track\n" +
                "!resume - resume track there stopped\n" +
                "!stop - stop playing track and disconnect from the server");
        }

        [Command("play")]
        public async Task PlayMusic(CommandContext ctx, [RemainingText] string query)
        {
            //Check user in voice chanel
            var userVC = ctx.Member.VoiceState.Channel;
            var lavalinkInstance = ctx.Client.GetLavalink();

            //PRE Checks
            if (ctx.Member.VoiceState == null || userVC == null)
            {
                await ctx.Channel.SendMessageAsync("Please enter a VC!");
                return;
            }

            if (!lavalinkInstance.ConnectedNodes.Any())
            {
                await ctx.Channel.SendMessageAsync("Connection failed!");
                return;
            }

            //Check user in right VC
            if (userVC.Type != ChannelType.Voice)
            {
                await ctx.Channel.SendMessageAsync("Please enter a valid VC!");
                return;
            }

            //Connecting to VC and playing music
            var node = lavalinkInstance.ConnectedNodes.Values.First();
            await node.ConnectAsync(userVC);

            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);
            if (conn == null)
            {
                await ctx.Channel.SendMessageAsync("Lavalink can't connect");
                return;
            }

            //Lavalink searching services
            var searchQuery = await node.Rest.GetTracksAsync(query);
            if (searchQuery.LoadResultType == LavalinkLoadResultType.NoMatches || searchQuery.LoadResultType == LavalinkLoadResultType.LoadFailed)
            {
                await ctx.Channel.SendMessageAsync($"Failed to find music: {query}");
                return;
            }

            var musicTrack = searchQuery.Tracks.First();

            await conn.PlayAsync(musicTrack);

            string musicDescription = $"Now Playing: {musicTrack.Title} \n" +
                                      $"Author: {musicTrack.Author} \n" +
                                      $"Length: {musicTrack.Length} \n" +
                                      $"URL: {musicTrack.Uri}";

            var nowPlaying = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Yellow,
                Title = $"Successfuly joined channel {userVC.Name} and playing music",
                Description = musicDescription
            };

            await ctx.Channel.SendMessageAsync(embed: nowPlaying);
        }

        [Command("pause")]
        public async Task PauseMusic(CommandContext ctx)
        {
            var userVC = ctx.Member.VoiceState.Channel;
            var lavalinkInstance = ctx.Client.GetLavalink();

            //Pre Checks
            if (ctx.Member.VoiceState == null || userVC == null)
            {
                await ctx.Channel.SendMessageAsync("Please enter a VC!");
                return;
            }

            if (!lavalinkInstance.ConnectedNodes.Any())
            {
                await ctx.Channel.SendMessageAsync("Connection failed!");
                return;
            }

            //Check user in right VC
            if (userVC.Type != ChannelType.Voice)
            {
                await ctx.Channel.SendMessageAsync("Please enter a valid VC!");
                return;
            }

            var node = lavalinkInstance.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.Channel.SendMessageAsync("Lavalink Failed to Connect!");
                return;
            }

            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.Channel.SendMessageAsync("No tracks are playing!");
                return;
            }

            await conn.PauseAsync();

            string musicDescription = $"Track Name: {conn.CurrentState.CurrentTrack.Title}";

            var pausedEmbed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Gray,
                Title = "Track Paused!",
                Description = musicDescription
            };

            await ctx.Channel.SendMessageAsync(embed: pausedEmbed);
        }

        [Command("resume")]
        public async Task ResumeMusic(CommandContext ctx)
        {
            var userVC = ctx.Member.VoiceState.Channel;
            var lavalinkInstance = ctx.Client.GetLavalink();

            //Pre Checks
            if (ctx.Member.VoiceState == null || userVC == null)
            {
                await ctx.Channel.SendMessageAsync("Please enter a VC!");
                return;
            }

            if (!lavalinkInstance.ConnectedNodes.Any())
            {
                await ctx.Channel.SendMessageAsync("Connection failed!");
                return;
            }

            //Check user in right VC
            if (userVC.Type != ChannelType.Voice)
            {
                await ctx.Channel.SendMessageAsync("Please enter a valid VC!");
                return;
            }

            var node = lavalinkInstance.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.Channel.SendMessageAsync("Lavalink Failed to Connect!");
                return;
            }

            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.Channel.SendMessageAsync("No tracks are playing!");
                return;
            }

            await conn.ResumeAsync();

            string musicDescription = $"Track Name: {conn.CurrentState.CurrentTrack.Title}";

            var resumeEmbed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Green,
                Title = "Track Resumed!",
                Description = musicDescription
            };

            await ctx.Channel.SendMessageAsync(embed: resumeEmbed);
        }

        [Command("stop")]
        public async Task StopMusic(CommandContext ctx)
        {
            var userVC = ctx.Member.VoiceState.Channel;
            var lavalinkInstance = ctx.Client.GetLavalink();

            //Pre Checks
            if (ctx.Member.VoiceState == null || userVC == null)
            {
                await ctx.Channel.SendMessageAsync("Please enter a VC!");
                return;
            }

            if (!lavalinkInstance.ConnectedNodes.Any())
            {
                await ctx.Channel.SendMessageAsync("Connection failed!");
                return;
            }

            //Check user in right VC
            if (userVC.Type != ChannelType.Voice)
            {
                await ctx.Channel.SendMessageAsync("Please enter a valid VC!");
                return;
            }

            var node = lavalinkInstance.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.Channel.SendMessageAsync("Lavalink Failed to Connect!");
                return;
            }

            await conn.StopAsync();
            await conn.DisconnectAsync();

            var stopEmbed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Red,
                Title = "Track stopped",
                Description = "Disconnected from VC!"
            };

            await ctx.Channel.SendMessageAsync(embed: stopEmbed);
        }
    }
}