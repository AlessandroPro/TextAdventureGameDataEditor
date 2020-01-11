using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextBasedGameDataTool
{
    // Location (room) in the game map
    class Location : GameObject
    {
        public string Description { get; set; }

        public Location(string name)
        {
            Name = name;
            Type = "Location";
            Description = "";
        }
    }
}
