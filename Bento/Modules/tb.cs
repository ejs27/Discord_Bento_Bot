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
            string[] colorList = new string[] {"Lavender", "Chartreuse", "Amber",
            "Amethyst", "Apricot", "Aquamarine", "Beige", "Black", "Blue", "Brown", "Cerise",
            "Champagne", "Emerald", "Gold", "Maroon", "Navy Blue",  "Poop", "Purple", "Red", "Ruby", "Salmon",
            "Sangria", "Sapphire", "Scarlet", "Silver", "Spring Bud", "Tan", "Teal", "Violet",
            "White", "Yellow"};


            int wireNum = 2;
            string[] randomWires = new string[wireNum];
            int randomWire = random.Next(0,1);
            Dictionary<int, int> dict = new Dictionary<int, int>();
            int co = 0;
            for (int i = 0; i < wireNum; i++) {
                //do { 
                //    int randomWire = random.Next(0, 5);
                //    Console.WriteLine("Random: " + randomWire);
                //    Console.WriteLine("Values: " + String.Join(",", dict.Values));
                //    co++;
                //}
                if (i == 0)
                {
                    dict.Add(0, randomWire);
                }
                else
                {
                    randomWire = random.Next(0, 1);
                    while (!dict.Values.All(x => x != randomWire))
                    {
                        randomWire = random.Next(0, 1);
                        Console.WriteLine("Random: " + randomWire);
                        Console.WriteLine("Values: " + String.Join(",", dict.Values));
                        co++;
                    }
                    
                    dict.Add(i, randomWire);
                }
            }
            
            foreach (int num in dict.Values){
                int count = 0;
                randomWires[count] = colorList[num];
                count++;
                Console.WriteLine(String.Join(",", randomWires));
                Console.WriteLine(colorList[num]);
            }
            Array.Sort(randomWires);

            return randomWires;

        }

        
    }
}
