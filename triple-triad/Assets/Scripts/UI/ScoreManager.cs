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
        GameManager.GetInstance().OnScoreChange += OnScoreChange;
    }

	private void OnScoreChange(object sender, GameManager.Score score)
	{
		AdversaryScoreText.text = score.adversary.ToString();
		PlayerScoreText.text = score.player.ToString();
	}
}
