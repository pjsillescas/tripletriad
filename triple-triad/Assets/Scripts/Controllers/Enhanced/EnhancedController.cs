using Enums;
using System;
using UnityEngine;

public abstract class EnhancedController : MonoBehaviour
{
	protected abstract void OnTurnStart(object sender, Team currentTeam);

	private Hand hand;

	private Transform middlePosition;

	private int numCardsToFlip;
	private bool isFlipping;
	protected GameManager gameManager;
	protected bool isControllerEnabled;

	protected Hand GetHand() => hand;
	private Team team;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	public void Initialize(Hand hand, Transform middlePosition, Team team)
	{
		this.team = team;
		this.hand = hand;
		this.middlePosition = middlePosition;

		gameManager = GameManager.GetInstance();
		gameManager.OnNewTurn += OnTurnStart;
		gameManager.OnFinishGame += OnFinishGame;

		//ResetController();
		isControllerEnabled = false;
		isFlipping = false;

	}

	private void OnDisable()
	{
		gameManager = GameManager.GetInstance();
		gameManager.OnNewTurn -= OnTurnStart;
		gameManager.OnFinishGame -= OnFinishGame;
	}

	public void ResetController()
	{
		isControllerEnabled = true;
		isFlipping = false;
	}

	protected void OnFinishGame(object sender, EventArgs args)
	{
		isControllerEnabled = false;
	}

	private void EndTurn()
	{
		gameManager.StartNextTurn();
	}

	private void OnEndFlip()
	{
		SoundManager.GetInstance().Drip();

		numCardsToFlip--;

		if (numCardsToFlip == 0)
		{
			isFlipping = false;
			EndTurn();
		}
	}

	protected bool IsMyTurn()
	{
		//var curteam = gameManager.GetCurrentTeamTurn();
		//Debug.Log($"i am {team} and its {curteam} turn");
		return team.Equals(gameManager.GetCurrentTeamTurn()) && isControllerEnabled && !isFlipping;
	}

	protected bool ThereAreCardsToFlip()
	{
		return numCardsToFlip > 0;
	}

	protected void PlayCard(PlayingCard playingCard, BoardTile boardTile)
	{
		var board = Board.GetInstance();
		if (playingCard != null && boardTile != null && board.CanPlaceCard(boardTile))
		{
			DoPlayCard(playingCard, boardTile);
		}
	}

	private void DoPlayCard(PlayingCard playingCard, BoardTile boardTile)
	{
		SoundManager.GetInstance().Click();

		isFlipping = true;
		//playingCard.transform.position = boardTile.transform.position;
		playingCard.Travel(boardTile.transform.position, middlePosition.position,
			() =>
			{
				SoundManager.GetInstance().Drip();

				isFlipping = false;
				ProcessCard(playingCard, boardTile);
			});
	}

	protected void ProcessCard(PlayingCard playingCard, BoardTile boardTile)
	{
		var flippedCards = gameManager.PlayCard(playingCard, boardTile, hand);
		if (flippedCards.Count > 0)
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
