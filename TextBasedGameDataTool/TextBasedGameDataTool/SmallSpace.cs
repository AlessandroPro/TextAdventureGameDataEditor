using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextBasedGameDataTool
{
    // A small space game object, which is something that can be placed in the game
    class SmallSpace : GameObject
    {
        public bool IsPlaced { get; set; }

        public SmallSpace(string name)
        {
            Name = name;
            Type = "SmallSpace";
            IsPlaced = false;
        }
    }
}
