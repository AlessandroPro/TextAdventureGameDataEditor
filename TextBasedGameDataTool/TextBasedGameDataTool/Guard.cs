using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextBasedGameDataTool
{
    // Enemy game object
    class Guard : GameObject
    {
        public bool IsPlaced { get; set; }
        public Guard()
        {
            Name = "Guard";
            Type = "Guard";
            IsPlaced = false;
        }
    }
}
