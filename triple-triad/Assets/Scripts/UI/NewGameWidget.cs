using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewGameWidget : MonoBehaviour
{
	[SerializeField]
	private Transform Widget;
	[SerializeField]
	private TextMeshProUGUI WinnerText;
	[SerializeField]
	private TMP_Dropdown SetDropdown;
	[SerializeField]
	private HandSelector HandSelector;
	[SerializeField]
	private Button NewGameButton;

	private Action<string, HandSelector.HandSelectionType> onNewGame;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		NewGameButton.onClick.AddListener(NewGameClick);
		GameManager.GetInstance().OnFinishGame += FinishGame;
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

	public void ActivateWidget(Action<string,HandSelector.HandSelectionType> onNewGame)
	{
		this.onNewGame = onNewGame;
		Widget.gameObject.SetActive(true);
		WinnerText.gameObject.SetActive(false);
	}

	public void ActivateWidget(string winner)
	{
		WinnerText.text = winner;
		Widget.gameObject.SetActive(true);
		WinnerText.gameObject.SetActive(true);

	}

	private void NewGameClick()
	{
		var chosenSet = SetDropdown.options[SetDropdown.value].text;
		var handSelectionMethod = HandSelector.GetHandSelection();
		Debug.Log($"set {chosenSet} player [{handSelectionMethod}]");
		Widget.gameObject.SetActive(false);
		WinnerText.gameObject.SetActive(false);

		onNewGame(chosenSet, handSelectionMethod);
	}

	// Update is called once per frame
	void Update()
	{

	}
}
