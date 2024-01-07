## Source

This repository folder builds upon the [blender_scripts](https://github.com/FoozleCC/blender_scripts) project, offering additional features for my own convenient needs.

## Motivation

The primary script allows users to render a selected object from 8 directional angles.
I found it cumbersome to manually edit the script for each directional format when it cames to my workflow, so i just made it more convenient to use based on the required situation.

New features include:

- **4 Directional Angles:** Lowered rendering options for simple visual representation.
- **16 Directional Angles:** Expanded rendering options for more advanced visual representation.
- **32 Directional Angles:** Greater flexibility for detailed and nuanced presentations.

## Running the script

For a full explanation on how to use the scripts, head over to FoozleCC [youtube video](https://www.youtube.com/watch?v=l1Io7fLYV4o).

You will need to change the properties `OUT_FOLDER_PATH` and `ACTIONS_NAMES` to fit your OS/Actions names.
You can also optionally change `RENDER_RESOLUTION_X`, `RENDER_RESOLUTION_Y` and `RENDER_FRAME_FREQUENCY` based on your needs.
I wouldn't recommand changing `_ANGLES` unless your requirements does not fit neither 4,8,16 and 32 angles.
I wouldn't recommand changing `_ANGLES_DIR` unless you find better directories name for your angles.

TODO: Rework the script to be a all-in-one, lazy rn

## Blender version

Each scripts were tested and working on MacOS Sonoma 14.0 with Blender 4.0.2