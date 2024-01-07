using UnityEngine;
using UnityEditor;
using System.Linq;

public class SpritesheetsAnimationImporter : MonoBehaviour
{
	#region PROPERTIES

	// Set your own parent folder if you don't use the Assets folder.
	public string parentFolder = "Assets";

	// Set your own path
	public string mainFolder = "Sprites/Model";

	// Set your own path
	public string outFolder = "Animations/Model";

	// Change it to your needs
	public float framesPerSecond = 12f;

	// TODO: I could potentially do that better per animations, but i would need a selector from the editor.. mh.
	// TODO: Is there a way to dynamically generate the properties based on mainFolder changes ?
	// Enable to make the animation play through and then restart when the end is reached.
	public bool loopTime = true;

	#endregion

	#region METHODS

#if (UNITY_EDITOR)

	[ContextMenu("CreateAnimations")]
	void CreateAnimations()
	{
		string[] animationFolderPaths = System.IO.Directory.GetDirectories($"{parentFolder}/{mainFolder}");

		foreach (string animationFolderPath in animationFolderPaths)
		{
			string animationFolderName = System.IO.Path.GetFileName(animationFolderPath);
			string animationDestinationPath = $"{outFolder}/{animationFolderName}";
			string[] spritesheetGUIDs = AssetDatabase.FindAssets("t:Sprite", new[] { animationFolderPath });

			CreateAnimationFolder(animationDestinationPath);

			for (int i = 0; i < spritesheetGUIDs.Length; i++)
			{
				string path = AssetDatabase.GUIDToAssetPath(spritesheetGUIDs[i]);
				string animationName = System.IO.Path.GetFileName(path).Replace(".png", "");
				Object[] objects = AssetDatabase.LoadAllAssetsAtPath(path);
				Sprite[] sprites = objects.OfType<Sprite>().ToArray(); // Filter out Texture2D at index 0

				CreateAnimationClip(animationDestinationPath, animationName, sprites);
			}
		}

		Debug.Log("Animations created successfully!");
	}

	private void CreateAnimationClip(string animationDestinationPath, string clipName, Object[] sprites)
	{
		AnimationClip clip = new AnimationClip();
		clip.name = clipName;
		clip.frameRate = framesPerSecond;

		EditorCurveBinding curveBinding = new EditorCurveBinding();
		curveBinding.type = typeof(SpriteRenderer);
		curveBinding.path = "";
		curveBinding.propertyName = "m_Sprite";

		ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[sprites.Length];

		for (int i = 0; i < sprites.Length; i++)
		{
			keyFrames[i] = new ObjectReferenceKeyframe();
			keyFrames[i].time = i / framesPerSecond;
			keyFrames[i].value = sprites[i];
		}

		AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, keyFrames);

		if (loopTime) {
			AnimationClipSettings clipSettings = AnimationUtility.GetAnimationClipSettings(clip);
			clipSettings.loopTime = true;
			AnimationUtility.SetAnimationClipSettings(clip, clipSettings);
		}

		AssetDatabase.CreateAsset(clip, $"{parentFolder}/{animationDestinationPath}/{clipName}.anim");
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}

	private void CreateAnimationFolder(string animationFolderPath) {
		if (!AssetDatabase.IsValidFolder(animationFolderPath))
		{
			string[] folders = animationFolderPath.Split('/');
			string currentPath = parentFolder;

			foreach (string folder in folders)
			{
				string newPath = $"{currentPath}/{folder}";

				if (!AssetDatabase.IsValidFolder(newPath))
				{
					AssetDatabase.CreateFolder(currentPath, folder);
				}

				currentPath = newPath;
			}
		}
	}

#endif

	#endregion
}
