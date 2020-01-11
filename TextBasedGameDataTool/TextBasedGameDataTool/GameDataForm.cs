using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace TextBasedGameDataTool
{
    public partial class GameDataForm : Form
    {
        private Game gameData;
        private LocationData locationData;
        private Direction direction;
        private Item item;
        private List<Item> availableItems;

        private LocationData emptyLocationData;
        private Direction emptyDirection;
        private Item emptyItem;

        private BindingSource gameDataSource;
        private BindingSource locationDataSource;
        private BindingSource smallSpaceDataSource;
        private BindingSource directionDataSource;
        private BindingSource itemDataSource;

        public GameDataForm()
        {
            InitializeComponent();

            gameDataSource = new BindingSource { DataSource = typeof(Game) };
            locationDataSource = new BindingSource { DataSource = typeof(Location) };
            smallSpaceDataSource = new BindingSource { DataSource = typeof(SmallSpace) };
            directionDataSource = new BindingSource { DataSource = typeof(Direction) };
            itemDataSource = new BindingSource { DataSource = typeof(Item) };

            gameData = new Game();
            availableItems = new List<Item>();
            emptyLocationData = new LocationData("");
            emptyDirection = new Direction("");
            emptyItem = new Item("");

            gameDataSource.DataSource = gameData;

            updateAvailableItems();
            setBindings();

            setLocationControls();
            setDirectionControls();
            setItemControls();
        }

        // Bind all of the necessary control data to the specified object data
        private void setBindings()
        {
            // Meta Game properties
            startLocationComboBox.DataBindings.Add("Text", gameDataSource, "StartLocation", false, DataSourceUpdateMode.OnPropertyChanged);
            endLocationComboBox.DataBindings.Add("Text", gameDataSource, "EndLocation", false, DataSourceUpdateMode.OnPropertyChanged);
            startMessageTextBox.DataBindings.Add("Text", gameDataSource, "StartMessage", false, DataSourceUpdateMode.OnPropertyChanged);
            endMessageTextBox.DataBindings.Add("Text", gameDataSource, "EndMessage", false, DataSourceUpdateMode.OnPropertyChanged);

            // Location properties
            locationDescriptionText.DataBindings.Add("Text", locationDataSource, "Description", false, DataSourceUpdateMode.OnPropertyChanged);
            smallSpaceNameText.DataBindings.Add("Text", smallSpaceDataSource, "Name", false, DataSourceUpdateMode.OnPropertyChanged);

            // Direction properties
            directionStartBlockedCheckBox.DataBindings.Add("Checked", directionDataSource, "StartBlocked", false, DataSourceUpdateMode.OnPropertyChanged);
            directionUnblockedDescText.DataBindings.Add("Text", directionDataSource, "UnblockedDesc", false, DataSourceUpdateMode.OnPropertyChanged);
            directionBlockedDescText.DataBindings.Add("Text", directionDataSource, "BlockedDesc", false, DataSourceUpdateMode.OnPropertyChanged);
            directionBlockMsgTextBox.DataBindings.Add("Text", directionDataSource, "BlockMessage", false, DataSourceUpdateMode.OnPropertyChanged);
            directionUnblockMsgTextBox.DataBindings.Add("Text", directionDataSource, "UnblockMessage", false, DataSourceUpdateMode.OnPropertyChanged);
            directionCheckLocTextBox.DataBindings.Add("Text", directionDataSource, "LocCheckList", false, DataSourceUpdateMode.OnPropertyChanged);
            directionCheckPlayerTextBox.DataBindings.Add("Text", directionDataSource, "PlayerCheckList", false, DataSourceUpdateMode.OnPropertyChanged);

            // Item properties
            itemStartInaccessibleCheckBox.DataBindings.Add("Checked", itemDataSource, "StartWithoutAccess", false, DataSourceUpdateMode.OnPropertyChanged);
            itemInaccessibleDescText.DataBindings.Add("Text", itemDataSource, "InaccessibleDesc", false, DataSourceUpdateMode.OnPropertyChanged);
            itemAccessMsgTextBox.DataBindings.Add("Text", itemDataSource, "AccessMessage", false, DataSourceUpdateMode.OnPropertyChanged);
            itemInaccessMsgTextBox.DataBindings.Add("Text", itemDataSource, "InaccessMessage", false, DataSourceUpdateMode.OnPropertyChanged);
            itemCheckLocTextBox.DataBindings.Add("Text", itemDataSource, "LocCheckList", false, DataSourceUpdateMode.OnPropertyChanged);
            itemCheckPlayerTextBox.DataBindings.Add("Text", itemDataSource, "PlayerCheckList", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        // Hides location and direction controls if there are 0 locations
        private void setLocationControls()
        {
            if(gameData.Locations.Count == 0)
            {
                locationPanel.Hide();
                newDirectionButton.Enabled = false;
                deleteLocationButton.Enabled = false;
                locationData = emptyLocationData;
                locationDataSource.DataSource = locationData.Location;
            }
            else
            {
                locationPanel.Show();
                newDirectionButton.Enabled = true;
                deleteLocationButton.Enabled = true;
            }
        }

        // Hides direction controls if there are 0 directons for a particular location
        private void setDirectionControls()
        {
            if (locationData.Directions.Count == 0)
            {
                directionPanel.Hide();
                direction = emptyDirection;
                deleteDirectionButton.Enabled = false;
                directionDataSource.DataSource = direction;
            }
            else
            {
                directionPanel.Show();
                deleteDirectionButton.Enabled = true;
            }
        }

        // Hides item controls if there are 0 items
        private void setItemControls()
        {
            if (gameData.Items.Count == 0)
            {
                itemPanel.Hide();
                item = emptyItem;
                deleteItemButton.Enabled = false;
                itemDataSource.DataSource = item;
            }
            else
            {
                itemPanel.Show();
                deleteItemButton.Enabled = true;
            }
        }

        private void locationComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LocationData selectedLocation = locationComboBox.SelectedItem as LocationData;

            if (selectedLocation == null)
            {
                return;
            }

            locationData = selectedLocation;
            locationDataSource.DataSource = locationData.Location;

            locationNameText.Text = locationData.LocationName;
            locationDescriptionText.Text = locationData.Location.Description;

            guardCheckBox.Checked = locationData.Guard.IsPlaced;
            smallSpaceCheckBox.Checked = locationData.SmallSpace.IsPlaced;

            smallSpaceDataSource.DataSource = locationData.SmallSpace;
            smallSpaceNameText.Text = locationData.SmallSpace.Name;

            rebindDirections();
            rebindListBox();
        }

        private void deleteLocationButton_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure to delete this Location?\n\n" + locationData.LocationName, "Confirm delete.", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                // Delete the location and rebind all of its control references
                MessageBox.Show(locationData.LocationName + " deleted.", "", MessageBoxButtons.OK);
                gameData.Locations.Remove(locationData.LocationName);
                updateLocationNames(locationData.LocationName, "");
                rebindLocations();
            }
        }

        // Saves all of the game's data in two files:
        // First file: serializes the Game instance into JSON for easy reloading into the editor
        // Second file: Extracts all of the Game instance's game objects and lists them in the GameSaveData instance, which 
        // is then serialized for each game object loading in the game itself
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gameData.SaveData.clear();

            GameSaveData saveData = gameData.SaveData;

            saveData.StartLocation = gameData.StartLocation;
            saveData.EndLocation = gameData.EndLocation;
            saveData.StartMessage = gameData.StartMessage;
            saveData.EndMessage = gameData.EndMessage;


            foreach (LocationData location in gameData.Locations.Values)
            {
                saveData.GameObjects.Add(location.Location);
            }

            foreach (LocationData location in gameData.Locations.Values)
            {
                foreach (Direction direction in location.Directions.Values)
                {
                    direction.LocationName = location.LocationName;
                    saveData.GameObjects.Add(direction);
                }

                if (location.SmallSpace.IsPlaced)
                {
                    location.SmallSpace.LocationName = location.LocationName;
                    saveData.GameObjects.Add(location.SmallSpace);
                }

                if (location.Guard.IsPlaced)
                {
                    location.Guard.LocationName = location.LocationName;
                    saveData.GameObjects.Add(location.Guard);
                }

                foreach (Item item in location.Items.Values)
                {
                    item.LocationName = location.LocationName;
                    saveData.GameObjects.Add(item);
                }
            }

            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };


            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string gameFileName = Path.GetDirectoryName(saveFileDialog.FileName) + "\\" + Path.GetFileNameWithoutExtension(saveFileDialog.FileName).Split('.')[0] + ".game.json";

                    Console.WriteLine(gameFileName);
                    StreamWriter writerEditor = new StreamWriter(gameFileName, false);
                    writerEditor.WriteLine(JsonConvert.SerializeObject(saveData, Newtonsoft.Json.Formatting.Indented));
                    writerEditor.Close();

                    saveData.clear();

                    StreamWriter writerGame = new StreamWriter(saveFileDialog.FileName, false);
                    writerGame.WriteLine(JsonConvert.SerializeObject(gameData, settings));
                    writerGame.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
        }

        // Loads from an editor.json file by eserializin it into a Game instance
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var sr = new StreamReader(openFileDialog.FileName);
                    string fileData = sr.ReadToEnd();
                    gameData = JsonConvert.DeserializeObject<Game>(fileData, settings);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }

            gameDataSource.DataSource = gameData;
            startMessageTextBox.Text = gameData.StartMessage;
            endMessageTextBox.Text = gameData.EndMessage;
            string startLoc = gameData.StartLocation;
            string endLoc = gameData.EndLocation;


            rebindLocations();
            rebindItems();
            updateAvailableItems();

            if (gameData.Locations.ContainsKey(startLoc))
            {
                startLocationComboBox.SelectedIndex = startLocationComboBox.FindStringExact(startLoc);
            }

            if (gameData.Locations.ContainsKey(endLoc))
            {
                endLocationComboBox.SelectedIndex = endLocationComboBox.FindStringExact(endLoc);
            }
        }

        // Adds a new location and rebinds the references to the map of locations
        private void newLocationButton_Click(object sender, EventArgs e)
        {

            InputPromptForm prompt = new InputPromptForm();
            prompt.label.Text = "Enter a new location name:";

            // Show prompt as a modal dialog and determine if DialogResult = OK.
            if (prompt.ShowDialog(this) == DialogResult.OK)
            {
                string newLocationName = prompt.textBox.Text;
                if(newLocationName.Length > 0 && !gameData.Locations.ContainsKey(newLocationName))
                {
                    LocationData loc = new LocationData(newLocationName);
                    gameData.Locations.Add(loc.LocationName, loc);

                    rebindLocations();

                    locationComboBox.SelectedIndex = locationComboBox.FindStringExact(loc.LocationName);
                }
                else
                {
                    errorLabel.Text = "ERROR: Invalid name for the new location. Make sure it is a non-empty, unique name.";
                }
            }
            prompt.Dispose();
        }

        private void smallSpaceCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if(smallSpaceCheckBox.Checked)
            {
                hideSmallSpaceRadioButton.Enabled = true;
                smallSpaceNameText.Enabled = true;
                locationData.SmallSpace.IsPlaced = true;
            }
            else
            {
                foreach (Item item in locationData.Items.Values)
                {
                    if (item.Owner.Equals("SmallSpace"))
                    {
                        item.Owner = "Location";
                    }
                }

                hideSmallSpaceRadioButton.Enabled = false;

                smallSpaceNameText.Enabled = false;
                locationData.SmallSpace.IsPlaced = false;
            }
        }

        // Updates the string value of variables that store a location's name
        // Called whenever a locationData's name changes or the locationData is deleted
        private void updateLocationNames(string oldName, string newName)
        {
            foreach(LocationData loc in gameData.Locations.Values)
            {
                foreach(Direction direction in loc.Directions.Values)
                {
                    if(direction.ConnectedLocation.Equals(oldName))
                    {
                        direction.ConnectedLocation = newName;
                    }
                }
            }
        }

        // Rebinds the combobox to the map of game locations
        private void rebindLocations(ComboBox comboBox)
        {
            LocationData loc = comboBox.SelectedItem as LocationData;
            BindingSource bs = comboBox.DataSource as BindingSource;

            if(bs != null)
            {
                bs.Clear();
            }
            comboBox.DataSource = new BindingSource(gameData.Locations.Values, null);
            comboBox.DisplayMember = "LocationName";

            if (loc != null)
            {
                if (gameData.Locations.ContainsKey(loc.LocationName))
                {
                    comboBox.SelectedIndex = comboBox.FindStringExact(loc.LocationName);
                }
            }
        }

        //rebinds all combo boxes to the map of game locations
        private void rebindLocations()
        {
            rebindLocations(locationComboBox);
            rebindLocations(directionConnectionComboBox);
            rebindLocations(startLocationComboBox);
            rebindLocations(endLocationComboBox);

            rebindDirections();
            updateAvailableItems();
            rebindListBox();

            setLocationControls();
            setDirectionControls();
        }

        // Ensures that when the name of the location is changed, that the name is unique and that it's changes are permeated throughout all data
        private void locationNameText_Leave(object sender, EventArgs e)
        {
            errorLabel.Text = "";

            if (gameData.Locations.Count > 0)
            {
                if (gameData.Locations.ContainsKey(locationNameText.Text))
                {
                    if (gameData.Locations[locationNameText.Text] != locationData)
                    {
                        errorLabel.Text = "Error: Can't have two locations with the same name. Enter another name.";
                        locationComboBox.SelectedIndex = locationComboBox.FindStringExact(locationData.LocationName);
                        locationComboBox.Enabled = false;
                        return;
                    }
                }
                else
                {
                    string newName = locationNameText.Text;
                    gameData.Locations.Remove(locationData.LocationName);
                    updateLocationNames(locationData.LocationName, newName);
                    locationData.changeName(newName);

                    gameData.Locations.Add(locationData.LocationName, locationData);

                    rebindLocations();
                }
            }
            locationComboBox.Enabled = true;
        }

        private void newDirectionButton_Click(object sender, EventArgs e)
        {
            InputPromptForm prompt = new InputPromptForm();
            prompt.label.Text = "Enter a new direction name for this location:";

            // Show prompt as a modal dialog and determine if DialogResult = OK.
            if (prompt.ShowDialog(this) == DialogResult.OK)
            {
                string newDirectionName = prompt.textBox.Text;
                if (newDirectionName.Length > 0 && !locationData.Directions.ContainsKey(newDirectionName))
                {
                    Direction direction = new Direction(newDirectionName);
                    direction.LocationName = locationData.LocationName;
                    locationData.Directions.Add(direction.Name, direction);

                    rebindDirections();

                    directionComboBox.SelectedIndex = directionComboBox.FindStringExact(direction.Name);

                }
                else
                {
                    errorLabel.Text = "ERROR: Invalid name for the new direction. Make sure it is non-empty and unique in this location.";
                }
            }
            prompt.Dispose();
        }

        private void rebindDirections()
        {
            BindingSource bs = directionComboBox.DataSource as BindingSource;

            if (bs != null)
            {
                (directionComboBox.DataSource as BindingSource).Clear();
            }

            directionComboBox.DataSource = new BindingSource(locationData.Directions.Values, null);
            directionComboBox.DisplayMember = "Name";

            setDirectionControls();
        }

        private void directionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Direction selectedDirection = directionComboBox.SelectedItem as Direction;

            if (selectedDirection == null)
            {
                return;
            }

            direction = selectedDirection;

           if (gameData.Locations.ContainsKey(direction.ConnectedLocation))
           {
                directionConnectionComboBox.SelectedIndex = directionConnectionComboBox.FindStringExact(direction.ConnectedLocation);
           }
           else
           {
                LocationData selectedLocation = directionConnectionComboBox.SelectedItem as LocationData;
                if (selectedLocation == null)
                {
                    selectedDirection.ConnectedLocation = "";
                }
                else
                {
                    selectedDirection.ConnectedLocation = selectedLocation.LocationName;
                }
            }

            directionDataSource.DataSource = direction;

            directionNameText.Text = direction.Name;
            directionStartBlockedCheckBox.Checked = direction.StartBlocked;
            directionUnblockedDescText.Text = direction.UnblockedDesc;
            directionBlockedDescText.Text = direction.BlockedDesc;
            directionBlockMsgTextBox.Text = direction.BlockMessage;
            directionUnblockMsgTextBox.Text = direction.UnblockMessage;
            directionCheckLocTextBox.Text = direction.LocCheckList;
            directionCheckPlayerTextBox.Text = direction.PlayerCheckList;
        }

        private void deleteDirectionButton_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure to delete this direction?\n\n" + direction.Name, "Confirm delete.", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                MessageBox.Show(direction.Name + " deleted from " + locationData.LocationName + ".", "", MessageBoxButtons.OK);
                locationData.Directions.Remove(direction.Name);
                rebindDirections();
            }
        }

        private void directionNameText_Leave(object sender, EventArgs e)
        {
            errorLabel.Text = "";

            if (locationData.Directions.Count > 0)
            {
                if (locationData.Directions.ContainsKey(directionNameText.Text))
                {
                    if (locationData.Directions[directionNameText.Text] != direction)
                    {
                        errorLabel.Text = "Error: Can't have two directions with the same name in the same location. Enter another name.";
                        directionComboBox.SelectedIndex = directionComboBox.FindStringExact(direction.Name);
                        directionComboBox.Enabled = false;
                        return;
                    }
                }
                else
                {
                    string newName = directionNameText.Text;
                    locationData.Directions.Remove(direction.Name);
                    direction.Name = newName;
                    locationData.Directions.Add(direction.Name, direction);

                    rebindDirections();
                    directionComboBox.SelectedIndex = directionComboBox.FindStringExact(newName);
                }
            }
            directionComboBox.Enabled = true;
        }

        private void newItemButton_Click(object sender, EventArgs e)
        {
            InputPromptForm prompt = new InputPromptForm();
            prompt.label.Text = "Enter a unique name for this new item:";
            prompt.isClothingRadioButton.Show();

            // Show prompt as a modal dialog and determine if DialogResult = OK.
            if (prompt.ShowDialog(this) == DialogResult.OK)
            {
                string newItemName = prompt.textBox.Text;
                if (newItemName.Length > 0 && !gameData.Items.ContainsKey(newItemName))
                {
                    Item item;
                    if(prompt.isClothingRadioButton.Checked)
                    {
                        item = new Clothing(newItemName);
                    }
                    else
                    {
                        item = new Item(newItemName);
                    }

                    if (item != null)
                    {
                        gameData.Items.Add(item.Name, item);

                        rebindItems();

                        itemComboBox.SelectedIndex = itemComboBox.FindStringExact(item.Name);
                        updateAvailableItems();
                    }
                }
                else
                {
                    errorLabel.Text = "ERROR: Invalid name for the new item. Make sure it is a non-empty, unique name.";
                }
            }
            prompt.isClothingRadioButton.Hide();
            prompt.Dispose();
        }

        private void rebindItems()
        {
            BindingSource bs = itemComboBox.DataSource as BindingSource;

            if (bs != null)
            {
                (itemComboBox.DataSource as BindingSource).Clear();
            }

            itemComboBox.DataSource = new BindingSource(gameData.Items.Values, null);
            itemComboBox.DisplayMember = "Name";

            setItemControls();
        }

        private void deleteItemButton_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure to delete this item from the world?\n\n" + item.Name, "Confirm delete.", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                MessageBox.Show(item.Name + " deleted.", "", MessageBoxButtons.OK);
                foreach (LocationData loc in gameData.Locations.Values)
                {
                    loc.Items.Remove(item.Name);
                }
                gameData.Items.Remove(item.Name);
                rebindItems();
                rebindListBox();
                updateAvailableItems();
            }
        }

        private void itemComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Item selectedItem = itemComboBox.SelectedItem as Item;

            if (selectedItem == null)
            {
                return;
            }

            item = selectedItem;
            itemDataSource.DataSource = item;
            itemNameText.Text = item.Name;

            itemStartInaccessibleCheckBox.Checked = item.StartWithoutAccess;
            itemInaccessibleDescText.Text = item.InaccessibleDesc;
            itemAccessMsgTextBox.Text = item.AccessMessage;
            itemInaccessMsgTextBox.Text = item.InaccessMessage;
            itemCheckLocTextBox.Text = item.LocCheckList;
            itemCheckPlayerTextBox.Text = item.PlayerCheckList;
        }

        private void itemNameText_Leave(object sender, EventArgs e)
        {
            errorLabel.Text = "";

            if (gameData.Items.Count > 0)
            {
                if (gameData.Items.ContainsKey(itemNameText.Text))
                {
                    if (gameData.Items[itemNameText.Text] != item)
                    {
                        errorLabel.Text = "Error: Can't have two items with the same name. Enter another name.";
                        itemComboBox.SelectedIndex = itemComboBox.FindStringExact(item.Name);
                        itemComboBox.Enabled = false;
                        return;
                    }
                }
                else
                {
                    string newName = itemNameText.Text;
                    gameData.Items.Remove(item.Name);
                    item.Name = newName;
                    gameData.Items.Add(item.Name, item);

                    foreach (LocationData loc in gameData.Locations.Values)
                    {
                        if(loc.Items.ContainsKey(item.Name))
                        {
                            loc.Items.Remove(item.Name);
                            loc.Items.Add(newName, item);

                        }
                    }

                    rebindItems();
                    rebindListBox();
                    updateAvailableItems();
                    itemComboBox.SelectedIndex = itemComboBox.FindStringExact(newName);
                }
            }
            itemComboBox.Enabled = true;
        }
        private void locationAddItemButton_Click(object sender, EventArgs e)
        {
            Item selectedItem = addItemLocationComboBox.SelectedItem as Item;
            if(selectedItem != null)
            {
                locationData.Items.Add(selectedItem.Name, selectedItem);
                selectedItem.IsPlaced = true;
                rebindListBox();
                updateAvailableItems();

                if(selectedItem.Owner.Equals("Location"))
                {
                    hideDontRadioButton.Checked = true;
                }
                else if (selectedItem.Owner.Equals("Guard"))
                {
                    hideGuardRadioButton.Checked = true;
                }
                else if (selectedItem.Owner.Equals("SmallSpace"))
                {
                    hideSmallSpaceRadioButton.Checked = true;
                }
            }
        }
        private void updateAvailableItems()
        {
            availableItems.Clear();

            foreach (Item item in gameData.Items.Values)
            {
                if(!item.IsPlaced)
                {
                    availableItems.Add(item);
                }
            }

            BindingSource bs = new BindingSource { DataSource = availableItems };
            addItemLocationComboBox.DisplayMember = "Name";
            addItemLocationComboBox.DataSource = bs;
        }

        private void rebindListBox()
        {
            BindingSource bs = locationItemListBox.DataSource as BindingSource;

            if (bs != null)
            {
                (locationItemListBox.DataSource as BindingSource).Clear();
            }
            locationItemListBox.DataSource = new BindingSource(locationData.Items.Values, null);
            locationItemListBox.DisplayMember = "Name";
        }

        private void locationRemoveItemButton_Click(object sender, EventArgs e)
        {
            Item selectedItem = locationItemListBox.SelectedItem as Item;
            if (selectedItem != null)
            {
                locationData.Items.Remove(selectedItem.Name);
                selectedItem.IsPlaced = false;
                selectedItem.Owner = "Location";
                rebindListBox();
                updateAvailableItems();
            }
        }

        private void guardCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if(guardCheckBox.Checked)
            {
                hideGuardRadioButton.Enabled = true;
                locationData.Guard.IsPlaced = true;
                smallSpaceCheckBox.Checked = true;
            }
            else
            {
                foreach (Item item in locationData.Items.Values)
                {
                    if (item.Owner.Equals("Guard"))
                    {
                        item.Owner = "Location";
                    }
                }

                hideGuardRadioButton.Enabled = false;
                locationData.Guard.IsPlaced = false;
            }
        }

        private void hideGuardRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if(hideGuardRadioButton.Checked)
            {
                Item item = locationItemListBox.SelectedItem as Item;
                if (item != null)
                {
                    item.Owner = "Guard";
                }
            }
        }

        private void hideSmallSpaceRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (hideSmallSpaceRadioButton.Checked)
            {
                Item item = locationItemListBox.SelectedItem as Item;
                if (item != null)
                {
                    item.Owner = "SmallSpace";
                }
            }
        }

        private void hideDontRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (hideDontRadioButton.Checked)
            {
                Item item = locationItemListBox.SelectedItem as Item;
                if (item != null)
                {
                    item.Owner = "Location";
                }
            }
        }

        private void DirectionConnectionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Direction selectedDirection = directionComboBox.SelectedItem as Direction;
            LocationData selectedLocation = directionConnectionComboBox.SelectedItem as LocationData;

            if (selectedDirection == null)
            {
                return;
            }

            if(selectedLocation == null)
            {
                selectedDirection.ConnectedLocation = "";
            }
            else
            {
                selectedDirection.ConnectedLocation = selectedLocation.LocationName;
            }
        }
    }
}
