using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class SetListLoader : MonoBehaviour
{
	[SerializeField]
	private TMP_Dropdown SetListDropdown;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
# if UNITY_STANDALONE_WIN || UNITY_EDITOR 
		string targetFolder = Path.Combine(Application.streamingAssetsPath, "Sets");

		if (Directory.Exists(targetFolder))
		{
			string[] subfolders = Directory.GetDirectories(targetFolder);

			SetListDropdown.options.Clear();
			foreach (string folder in subfolders)
			{
				SetListDropdown.options.Add(new TMP_Dropdown.OptionData() { text = Path.GetFileName(folder) });
			}
		}
		else
		{
			Debug.LogWarning("Target folder does not exist: " + targetFolder);
		}
#endif

#if UNITY_WEBGL
		StartCoroutine(LoadWebGLSets());
#endif
	}

	private IEnumerator LoadWebGLSets()
	{
		string path = Path.Combine(Application.streamingAssetsPath, "Sets/folderList.txt");

		UnityWebRequest request = UnityWebRequest.Get(path);
		yield return request.SendWebRequest();

		if (request.result == UnityWebRequest.Result.Success)
		{
			SetListDropdown.options.Clear();
			string[] subfolders = request.downloadHandler.text.Split('\n');
			foreach (string folder in subfolders)
			{
				SetListDropdown.options.Add(new TMP_Dropdown.OptionData() { text = folder.Trim() });
			}
		}
		else
		{
			Debug.LogError("Failed to load folder list: " + request.error);
		}
	}
	// Update is called once per frame
	void Update()
	{

	}
}
