using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Zork
{
    public class Game : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public World World { get; private set; }

        public string StartingLocation { get; set; }
        
        public string WelcomeMessage { get; set; }
        
        public string ExitMessage { get; set; }

        [JsonIgnore]
        public Player Player { get; private set; }

        [JsonIgnore]
        public bool IsRunning { get; set; }

        [JsonIgnore]
        public Dictionary<string, Command> Commands { get; private set; }

        [JsonIgnore]
        public IOutputService Output { get; set; }

        public IInputService Input { get; set; }

        public Game(World world, Player player)
        {
            World = world;
            Player = player;

            Commands = new Dictionary<string, Command>()
            {
                { "QUIT", new Command("QUIT", new string[] { "QUIT", "Q", "BYE" }, game => game.IsRunning = false) },
                { "LOOK", new Command("LOOK", new string[] { "LOOK", "L" }, game =>
                {
                    Output.WriteLine(game.Player.Location, isBold: true);
                    Output.WriteLine(game.Player.Location.Description);
                })},
                { "NORTH", new Command("NORTH", new string[] { "NORTH", "N" }, game => Move(game, Directions.North)) },
                { "SOUTH", new Command("SOUTH", new string[] { "SOUTH", "S" }, game => Move(game, Directions.South)) },
                { "EAST", new Command("EAST", new string[] { "EAST", "E"}, game => Move(game, Directions.East)) },
                { "WEST", new Command("WEST", new string[] { "WEST", "W" }, game => Move(game, Directions.West)) },
                { "SCORE", new Command("SCORE", new string[] {"SCORE"}, game => Output.WriteLine($"Your score would be {game.Player.Score}, in {game.Player.Moves} move(s).")) },
                { "REWARD", new Command("REWARD", new string[] {"REWARD"}, game => game.Player.Score++) },
            };
        }

        public void Initialize(IInputService inputService, IOutputService outputService)
        {
            Assert.IsNotNull(inputService);
            Input = inputService;
            Input.InputReceived += InputReceivedHandler;

            Assert.IsNotNull(outputService);
            Output = outputService;
            
            Output.WriteLine(string.IsNullOrWhiteSpace(WelcomeMessage) ? "Welcome to Zork!" : WelcomeMessage);
            IsRunning = true;
        }

        private void InputReceivedHandler(object sender, string inputString)
        {
            Command foundCommand = null;
            foreach (Command command in Commands.Values)
            {
                if (command.Verbs.Contains(inputString.Trim(), StringComparer.InvariantCultureIgnoreCase))
                {
                    foundCommand = command;
                    break;
                }
            }

            if (foundCommand != null)
            {
                Player.Moves++;
                Room previousRoom = Player.Location;
                foundCommand.Action(this);

                if (previousRoom != Player.Location)
                {
                    Commands["LOOK"].Action(this);
                }
            }
            else
            {
                Output.WriteLine("Unknown command.");
            }
        }

        public void ShutDown()
        {
            Output.WriteLine(string.IsNullOrWhiteSpace(ExitMessage) ? "Thank you for playing!" : ExitMessage);
        }

        private static void Move(Game game, Directions direction)
        {
            if (game.Player.Move(direction) == false)
            {
                game.Output.WriteLine("The way is shut!");
            }
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) => Player = new Player(World, StartingLocation);
    }
}