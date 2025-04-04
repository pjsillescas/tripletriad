using System;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI AdversaryScoreText;

	[SerializeField]
	private TextMeshProUGUI PlayerScoreText;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		var gameManager = GameManager.GetInstance();

		gameManager.OnScoreChange += OnScoreChange;
		//gameManager.OnFinishGame += (sender, args) => DeactivateWidget();
		gameManager.OnStartGame += (sender, args) => ActivateWidget();

		DeactivateWidget();
    }

	public void ActivateWidget()
	{
		AdversaryScoreText.gameObject.SetActive(true);
		PlayerScoreText.gameObject.SetActive(true);
	}

	public void DeactivateWidget()
	{
		AdversaryScoreText.gameObject.SetActive(false);
		PlayerScoreText.gameObject.SetActive(false);
	}

	private void OnScoreChange(object sender, GameManager.Score score)
	{
		AdversaryScoreText.text = score.adversary.ToString();
		PlayerScoreText.text = score.player.ToString();
	}

	public void Initialize()
	{
		ActivateWidget();
	}
}
