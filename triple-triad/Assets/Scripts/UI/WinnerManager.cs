using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinnerManager : MonoBehaviour
{
    [SerializeField]
    private Transform Widget;
	[SerializeField]
	private TextMeshProUGUI WinnerText;
	[SerializeField]
	private Button NewGameButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NewGameButton.onClick.AddListener(NewGameClick);
        GameManager.GetInstance().OnFinishGame += OnFinishGame;
        Initialize();
    }

    public void Initialize()
    {
		Widget.gameObject.SetActive(false);
	}

	private void OnFinishGame(object sender, EventArgs e)
	{
        var score = GameManager.GetInstance().GetScore();

        if (score.player > score.adversary)
        {
            WinnerText.text = "You Won!";
        }
		else if (score.player < score.adversary)
		{
			WinnerText.text = "You Lost!";
		}
        else
        {
            WinnerText.text = "Draw";
        }

        Widget.gameObject.SetActive(true);
	}

	private void NewGameClick()
    {
        Debug.Log("New game!!");
    }
}
