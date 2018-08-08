using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Bento
{
    class Program
    {

        private DiscordSocketClient Client;
        private CommandService Commands;
        private IServiceProvider Service;

        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();


        private async Task RunBotAsync()
        {
            Client = new DiscordSocketClient();
            Commands = new CommandService();

            Service = new ServiceCollection()
                .AddSingleton(Client)
                .AddSingleton(Commands)
                .BuildServiceProvider();

            string botToken = "NDc2NTM1ODk5MDk4NzEwMDI5.DkvOSg.rU2IfaPepodWvLgcCPYhwRpPOFw";

            //event subscription

            Client.Log += Log;
            Client.UserJoined += AnnounceUserJoined;

            await RegisterCommandsAsync();

            await Client.LoginAsync(TokenType.Bot, botToken);

            await Client.StartAsync();

            await Task.Delay(-1);



        }

        private async Task AnnounceUserJoined(SocketGuildUser user)
        {
            var guild = user.Guild;
            var channel = guild.DefaultChannel;
            await channel.SendMessageAsync($"Welcome, {user.Mention}");
        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);

            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            Client.MessageReceived += HandleCommandAsync;

            await Commands.AddModulesAsync(Assembly.GetEntryAssembly());


        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;

            if (message is null || message.Author.IsBot) return;

            int argPos = 0;

            if (message.HasStringPrefix("!", ref argPos) || message.HasMentionPrefix(Client.CurrentUser, ref argPos)){
                var context = new SocketCommandContext(Client, message);

                var result = await Commands.ExecuteAsync(context, argPos, Service);

                if (!result.IsSuccess)
                {
                    Console.WriteLine(result.Error);
                }
            }

        }
    }
}
