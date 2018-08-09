using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;

namespace Bento.Modules
{

    public class tb : ModuleBase<SocketCommandContext>
    {

        private static Random random = new Random();
        //time set for the user to answer
        private static int time = random.Next(15, 40);
        //sets wires used in the bomb
        private string[] randomWires = RandomWires();
        //contains the correct wire value
        private static int correct;
        //default user 
        private static IGuildUser user;
        private static Timer muteTime;
        private static IGuild currentGuild;


        [Command("tb")]
        public async Task BombSet()
        {
            user = Context.Guild.CurrentUser;
            currentGuild = Context.Guild;
            string wireString = String.Join(", ", randomWires);
            EmbedBuilder embed = new EmbedBuilder();
            embed.AddField($"Bomb has been planted",
                $"Bento bot stuffs the bomb into {user}'s pants.  The display reads {time} seconds." +
                $"Diffuse the bomb by cutting the correct wire.There are {randomWires.Length} wires.They are {wireString}.");

            embed.WithColor(Color.Red);

            Timer bombTime = new Timer(time * 1000);


            await ReplyAsync("", false, embed.Build());
        }
        [Command("tb")]
        public async Task BombSet(IGuildUser user1)
        {
            user = user1;
            currentGuild = Context.Guild;
            string wireString = String.Join(", ", randomWires);
            EmbedBuilder embed = new EmbedBuilder();
            embed.AddField($"Bomb has been planted",
                $"Bento bot stuffs the bomb into {user}'s pants.  The display reads {time} seconds." +
                $"Diffuse the bomb by cutting the correct wire.There are {randomWires.Length} wires.They are {wireString}.");

            embed.WithColor(Color.Red);

            await ReplyAsync("", false, embed.Build());
        }
        //does the same thing as above but allows additional comments after the mention
        [Command("tb")]
        public async Task BombSet(IGuildUser user1, [Remainder] string remainder)
        {
            user = user1;
            currentGuild = Context.Guild;
            string wireString = String.Join(", ", randomWires);
            EmbedBuilder embed = new EmbedBuilder();
            embed.AddField($"Bomb has been planted",
                $"Bento bot stuffs the bomb into {user}'s pants.  The display reads {time} seconds." +
                $"Diffuse the bomb by cutting the correct wire.There are {randomWires.Length} wires.They are {wireString}.");

            embed.WithColor(Color.Red);

            await ReplyAsync("", false, embed.Build());
        }


        [Command("cut")]
        public async Task TimeBomb(string answer)
        {
            //true if answer was correct
            bool correctAnswer = false;
            if (answer == null)
            {
                Console.WriteLine("Select a color or its number");
                return;
            }
            else if (answer == randomWires[correct] || Convert.ToInt32(answer) - 1 == correct)
            {
                correctAnswer = true;
            }

            if (correctAnswer){
                Console.WriteLine("Bomb has been diffused");
                return;
            }
            else
            {   //mute for a certain time
                BombGoesOff(currentGuild, user);
               
            }


        }


        public async void BombGoesOff(IGuild guild, IUser user)
        {
            int muteSec = random.Next(30, 120);
            Mute(muteSec, currentGuild, user);
            await ReplyAsync($"{user} has been muted for {muteSec} seconds");
        }

        //mute user
        public async void Mute(int muteSec, IGuild guild, IUser user)
        {
            //MUTE USER HERE

            await (user as IGuildUser).ModifyAsync(x => x.Mute = true);
            
            Timer muteTime = new Timer(muteSec * 1000);
            muteTime.Elapsed += HandleTimerElapsed;

            
        }
        //unmute
        public async void HandleTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // UNMUTE USER
            await(user as IGuildUser).ModifyAsync(x => x.Mute = false);
        }

        //sets existing wires and return in sorted order
        public static string[] RandomWires()
        {
            //maximum number of wires
            int maxNum = 7;
            //number of wires randomly selected between 1 and 7
            int num = random.Next(1, maxNum);
            //the correct wire
            correct = random.Next(0, num - 1);
            //wires to choose from
            string[] colorList = new string[] {"Lavender", "Chartreuse", "Amber",
            "Amethyst", "Apricot", "Aquamarine", "Beige", "Black", "Blue", "Brown", "Cerise",
            "Champagne", "Emerald", "Gold", "Maroon", "Navy Blue",  "Poop", "Purple", "Red", "Ruby", "Salmon",
            "Sangria", "Sapphire", "Scarlet", "Silver", "Spring Bud", "Tan", "Teal", "Violet",
            "White", "Yellow"};
            //pick the first wire
            int wireNum = random.Next(0, colorList.Length);
            //create wire pool
            string[] randomWires = new string[num];
            //put wires into the pool to return
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
            //sort the wires in order
            Array.Sort(randomWires);
            return randomWires;
        }

        

    }
}