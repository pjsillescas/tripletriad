using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandSelector : MonoBehaviour
{
	public enum HandSelectionType { Random, Manual }

	[SerializeField]
	private List<Toggle> Toggles;

	private HandSelectionType currentHandSelection = HandSelectionType.Random;

	public HandSelectionType GetHandSelection() => currentHandSelection;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		foreach (var toggle in Toggles)
		{
			toggle.onValueChanged.AddListener((value) => ToggleOnValueChanged(toggle, value));
		}
	}

	private void ToggleOnValueChanged(Toggle toggle, bool value)
	{
		if (!value)
		{
			return;
		}

		currentHandSelection = toggle.name.StartsWith("Manual") ? HandSelectionType.Manual : HandSelectionType.Random;
	}


	// Update is called once per frame
	void Update()
	{

	}
}
