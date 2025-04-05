using UnityEngine;
using UnityEngine.UI;
using static HandSelector;

public class SoundWidget : MonoBehaviour
{
	private Toggle toggle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		toggle = GetComponentInChildren<Toggle>();
		if (toggle != null)
		{
			toggle.onValueChanged.AddListener((value) => ToggleOnValueChanged(toggle, value));
		}
	}

	private void ToggleOnValueChanged(Toggle toggle, bool value)
	{
		var soundManager = SoundManager.GetInstance();

		soundManager.Click();
		if (value)
		{
			soundManager.EnableSound();
		}
		else
		{
			soundManager.DisableSound();
		}
	}
}
