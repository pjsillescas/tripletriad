using Enums;
using NUnit.Framework.Interfaces;
using System;
using UnityEngine;

public class AdversaryController : Controller
{

	protected override void OnTurnStart(object sender, Team currentTeam)
	{
        ;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		Initialize();
		isFlipping = false;
	}

	// Update is called once per frame
	void Update()
    {
		if (Team.Blue.Equals(gameManager.GetCurrentTeamTurn()) || !isControllerEnabled || isFlipping)
        {
            return;
        }

        Debug.Log("Adversary turn start");

		var selectedPlayingCard = SelectPlayingCard();

		var selectedBoardTile = SelectTile();

		var board = Board.GetInstance();
		if (selectedPlayingCard != null && selectedBoardTile != null && board.CanPlaceCard(selectedBoardTile))
		{
			var flippedCards = gameManager.PlayCard(selectedPlayingCard, selectedBoardTile, Hand);
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

	private PlayingCard SelectPlayingCard()
	{
		var cards = Hand.GetPlayingCards();
		return (cards != null && cards.Count > 0) ? cards[UnityEngine.Random.Range(0, cards.Count)] : null;
	}

	private BoardTile SelectTile()
	{
		var tiles = Board.GetInstance().GetFreeBoardTiles();
		return (tiles != null && tiles.Count > 0) ? tiles[UnityEngine.Random.Range(0, tiles.Count)] : null;
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

}
