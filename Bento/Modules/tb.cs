using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
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
        //contains the correct wire color
        private static String correctWire;
        private static SocketUser user;
        private static Timer muteTime;
        private static IGuild currentGuild;
        private static Timer bombTime;
        private static bool isBombSet = false;

        //"!tb" initiates for time bomb game. A user ID can be followed to select the person to bomb. If user ID is missing, time bomb the initated user
        [Command("tb")]
        public async Task BombSet(string user1 = "", [Remainder] string remainder = "")
        {
            //if a bomb has already been set, do not allow another bomb to be set
            if (isBombSet)
            {
                await ReplyAsync("Bomb has already been planted");
                return;
            }
            //check if a specific user is bombed.
            if (user1 == "")
            {
                user = Context.Message.Author;
            }
            else
            {
                user1 = user1.Insert(2, "");
                user = Context.Guild.Users.FirstOrDefault(x => x.Mention == user1);
            }

            //check if mentioned user is online or a bot
            if (user.Status == UserStatus.Offline) {
                await ReplyAsync("User must be online");
                return;
            };
            //sets a random time for the user to select the cable to cut and prompt the user with selection of cables.
            currentGuild = Context.Guild;
            time = random.Next(15, 40);
            randomWires = RandomWires();
            string wireString = String.Join(", ", randomWires);
            EmbedBuilder embed = new EmbedBuilder();
            embed.AddField($"Bomb has been planted",
                $"Bento bot stuffs the bomb into {user.Mention}'s pants.  The display reads {time} seconds. " +
                $"Diffuse the bomb by cutting the correct wire.There are **{randomWires.Length}** wires.They are **{wireString}**.");

            embed.WithColor(Color.Red);
            bombTime = new Timer(time * 1000);
            bombTime.Start();
            bombTime.Elapsed += bombTimeHandle;
            isBombSet = true;
            Console.WriteLine($"{user} has set off the bomb. The answer is {(correct + 1)}. correctWire");
            await ReplyAsync("", false, embed.Build());
            
        }
        

        /*user who has been chosen can answer to cut the bomb before it goes off. The user must answer with "!cut {color}" in order to cut.
        Wrong answer will result in bomb going off*/
        [Command("cut")]
        public async Task TimeBomb(string answer = "", [Remainder] string remainder = "")
        {

            //check if bomb has been planted. Prompt to set bomb if it has not.
            if (!isBombSet)
            {
                await ReplyAsync("bomb has not been set. \"!tb\" to set a bomb");
                return;
            }
                        
            try
            {               
                bool correctAnswer = false;
                //if the answer is not provided
                if (answer.Length == 0)
                {
                    await ReplyAsync("Select a color or its number");
                    return;
                }
                //check if the user's answer is correct.
                else if (answer.ToUpper().Equals(correctWire.ToUpper()))
                {
                    Console.WriteLine("correct");
                    correctAnswer = true;
                }
                //check if the answer given was in numbers.
                else if (int.TryParse(answer, out _) && (Convert.ToInt32(answer) - 1 == correct))
                {                    
                    correctAnswer = true;
                }
                //if the user answered correctly, diffuse bomb.
                if (correctAnswer)
                {
                    bombTime.Stop();
                    isBombSet = false;
                    await ReplyAsync("Bomb has been diffused");
                    return;
                }
                else
                {   //mute for a certain time
                    BombGoesOff();
                }
            }
            catch (Exception e)
            {
                await ReplyAsync(e.Message, false);
                
                Console.WriteLine(e.Message);
            }
        }
        //activates when bomb timer goes off
        private void bombTimeHandle(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("bomeTimeHandle");
            BombGoesOff();
        }

        //sets mute timer and send bomb explosion message to the channel
        public async void BombGoesOff()
        {
            Console.WriteLine("bombGoesOff");
            bombTime.Stop();
            //set mute timer between 30 and 120 seconds and message channel
            int muteSec = random.Next(30, 120);
            Mute(muteSec, currentGuild, user);
            await ReplyAsync($"**BOOOOOOOOM** {user.Mention} has been muted for {muteSec} seconds");
            
        }

        //mute user
        public async void Mute(int muteSec, IGuild guild, IUser user)
        {
            //Find mute role by its name and assign the role to the user
            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Muted");
            await (user as IGuildUser).AddRoleAsync(role);
            
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
            //find the mute role 
            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Muted");
            await (user as IGuildUser).RemoveRoleAsync(role);
            isBombSet = false;
            await ReplyAsync($"You may now speak {user.Mention}" );
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
            //sort the wires in alphabetical order
            Array.Sort(randomWires);
            //select the correct wire color
            correctWire = randomWires[correct];
            return randomWires;
        }
    }
}