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
        private static int time = 0;
        //sets wires used in the bomb
        private static string[] randomWires;
        //contains the correct wire value
        private static int correct;
        //default user 
        private static SocketUser user;
        private static Timer muteTime;
        private static IGuild currentGuild;
        private static Timer bombTime;


        [Command("tb")]
        public async Task BombSet(string user1 = "", [Remainder] string remainder = "")
        {
            if (user1 == "")
            {
                user = Context.Message.Author;
            }
            else
            {
                user1 = user1.Insert(2, "!");
                user = Context.Guild.Users.FirstOrDefault(x => x.Mention == user1);
            }


            currentGuild = Context.Guild;
            time = random.Next(15, 40);
            randomWires = RandomWires();
            string wireString = String.Join(", ", randomWires);
            EmbedBuilder embed = new EmbedBuilder();
            embed.AddField($"Bomb has been planted",
                $"Bento bot stuffs the bomb into {user.Mention}'s pants.  The display reads {time} seconds." +
                $"Diffuse the bomb by cutting the correct wire.There are {randomWires.Length} wires.They are {wireString}.");

            embed.WithColor(Color.Red);
            bombTime = new Timer(time * 1000);
            bombTime.Start();
            bombTime.Elapsed += bombTimeHandle;


            await ReplyAsync("", false, embed.Build());
            
        }
        
        [Command("cut")]
        public async Task TimeBomb([Remainder] string answer)
        {
            try
            {
                if (user == null)
                {
                    user = Context.Message.Author;
                }
                //true if answer was correct
                bool correctAnswer = false;

                if (answer == null)
                {
                    Console.WriteLine("Select a color or its number");
                    return;
                }
                else if (answer ==randomWires[correct])
                {
                    Console.WriteLine("correct");
                    correctAnswer = true;
                }
                else if (Convert.ToInt32(answer) - 1 == correct)
                {
                    correctAnswer = true;
                }

                if (correctAnswer)
                {

                    bombTime.Stop();
                    await ReplyAsync("Bomb has been diffused");
                    return;
                }
                else
                {   //mute for a certain time
                    BombGoesOff(currentGuild, user);
                }
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
                BombGoesOff(currentGuild, user);
                bombTime.Stop();
            }
            //when bomb hasn't been planted, message channel to set a bomb
            catch (Exception e)
            {
                await ReplyAsync("You must plant a bomb first");
                Console.WriteLine(e.Message);
            }
        }
        //activates when bomb timer goes off
        private void bombTimeHandle(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("bomeTimeHandle");
            BombGoesOff(currentGuild, user);
            bombTime.Stop();
        }

        //sets mute timer and send bomb explosion message to the channel
        public async void BombGoesOff(IGuild guild, IUser user)
        {
            Console.WriteLine("bombGoesOff");
            bombTime.Stop();
            //set mute timer between 30 and 120 seconds and message channel
            int muteSec = random.Next(30, 120);
            Mute(muteSec, currentGuild, user);
            await ReplyAsync($"**BOOOOOOOOM** {user} has been muted for {muteSec} seconds");
            
        }

        //mute user
        public async void Mute(int muteSec, IGuild guild, IUser user)
        {
            //Find mute role by its name and assign the role to the user
            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Muted");
            await (user as IGuildUser).AddRoleAsync(role);
            
            //await (user as IGuildUser).ModifyAsync(x => x.Mute = true);
            Console.WriteLine("role?");     
            //start timer for the length of the mute
            muteTime = new Timer(muteSec * 1000);
            muteTime.Start();
            muteTime.Elapsed += HandleTimerElapsed;            
        }
        //activates when mute timer goes off and unmute
        public async void HandleTimerElapsed(object sender, ElapsedEventArgs e)
        {
            //stop the timer
            muteTime.Stop();
            //find the role 
            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Muted");
            await (user as IGuildUser).RemoveRoleAsync(role);
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
            "Champagne", "Emerald", "Gold", "Maroon", "Poop", "Purple", "Red", "Ruby", "Salmon",
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