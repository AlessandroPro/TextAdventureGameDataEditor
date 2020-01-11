using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextBasedGameDataTool
{
    // This object is constructed from the Game instance's data and then serialized to then be loaded into the game
    // It basically grabs all of the Game Instance's game objects and stores them in one large list
    class GameSaveData
    {
        public string StartLocation { get; set; }
        public string EndLocation { get; set; }
        public string StartMessage { get; set; }
        public string EndMessage { get; set; }
        public List<GameObject> GameObjects { get; set; }

        public GameSaveData()
        {
            StartLocation = "";
            EndLocation = "";
            StartMessage = "";
            EndMessage = "";
            GameObjects = new List<GameObject>();
        }

        public void clear()
        {
            StartLocation = "";
            EndLocation = "";
            StartMessage = "";
            EndMessage = "";
            GameObjects.Clear();
        }
    }
}
