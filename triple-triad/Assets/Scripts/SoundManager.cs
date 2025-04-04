using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource ClickSource;
    [SerializeField]
    private AudioSource DripSource;
    [SerializeField]
    private AudioSource BackgroundSource;

    private static SoundManager instance = null;

    public static SoundManager GetInstance() => instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instance != null)
        {
            Debug.LogError("SoundManager duplicated");
            return;
        }

        instance = this;

        //PlayBackground();
    }

    public void PlayBackground()
    {
        BackgroundSource.Play();
    }

    public void StopBackground()
    {
        BackgroundSource.Stop();
    }

    public void Click() => ClickSource.Play();
    public void Drip() { Debug.Log("drip"); DripSource.Play();}
}
