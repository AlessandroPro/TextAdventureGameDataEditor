using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextBasedGameDataTool
{
    // A type of item that can be placed somewhere and picked up by the player
    class Clothing : Item
    {
        public Clothing(string name)
            : base(name)
        {
            Type = "Clothing";
        }
    }
}
