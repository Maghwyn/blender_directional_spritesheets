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

_ANGLES = [x * 11.25 for x in range(0, 32)]
_ANGLES_DIR = { #! 32-point compass counterclockwise
	0.0: "E",
	11.25: "EbN",
	22.5: "ENE",
	33.75: "NEbE",
	45.0: "NE",
	56.25: "NEbN",
	67.5: "NNE",
	78.75: "NbE",
	90.0: "N",
	101.25: "NbW",
	112.5: "NNW",
	123.75: "NWbN",
	135.0: "NW",
	146.25: "NWbW",
	157.5: "WNW",
	168.75: "WbN",
	180.0: "W",
	191.25: "WbS",
	202.5: "WSW",
	213.75: "SWbW",
	225.0: "SW",
	236.25: "SWbS",
	247.5: "SSW",
	258.75: "SbW",
	270.0: "S",
	281.25: "SbE",
	292.5: "SSE",
	303.75: "SEbS",
	315.0: "SE",
	326.25: "SEbE",
	337.5: "ESE",
	348.75: "EbS"
}

## METHODS

def get_direction(angle: int):
	for key in _ANGLES_DIR:
		if angle == key:
			return _ANGLES_DIR[key]
		elif key < angle < list(_ANGLES_DIR.keys())[list(_ANGLES_DIR.keys()).index(key) + 1]:
			return _ANGLES_DIR[key]

	return "NwN"

def render_32_directional_angles(path: str):
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

render_32_directional_angles(OUT_FOLDER_PATH)