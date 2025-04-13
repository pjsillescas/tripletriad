using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NewGameWidget : MonoBehaviour
{
	[SerializeField]
	private Transform Widget;
	[SerializeField]
	private TextMeshProUGUI WinnerText;
	[SerializeField]
	private GameObject WinnerPanel;
	[SerializeField]
	private TMP_Dropdown SetDropdown;
	[SerializeField]
	private HandSelector HandSelector;
	[SerializeField]
	private Button NewGameButton;
	[SerializeField]
	private RuleVariationWidget RuleVariationWidget;

	private Action<string, HandSelector.HandSelectionType, List<IRuleVariation>> onNewGame;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		NewGameButton.onClick.AddListener(NewGameClick);
		GameManager.GetInstance().OnFinishGame += FinishGame;

		RefreshCardSets();

		SetDropdown.onValueChanged.AddListener((_val) => SoundManager.GetInstance().Click());
	}

	private void RefreshCardSets()
	{
#if !UNITY_WEBGL
		var setsDirectory = Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Sets";
		var dirs = new List<string>(Directory.GetDirectories(setsDirectory)).
			Select(dir => { return dir.Split(Path.DirectorySeparatorChar).LastOrDefault(); }).ToList();
		
		SetDropdown.ClearOptions();
		SetDropdown.AddOptions(dirs);
#endif
	}

	private void FinishGame(object sender, EventArgs e)
	{
		var score = GameManager.GetInstance().GetScore();

		string winner;
		if (score.player > score.adversary)
		{
			winner = "You Won!";
		}
		else if (score.player < score.adversary)
		{
			winner = "You Lost!";
		}
		else
		{
			winner = "Draw";
		}

		ActivateWidget(winner);
	}

	public void ActivateWidget(Action<string,HandSelector.HandSelectionType, List<IRuleVariation>> onNewGame)
	{
		this.onNewGame = onNewGame;
		Widget.gameObject.SetActive(true);
		SetWinnerWidgetActive(false);
	}

	public void ActivateWidget(string winner)
	{
		SoundManager.GetInstance().StopBackground();
		SoundManager.GetInstance().PlayFanfare();

		WinnerText.text = winner;
		SetWinnerWidgetActive(true);

		StartCoroutine(DelayNewGameCoroutine());
	}

	private IEnumerator DelayNewGameCoroutine()
	{
		yield return new WaitForSeconds(2);
		Widget.gameObject.SetActive(true);
		RuleVariationWidget.gameObject.SetActive(true);
		yield return null;
	}

	private void SetWinnerWidgetActive(bool active)
	{
		WinnerText.gameObject.SetActive(active);
		WinnerPanel.SetActive(active);
	}
	private void NewGameClick()
	{
		SoundManager.GetInstance().Click();
		
		var chosenSet = SetDropdown.options[SetDropdown.value].text;
		var handSelectionMethod = HandSelector.GetHandSelection();
		//Debug.Log($"set {chosenSet} player [{handSelectionMethod}]");
		Widget.gameObject.SetActive(false);
		RuleVariationWidget.gameObject.SetActive(false);
		SetWinnerWidgetActive(false);

		var rules = RuleVariationWidget.GetRuleVariations();

		Debug.Log($"{rules.Count} rules selected");
		rules.ForEach(rule => Debug.Log($"{rule} rules selected"));

		SoundManager.GetInstance().StopFanfare();
		SoundManager.GetInstance().PlayBackground();

		onNewGame(chosenSet, handSelectionMethod, rules);
	}

	// Update is called once per frame
	void Update()
	{

	}
}
