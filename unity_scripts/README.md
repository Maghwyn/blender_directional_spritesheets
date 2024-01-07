## Motivation

This script streamlines the process of creating animations from spritesheets in Unity.
The primary goal is to save time and effort by automating the animation creation for a collection of spritesheets.

So instead of dragging and dropping each animations to the scene, renaming them and changing their location for the .anim that was created yadayada.
Instead the only thing you need to do is edit some properties, open the context menu and click on the function name, and voila.

## Running the script

![](https://github.com/Maghwyn/blender_directional_spritesheets/blob/main/examples/Unity_Script_Use_Example.gif)

You will need to change the properties `mainFolder` and `outFolder` to your needs.

Once you got a correct path for the `mainFolder` property, the list of animation folders will be updated reactively. From there you can include/exclude the folder, specify if the animation are looping and adjust the `framesPerSecond`.

Example of the script in the editor from the last gif update.
![Unity_Script_SS](examples/Unity_Script_SS.png)

## Blender version

The script was tested and working on MacOS Sonoma 14.0 with Unity 2023.2.1
