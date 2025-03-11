using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class ImageLoader : MonoBehaviour
{
    private UnityWebRequest uwr;

    public static string GetFileLocation(string path)
    {
        return /*"file://" +*/ Path.Combine(Application.streamingAssetsPath, path);
    }

    public void Load(string fileName, Action<Texture2D> onTextureLoad)
    {
        StartCoroutine(LoadImageCoroutine(fileName, onTextureLoad));
    }

    private IEnumerator LoadImageCoroutine(string fileName, Action<Texture2D> onTextureLoad)
    {
		using (uwr = UnityWebRequestTexture.GetTexture(GetFileLocation(fileName)))
        {
            if(uwr == null)
            {
                yield return null;
            }

            yield return uwr.SendWebRequest();
            if(uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
            {
				Debug.LogError(uwr.error);
            }
            else
            {
                onTextureLoad?.Invoke(DownloadHandlerTexture.GetContent(uwr));
            }
        }
    }

	private void Awake()
	{
        Debug.Log($"awake {gameObject.name}");
	}
}
