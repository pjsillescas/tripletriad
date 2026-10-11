using UnityEngine;
using UnityEngine.UI;

public class EnhancedAudioWidget : MonoBehaviour
{
	[SerializeField]
	private Button SoundOnButton;
	[SerializeField]
	private Button SoundOffButton;
	[SerializeField]
	private bool defaultSoundEnabled = false;

	private SoundManager soundManager;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		soundManager = SoundManager.GetInstance();

		if (defaultSoundEnabled)
		{
			SoundOffClick();
		}
		else
		{
			SoundOnClick();
		}

		SoundOnButton.onClick.RemoveAllListeners();
		SoundOnButton.onClick.AddListener(SoundOnClick);
		
		SoundOffButton.onClick.RemoveAllListeners();
		SoundOffButton.onClick.AddListener(SoundOffClick);
	}

	private void SoundOnClick()
	{
		soundManager.Click();
		soundManager.DisableSound();

		SoundOnButton.gameObject.SetActive(false);
		SoundOffButton.gameObject.SetActive(true);
	}

	private void SoundOffClick()
	{
		soundManager.Click();
		soundManager.EnableSound();
		
		SoundOnButton.gameObject.SetActive(true);
		SoundOffButton.gameObject.SetActive(false);
	}
}
