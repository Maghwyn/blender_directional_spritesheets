## Source

This repository folder builds upon the [blender_scripts](https://github.com/FoozleCC/blender_scripts) project, offering additional features for my own convenient needs.

## Motivation

The primary script allows users to render a selected object from 8 directional angles.
I found it cumbersome to manually edit the script for each directional format when it cames to my workflow, so i just made it more convenient to use based on the required situation.

New features include:

- **4 Directional Angles:** Lowered rendering options for simple visual representation.
- **16 Directional Angles:** Expanded rendering options for more advanced visual representation.
- **32 Directional Angles:** Greater flexibility for detailed and nuanced presentations.
- **Isometric:** Moved the camera to fit the Directional Angles for iso.

## Running the script

For a full explanation on how to use the scripts, head over to FoozleCC [youtube video](https://www.youtube.com/watch?v=l1Io7fLYV4o).
You can find his base blender template over there at the bottom [foozlecc.itch.io](https://foozlecc.itch.io/render-4-or-8-direction-sprites-from-blender)

You will need to change the properties `OUT_FOLDER_PATH` and `ACTIONS_NAMES` to fit your OS/Actions names.
You can also optionally change `RENDER_RESOLUTION_X`, `RENDER_RESOLUTION_Y`, `RENDER_FRAME_FREQUENCY`, `ISOMETRIC`, `DESIRED_NUM_ANGLES` and `COMPASS_POINTS` based on your needs.

## Blender version

The script was tested and working on MacOS Sonoma 14.0 with Blender 4.0.2