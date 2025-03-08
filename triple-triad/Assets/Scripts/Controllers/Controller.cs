using Enums;
using System;
using UnityEngine;

public abstract class Controller : MonoBehaviour
{
	[SerializeField]
	protected Hand Hand;

	protected GameManager gameManager;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	protected void Initialize()
    {
		gameManager = GameManager.GetInstance();
		gameManager.OnNewTurn += OnTurnStart;
	}

	protected abstract void OnTurnStart(object sender, Team currentTeam);
}
