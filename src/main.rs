use std::fs;
use std::path::{Path, PathBuf};
use image::{GenericImageView, RgbaImage};

// Set the folders path
const IN_FOLDER_PATH: &str = "/Users/_YourUsername_/Desktop/Blender/Sprites/Frames/Character"; // Change the path to your desired input folder
const OUT_FOLDER_PATH: &str = "/Users/_YourUsername_/Desktop/Blender/Sprites/Sheets/Character"; // Change the path to your desired output folder

// Set to whatever you want!
const MAX_SPRITE_PER_ROW: usize = 6;

/// Function to read the directory files and output the files path in a vector array
fn read_and_sort_files(directory: &Path) -> Vec<PathBuf> {
	let mut result = Vec::new();

	if let Ok(entries) = fs::read_dir(directory) {
		let mut files: Vec<_> = entries
			.filter_map(|entry| entry.ok())
			.map(|entry| entry.path())
			.collect();

		// Sort files numerically by file name considering leading zeros
		files.sort_by(|a, b| {
			let a_stem = a.file_stem().and_then(|s| s.to_str()).unwrap_or("");
			let b_stem = b.file_stem().and_then(|s| s.to_str()).unwrap_or("");
			a_stem.cmp(b_stem)
		});

		result.extend(files);
	}

	result
}

/// Create a spritesheet png for a given angle
/// 
/// * `angle_folder` - The folder containing the sprite frames of an animation on a specific angle
/// * `output_filename` - The destination of the generated spritesheet png
/// * `max_sprite_per_row` - The maximum allowed sprite per row on the generated spritesheet
fn create_sprite_sheet(angle_folder: &Path, output_filename: &Path, max_sprite_per_row: usize) {
	let files = read_and_sort_files(angle_folder);

	if files.is_empty() {
		println!("No images found in {:?}", angle_folder);
		return;
	}

	// Get image dimensions from the first image
	let first_image = image::open(&files[0]).expect("Failed to open image");
	let (width, height) = first_image.dimensions();

	// Calculate the number of rows based on max_sprite_per_row
	let num_rows = (files.len() as f32 / max_sprite_per_row as f32).ceil() as u32;

	// Create a new RGBA image for the spritesheet
	let mut sprite_sheet = RgbaImage::new(width * max_sprite_per_row as u32, height * num_rows);

	// Composite images onto the spritesheet
	for (index, file) in files.iter().enumerate() {
		let image = image::open(file).expect("Failed to open image");
		let (left, top) = ((index % max_sprite_per_row) as u32 * width, (index / max_sprite_per_row) as u32 * height);
		image::imageops::overlay(&mut sprite_sheet, &image.to_rgba8(), left.into(), top.into());
	}

	// Save the spritesheet
	sprite_sheet.save(output_filename).expect("Failed to save spritesheet");
	println!("Spritesheet created: {:?}", output_filename);
}

fn main() {
	let sprite_folder_path = Path::new(IN_FOLDER_PATH);
	let sheet_folder_path = Path::new(OUT_FOLDER_PATH);

	if let Ok(action_folders) = fs::read_dir(sprite_folder_path) {

		// Generate sprite sheets for each action
		for action_folder in action_folders.filter_map(|entry| entry.ok()) {
			let action_folder_path = action_folder.path();
			let action_folder_name = action_folder_path.file_name().unwrap().to_string_lossy();

			if let Ok(angle_folders) = fs::read_dir(&action_folder_path) {

				// Create ouput action folder if doesn't exist
				let output_action_folder = sheet_folder_path.join(format!("{}", action_folder_name));
				if let Err(e) = fs::create_dir_all(&output_action_folder) {
					eprintln!("Error creating output action folder {}: {}", action_folder_name, e);
					continue;
				}

				// Generate sprite sheets for each angles
				for angle_folder in angle_folders.filter_map(|entry| entry.ok()) {
					let angle_folder_path = angle_folder.path();
					let angle_folder_name = angle_folder_path.file_name().unwrap().to_string_lossy();

					if let Ok(metadata) = fs::metadata(&angle_folder_path) {
						if metadata.is_dir() {
							// Output filename based on the provided formats
							let output_filename = output_action_folder.join(format!(
								"{}_{}.png",
								action_folder_name,
								angle_folder_name,
							));

							// Create the spritesheet
							create_sprite_sheet(&angle_folder_path, &output_filename, MAX_SPRITE_PER_ROW);
						}
					}
				}
			}
		}
	}
}
