using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

public class SpritesheetsAnimationImporter : MonoBehaviour
{
	#region SUBCLASS

	[System.Serializable]
	public class SpritesheetAnimation
	{
		public string name; //! Avoid changing this property in the editor
		public string path; //! Avoid changing this property in the editor
		public float framesPerSecond;
		public bool loopTime;
		public bool includeFolder;
	}

	#endregion

	#region PROPERTIES

	// Set your own parent folder if you don't use the Assets folder.
	public string parentFolder = "Assets";

	// Set your own path
	public string mainFolder = "Sprites/Model";

	// Set your own path
	public string outFolder = "Animations/Model";

	[SerializeField]
	private List<SpritesheetAnimation> spritesheetAnimationList = new List<SpritesheetAnimation>();

	// Used as a comparator
	private string prevMainFolder = "";

	#endregion

#if (UNITY_EDITOR)

	#region METHODS

	[ContextMenu("CreateAnimations")]
	void CreateAnimations()
	{
		if (spritesheetAnimationList.Count == 0) {
			Debug.LogError($"Couldn't find the animation folders for path: {parentFolder}/{mainFolder}");
			return;
		}

		for (int x = 0; x < spritesheetAnimationList.Count; x++)
		{
			SpritesheetAnimation spritesheetAnimation = spritesheetAnimationList[x];
			if (spritesheetAnimation.includeFolder == false) continue;

			string animationFolderName = System.IO.Path.GetFileName(spritesheetAnimation.path);
			string animationDestinationPath = $"{outFolder}/{animationFolderName}";
			string[] spritesheetGUIDs = AssetDatabase.FindAssets("t:Sprite", new[] { spritesheetAnimation.path });

			CreateAnimationFolder(animationDestinationPath);

			for (int i = 0; i < spritesheetGUIDs.Length; i++)
			{
				string path = AssetDatabase.GUIDToAssetPath(spritesheetGUIDs[i]);
				string animationName = System.IO.Path.GetFileName(path).Replace(".png", "");
				Object[] objects = AssetDatabase.LoadAllAssetsAtPath(path);
				Sprite[] sprites = objects.OfType<Sprite>().ToArray(); // Filter out Texture2D at index 0

				CreateAnimationClip(
					animationDestinationPath,
					animationName,
					sprites,
					spritesheetAnimation.framesPerSecond,
					spritesheetAnimation.loopTime
				);
			}
		}

		Debug.Log("Animations created successfully!");
	}

	private void CreateAnimationClip(
		string animationDestinationPath,
		string clipName,
		Object[] sprites,
		float framesPerSecond,
		bool loopTime
	) {
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

	private void UpdateAnimationsList()
	{
		string spritesheetFolderPath = $"{parentFolder}/{mainFolder}";

		if (AssetDatabase.IsValidFolder(spritesheetFolderPath)) {
			string[] animationFolderPaths = System.IO.Directory.GetDirectories(spritesheetFolderPath);

			foreach (string animationFolderPath in animationFolderPaths)
			{
				SpritesheetAnimation animation = new SpritesheetAnimation
				{
					name = System.IO.Path.GetFileName(animationFolderPath),
					path = animationFolderPath,
					framesPerSecond = 12f,
					loopTime = false,
					includeFolder = true
				};

				spritesheetAnimationList.Add(animation);
			}
		}
	}

	#endregion

	#region MONOBEHAVIOR

	private void OnValidate()
	{
		if (prevMainFolder != mainFolder)
		{
			// Always clear the animation list whenever the animationFolderPath change
			spritesheetAnimationList.Clear();
			prevMainFolder = mainFolder;

			if (mainFolder != "") {
				UpdateAnimationsList();
			}
		}
	}

	#endregion

#endif
}
