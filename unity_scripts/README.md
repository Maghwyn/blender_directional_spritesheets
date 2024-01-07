## Motivation

This script streamlines the process of creating animations from spritesheets in Unity.
The primary goal is to save time and effort by automating the animation creation for a collection of spritesheets.

So instead of dragging and dropping each animations to the scene, renaming them and changing their location for the .anim that was created yadayada.
Instead the only thing you need to do is edit some properties, open the context menu and click on the function name, and voila.

## Running the script

![Video_Example](examples/example_unity_script.gif)

You will need to change the properties `mainFolder` and `outFolder` to your needs.
Might aswell change the `framesPerSecond` if it's too low.

WIP: Working on a better approach to includes/excludes animations from the tracking.

## Blender version

The script was tested and working on MacOS Sonoma 14.0 with Unity 2023.2.1