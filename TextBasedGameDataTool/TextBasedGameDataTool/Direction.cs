using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextBasedGameDataTool
{
    class Direction : GameObject
    {
        public string ConnectedLocation { get; set; }
        public string BlockedDesc { get; set; }
        public string BlockMessage { get; set; }
        public string UnblockedDesc { get; set; }
        public string UnblockMessage { get; set; }
        public string PlayerCheckList { get; set; }
        public string LocCheckList { get; set; }
        public bool StartBlocked { get; set; }

        public Direction(string name)
        {
            Name = name;
            Type = "Direction";
            ConnectedLocation = "";
            BlockedDesc = "";
            UnblockedDesc = "";
            UnblockMessage = "";
            BlockMessage = "";
            PlayerCheckList = "";
            LocCheckList = "";
            StartBlocked = false;
        }
    }
}
