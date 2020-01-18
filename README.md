# Text Adventure Game Data Editor Tool

All relevant C# code is in /TextBasedGameDataTool/TextBasedGameDataTool/. 

This is a C# Winform application written to support this text-based adventure game: <br>
https://github.com/AlessandroPro/BearlyMadeItOut.git

This tool is used to write all of the information used in the game, and export it to JSON for easy loading into the game. <br>
The tool itself also supports saving and loading, so that any work within it can be stopped and continued at a later time.

### Overview

This tool allows the user to:

* Add and edit locations (name, description, items, if they have a small space, guard, etc.)
* Add and edit directions for each location (name, description, connected to location, etc.)
* Add and edit items (Name, etc.)

The text-base adventure game was designed to have unique location names and item names. Each location has it's own set
of directions, which are uniquely named within the location.
The items are global, so there can't be more than one with the same name. 
So, items can be created, and then added to their respective locations after.
This tool ensures these constraints.

Note: The game utilizes ItemCheck instances, which are basically given to directions and items to determine conditions.
The parameters for these conditions can also be edited in this tool. Examples: if a direction is blocked,
what message is shown when blocked, what items are required to unblock it, etc.

### SAVING AND LOADING in the tool:

File->Save opens a file dialog box. Name the save file exactly as 'newGameData' in the game's project directory under /TextAdventureGame/Build/Assets/SaveData. 
This is where the game itself will load from, which is ../Assets/SaveData/ relative to the game's working directory.

Saving will create two json files with the same name, and different extensions, in the same place: <br>
* newGameData.editor.json -> use this to load into the editor using File->Load
* newGameData.game.json -> the game will load this exact filename

Note: If you want to just save and load editor files, you can put them wherever you want.
The above instructions are only if you want the editor save files to load properly into the game.

Screenshot of the tool:
![GameDataTool_Screenshot](https://user-images.githubusercontent.com/15040875/72210060-d0ffb300-3483-11ea-8757-f0541e5de785.PNG)
