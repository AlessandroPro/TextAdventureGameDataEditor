using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextBasedGameDataTool
{
    // Contains all data for a location, including its items, directions, and meta data
    class LocationData
    {
        public string LocationName { get; set; }
        public Location Location { get; set; }
        public SmallSpace SmallSpace { get; set; }
        public Guard Guard { get; set; }

        public SortedDictionary<string, Direction> Directions { get; set; }

        public SortedDictionary<string, Item> Items { get; set; }

        public LocationData(string locName)
        {
            LocationName = locName;
            Location = new Location(locName);
            Directions = new SortedDictionary<string, Direction>();
            Items = new SortedDictionary<string, Item>();
            Guard = new Guard();
            SmallSpace = new SmallSpace("small space");
        }

        public void changeName(string newName)
        {
            LocationName = newName;
            Location.Name = newName;
        }
    }
}
