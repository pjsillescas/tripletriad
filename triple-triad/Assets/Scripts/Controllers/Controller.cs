using Enums;
using System;
using UnityEngine;

public abstract class Controller : MonoBehaviour
{
	[SerializeField]
	protected Hand Hand;

	[SerializeField]
	private Transform MiddlePosition;

	protected GameManager gameManager;
	protected bool isControllerEnabled;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	protected void Initialize()
    {
		gameManager = GameManager.GetInstance();
		gameManager.OnNewTurn += OnTurnStart;
		gameManager.OnFinishGame += OnFinishGame;

		//ResetController();
		isControllerEnabled = false;
		isFlipping = false;

	}

	public void ResetController()
	{
		isControllerEnabled = true;
		isFlipping = false;
	}

	protected abstract void OnTurnStart(object sender, Team currentTeam);
	protected void OnFinishGame(object sender, EventArgs args)
	{
		isControllerEnabled = false;
	}

	private void EndTurn()
	{
		gameManager.StartNextTurn();
	}

	private int numCardsToFlip;
	private bool isFlipping;
	private void OnEndFlip()
	{
		numCardsToFlip--;

		if (numCardsToFlip == 0)
		{
			isFlipping = false;
			EndTurn();
		}
	}

	protected bool IsTeamTurn(Team team)
	{
		return team.Equals(gameManager.GetCurrentTeamTurn()) || !isControllerEnabled || isFlipping;
	}

	protected bool ThereAreCardsToFlip()
	{
		return numCardsToFlip > 0;
	}

	protected void PlayCard(PlayingCard playingCard, BoardTile boardTile)
	{
		isFlipping = true;
		//playingCard.transform.position = boardTile.transform.position;
		playingCard.Travel(boardTile.transform.position, MiddlePosition.position,
			() =>
			{
				isFlipping = false;
				ProcessCard(playingCard, boardTile);
			});
	}

	protected void ProcessCard(PlayingCard playingCard, BoardTile boardTile)
	{
		var flippedCards = gameManager.PlayCard(playingCard, boardTile, Hand);
		if (flippedCards != null && flippedCards.Count > 0)
		{
			isFlipping = true;
			numCardsToFlip = flippedCards.Count;
			flippedCards.ForEach(card => card.Flip(OnEndFlip));
		}
		else
		{
			EndTurn();
		}
	}
}
