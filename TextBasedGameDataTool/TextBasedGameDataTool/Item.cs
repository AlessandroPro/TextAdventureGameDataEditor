
namespace TextBasedGameDataTool
{
    // An item that can be placed somewhere and picked up by the player
    class Item : GameObject
    {
        public string InaccessibleDesc { get; set; }
        public string InaccessMessage { get; set; }
        public string AccessMessage { get; set; }
        public bool StartWithoutAccess { get; set; }
        public string PlayerCheckList { get; set; }
        public string LocCheckList { get; set; }
        public bool IsPlaced { get; set; }
        public string Owner { get; set; }

        public Item(string name)
        {
            Name = name;
            Type = "Item";
            IsPlaced = false;
            Owner = "Location";
            InaccessibleDesc = "";
            InaccessMessage = "";
            AccessMessage = "";
            StartWithoutAccess = false;
            LocCheckList = "";
            PlayerCheckList = "";
        }
    }
}
