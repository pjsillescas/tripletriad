using Enums;
using System;
using UnityEngine;

public abstract class Controller : MonoBehaviour
{
	[SerializeField]
	protected Hand Hand;

	protected GameManager gameManager;
	protected bool isControllerEnabled;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	protected void Initialize()
    {
		gameManager = GameManager.GetInstance();
		gameManager.OnNewTurn += OnTurnStart;
		gameManager.OnFinishGame += OnFinishGame;

		isControllerEnabled = true;
	}

	protected abstract void OnTurnStart(object sender, Team currentTeam);
	protected void OnFinishGame(object sender, EventArgs args)
	{
		isControllerEnabled = false;
	}
}
