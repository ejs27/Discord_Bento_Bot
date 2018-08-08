using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bento.Modules
{
    public class tb:ModuleBase<SocketCommandContext>
    {

        private static Random random = new Random();
        private static int time = random.Next(15, 40);

        [Command("tb")]
        public async Task PingAsync(string name)
        {


            string[] randomWires = RandomWires();
            string wireString = String.Join(", ", randomWires);
            EmbedBuilder embed = new EmbedBuilder();
            embed.AddField($"Bomb has been planted",
                $"Bento bot stuffs the bomb into {name}'s pants.  The display reads {time} seconds." +
                $"Diffuse the bomb by cutting the correct wire.There are {randomWires.Length} wires.They are {wireString}.");
            

            //foreach (string wire in randomWires)
            //{
            //    embed.AddField($"Bomb has been planted");
            //}
            //);
            embed.WithColor(Color.Red);


            

            await ReplyAsync("", false, embed.Build());
        }

        public async Task PingAsync(string name, [Remainder] string remainder)
        {

            EmbedBuilder embed = new EmbedBuilder();
            embed.AddField($"Bomb has been planted",
                $"Bento bot stuffs the bomb into {name}'s pants.  The display reads {time} seconds." +
                "Diffuse the bomb by cutting the correct wire.There are six wires.They are Lavender, Chartreuse, Orange, Teal, White and Aquamarine.");
            embed.WithColor(Color.Red);

            await ReplyAsync("", false, embed.Build());
        }



        public static string[] RandomWires()
        {
            Random random = new Random();
            int maxNum = 7;
            int num = random.Next(1, maxNum);
            string[] colorList = new string[] {"Lavender", "Chartreuse", "Amber",
            "Amethyst", "Apricot", "Aquamarine", "Beige", "Black", "Blue", "Brown", "Cerise",
            "Champagne", "Emerald", "Gold", "Maroon", "Navy Blue",  "Poop", "Purple", "Red", "Ruby", "Salmon",
            "Sangria", "Sapphire", "Scarlet", "Silver", "Spring Bud", "Tan", "Teal", "Violet",
            "White", "Yellow"};
            int wireNum = random.Next(0, colorList.Length);
            string[] randomWires = new string[num];

            for (int i = 0; i < num; i++)
            {
                wireNum = random.Next(0, colorList.Length);
                if (i == 0)
                {
                    randomWires[i] = colorList[wireNum];
                }
                else
                {
                    while (randomWires.Contains(colorList[wireNum]))
                    {
                        wireNum = random.Next(0, colorList.Length);
                    }
                    randomWires[i] = colorList[wireNum];
                }
            }
            Array.Sort(randomWires);
            return randomWires;

        
        }

        
    }
}
