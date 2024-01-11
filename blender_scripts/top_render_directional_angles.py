import bpy
import os
import math

## PROPERTIES

# Set the folder output path
OUT_FOLDER_PATH = '/Users/_YourUsername_/Desktop/Blender/Sprites/Frames/Character' # Change the path to your desired output folder

# Set the desired actions names
ACTIONS_NAMES = ["Idle", "Running", "Dying"] #! Make sure to use the exact name of the action!

# Set to whatever you want!
RENDER_RESOLUTION_X = 256
RENDER_RESOLUTION_Y = 256
RENDER_FRAME_FREQUENCY = 2
#! Make sure it's either 4, 8, 16 or 32
DESIRED_NUM_ANGLES = 4 

# If you need to edit the clock direction or move N to be angle 0, feel free to do so.
# Just be sure to have the correct points at the correct position
COMPASS_POINTS = [ #! 32-point compass counterclockwise
	"E", "EbN", "ENE", "NEbE", "NE", "NEbN", "NNE", "NbE",
	"N", "NbW", "NNW", "NWbN", "NW", "NWbW", "WNW", "WbN",
	"W", "WbS", "WSW", "SWbW", "SW", "SWbS", "SSW", "SbW",
	"S", "SbE", "SSE", "SEbS", "SE", "SEbE", "ESE", "EbS"
]

#! These are dynamic static var, so you don't need to edit them
_ANGLES = [x * 360 / DESIRED_NUM_ANGLES for x in range(DESIRED_NUM_ANGLES)]
_COMPASS_ROSE = [COMPASS_POINTS[int(round((i * len(COMPASS_POINTS)) / DESIRED_NUM_ANGLES)) % len(COMPASS_POINTS)] for i in range(DESIRED_NUM_ANGLES)]
_ANGLES_DIR = {angle: _COMPASS_ROSE[i] for i, angle in enumerate(_ANGLES)}

## METHODS

def get_direction(angle: int):
	for key in _ANGLES_DIR:
		if angle == key:
			return _ANGLES_DIR[key]
		elif key < angle < list(_ANGLES_DIR.keys())[list(_ANGLES_DIR.keys()).index(key) + 1]:
			return _ANGLES_DIR[key]

	return "NwN"

def render_directional_angles(path: str):
	# Ensure the path is absolute
	path = os.path.abspath(path)

	# Get the list of selected objects
	selected_list = bpy.context.selected_objects

	# Deselect all in scene
	bpy.ops.object.select_all(action='TOGGLE') # TODO: Verify if 'DESELECT' isn't better

	# Set render settings
	scene = bpy.context.scene
	scene.render.resolution_x = RENDER_RESOLUTION_X
	scene.render.resolution_y = RENDER_RESOLUTION_Y

	# Retrieve the root object if it is necessary to transalte it.
	# obRoot = bpy.context.scene.objects["root"]

	for obj in selected_list:
		# Select the object
		obj.select_set(True)

		scn = bpy.context.scene
		
		for action in bpy.data.actions:
			# Assign the action to the active object
			bpy.context.active_object.animation_data.action = bpy.data.actions.get(action.name)
			
			# Set the last frame to render based on the action
			scn.frame_end = int(action.frame_range[1])
			
			if action.name in ACTIONS_NAMES:
				# Create a folder for the action name if not exist
				action_folder = os.path.join(path, action.name)
				os.makedirs(action_folder, exist_ok=True)

				for angle in _ANGLES:
					angleDir = get_direction(angle)

					# Create a folder for the specific angle if not exist
					animation_folder = os.path.join(action_folder, angleDir)
					os.makedirs(animation_folder, exist_ok=True)
					
					# Rotate the model for the new angle
					bpy.context.active_object.rotation_euler[2] = math.radians(angle)
					
					# The below is for the use case where the root needed to be translated.
					# obRoot.rotation_euler[2] = math.radians(angle)

					for i in range(scene.frame_start, scene.frame_end, RENDER_FRAME_FREQUENCY):
						scene.frame_current = i

						scene.render.filepath = os.path.join(
							animation_folder,
							f"{action.name}_{angle}_{str(scene.frame_current).zfill(3)}"
						)

						bpy.ops.render.render(
							False, # undo support
							animation=False,
							write_still=True
						)

render_directional_angles(OUT_FOLDER_PATH)