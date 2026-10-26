using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateGameWidget : MonoBehaviour
{
	[SerializeField]
	private Transform Widget;
	[SerializeField]
	private TextMeshProUGUI WinnerText;
	[SerializeField]
	private TMP_Dropdown SetDropdown;
	[SerializeField]
	private Button CreateGameSoloButton;
	[SerializeField]
	private Button CreateGameOnlineButton;
	[SerializeField]
	private RuleVariationWidget RuleVariationWidget;
	[SerializeField]
	private float FinishTimeout = 2.5f;

	public class OnNewGameData
	{
		public string set;
		public List<IRuleVariation> rules;
	}

	public static event EventHandler<OnNewGameData> OnNewGame;


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		CreateGameSoloButton.onClick.AddListener(CreateGameSoloClick);
		CreateGameOnlineButton.onClick.AddListener(CreateGameOnlineClick);
		GameManager.GetInstance().OnEnhancedFinishGame += EnhancedFinishGame;

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

	private void EnhancedFinishGame(object sender, Score score)
	{
		StartCoroutine(DelayFinish(score));
	}

	private IEnumerator DelayFinish(Score score)
	{
		yield return new WaitForSeconds(FinishTimeout);

		SoundManager.GetInstance().StopBackground();

		string winner;
		if (score.IsLocalLoser())
		{
			SoundManager.GetInstance().PlaySadFanfare();
			winner = "You Lost!";
		}
		else if (score.IsLocalWinner())
		{
			SoundManager.GetInstance().PlayFanfare();
			winner = "You Won!";
		}
		else
		{
			SoundManager.GetInstance().PlayFanfare();
			winner = "Draw";
		}

		ActivateWidget(winner);

	}

	public void ActivateWidget()
	{
		Widget.gameObject.SetActive(true);
		DeactivateWinnerText();
	}

	public void ActivateWidget(string winner)
	{
		ActivateWinnerText(winner);

		StartCoroutine(DelayNewGameCoroutine());
	}

	private IEnumerator DelayNewGameCoroutine()
	{
		yield return new WaitForSeconds(2);
		Widget.gameObject.SetActive(true);
		RuleVariationWidget.gameObject.SetActive(true);
		yield return null;
	}

	private void DeactivateWinnerText()
	{
		WinnerText.gameObject.SetActive(false);
	}
	private void ActivateWinnerText(string winnerText)
	{
		WinnerText.text = winnerText;
		WinnerText.gameObject.SetActive(true);
	}

	private void CreateGameSoloClick()
	{
		SoundManager.GetInstance().Click();

		var chosenSet = SetDropdown.options[SetDropdown.value].text;
		//Debug.Log($"set {chosenSet} player [{handSelectionMethod}]");
		Widget.gameObject.SetActive(false);
		RuleVariationWidget.gameObject.SetActive(false);

		var rules = RuleVariationWidget.GetRuleVariations();

		Debug.Log($"{rules.Count} rules selected");
		rules.ForEach(rule => Debug.Log($"{rule} rules selected"));

		SoundManager.GetInstance().StopFanfare();
		SoundManager.GetInstance().PlayBackground();

		OnNewGame?.Invoke(this, new() { set = chosenSet, rules = rules });
	}

	private void CreateGameOnlineClick()
	{
		CreateGameSoloClick();
	}
}
