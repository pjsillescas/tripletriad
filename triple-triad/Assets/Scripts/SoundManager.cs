using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource ClickSource;
    [SerializeField]
    private AudioSource DripSource;
    [SerializeField]
    private AudioSource BackgroundSource;
	[SerializeField]
	private AudioSource FanfareSource;
	[SerializeField]
	private AudioSource SadFanfareSource;

	private static SoundManager instance = null;

    private bool isSoundEnabled = false;

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
        if (isSoundEnabled)
        {
            BackgroundSource.Play();
        }
    }

    public void StopBackground()
    {
        if (isSoundEnabled)
        {
            BackgroundSource.Stop();
        }
    }

    public void PlayFanfare()
    {
        if (isSoundEnabled)
        {
            FanfareSource.Play();
        }
    }
	public void StopFanfare()
	{
		if (isSoundEnabled)
		{
			FanfareSource.Stop();
		}
	}

	public void PlaySadFanfare()
	{
		if (isSoundEnabled)
		{
			SadFanfareSource.Play();
		}
	}
	public void StopSadFanfare()
	{
		if (isSoundEnabled)
		{
			SadFanfareSource.Stop();
		}
	}

	public void Click()
    {
        if (isSoundEnabled)
        {
            ClickSource.Play();
        }
    }

    public void Drip()
    {
        if (isSoundEnabled)
        {
            DripSource.Play();
        }
    }

    public void EnableSound()
    {
        isSoundEnabled = true;
        BackgroundSource.Play();
    }

	public void DisableSound()
	{
		isSoundEnabled = false;
		BackgroundSource.Stop();
	}

}
