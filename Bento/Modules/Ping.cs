using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bento.Modules
{
    public class Ping:ModuleBase<SocketCommandContext>  
    {
        [Command("Bento")]

        public async Task PingAsync()
        {
            await ReplyAsync("Bento");
        }
    }
}
