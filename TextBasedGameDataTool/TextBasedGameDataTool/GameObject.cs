using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextBasedGameDataTool
{
    // Base representation of a game object 
    class GameObject
    {
        public string Name { get; set; }

        public string LocationName { get; set; }

        public string Type { get; set; }

        public GameObject()
        {
            Name = "NO NAME";
            LocationName = "NO LOCATION";
            Type = "GameObject";
        }
    }
}
