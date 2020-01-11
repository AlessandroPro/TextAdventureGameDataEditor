using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextBasedGameDataTool
{
    // Represents a all information that will be loaded into the game
    class Game
    {
        public SortedDictionary<string, LocationData> Locations { get; set; }
        public SortedDictionary<string, Item> Items { get; set; }
        public string StartLocation { get; set; }
        public string EndLocation { get; set; }
        public string StartMessage { get; set; }
        public string EndMessage { get; set; }

        public GameSaveData SaveData { get; set; }

        public Game()
        {
            Locations = new SortedDictionary<string, LocationData>();
            Items = new SortedDictionary<string, Item>();
            StartLocation = "";
            EndLocation = "";
            StartMessage = "";
            EndMessage = "";
            SaveData = new GameSaveData();
        }
    }
}
