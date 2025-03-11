using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class JsonLoader : MonoBehaviour
{
    private UnityWebRequest uwr;

    public static string GetFileLocation(string path)
    {
        Debug.Log($"path {Application.streamingAssetsPath}");
        return /*"file://" + */Path.Combine(Application.streamingAssetsPath, path);
    }

    public void Load(string fileName, Action<string> onJsonLoad)
    {
        StartCoroutine(LoadJsonCoroutine(fileName, onJsonLoad));
    }

    private IEnumerator LoadJsonCoroutine(string fileName, Action<string> onJsonLoad)
    {
        Debug.Log(GetFileLocation(fileName));
        using (uwr = UnityWebRequest.Get(GetFileLocation(fileName)))
        {
            yield return uwr.SendWebRequest();
            if(uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
				onJsonLoad?.Invoke(uwr.downloadHandler.text);
            }
        }
    }
}
