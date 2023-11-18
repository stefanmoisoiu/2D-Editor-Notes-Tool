# 2D Editor Notes Tool
> Create notes in the scene view as a way to keep track of your level design, communicate with your team, or just to remember what you were doing last time you opened the project.

## Features
- Create notes in the editor
- Save and load notes to a file
- Create notes specific to a level
- Customize notes (size, color)

## Installation
1. Download the files from the latest release page
2. Add them to your project
3. Done!

## Usage
1. Create a new Level Note : `Right click in project > Notes > Level Note`
2. Open the Notes Window : `Window > Notes`
3. Select the Level Note you want to edit in the Notes Window.

#### Creating a note
- Add a note by clicking on the `+` button.

#### Editing a note
- Edit the note's properties in the top left corner of the Scene View.

#### Deleting a note
- Delete a note by clicking on the `-` button.

## Common issues
#### The notes are not visible in the Scene View
- Make sur the Notes Window is open .
- Make sure you have selected the level note in the Notes Window.
- Make sure there are notes in the level note (Click on the level note in your project, if there are no notes in the list, add them manually).
#### I can't create a note
- Make sure you have selected the level note in the Notes Window.
- Make sure you have not selected a note in the Scene View.
#### I can't delete a note
- Make sure you have selected the level note in the Notes Window.
- Make sure you have selected a note in the Scene View.

## Code structure
#### If you want to expand the tool, here is a quick overview of the code structure
- `Notes.cs` : The main class of the tool, it contains the level note and handles the window, displaying the notes in the Scene View, selecting, adding and deleting them.
- `ScriptableNote.cs` : A note, it contains the data of a note (position, size, color, text).
- `ScriptableLevelNote.cs` : A level note, it contains a list of notes.
## License
This project is licensed under the MIT License - see the [license](LICENSE.txt) file for details.