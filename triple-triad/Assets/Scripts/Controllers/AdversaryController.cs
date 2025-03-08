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
	}

	// Update is called once per frame
	void Update()
    {
		if (Team.Blue.Equals(gameManager.GetCurrentTeamTurn()))
        {
            return;
        }

        Debug.Log("Adversary turn start");

		var selectedPlayingCard = SelectPlayingCard();

		var selectedBoardTile = SelectTile();

		var board = Board.GetInstance();
		if (selectedPlayingCard != null && selectedBoardTile != null && board.CanPlaceCard(selectedBoardTile))
		{
			// 1. Place card in board
			board.AddCard(selectedPlayingCard, selectedBoardTile);

			// 2. Take card off the hand
			Hand.Drop(selectedPlayingCard);
		}

		gameManager.StartNextTurn();

        Debug.Log("Passing turn for now");
    }

	private PlayingCard SelectPlayingCard()
	{
		var cards = Hand.GetPlayingCards();
		return cards[UnityEngine.Random.Range(0, cards.Count)];
	}

	private BoardTile SelectTile()
	{
		var tiles = Board.GetInstance().GetFreeBoardTiles();
		return tiles[UnityEngine.Random.Range(0, tiles.Count)];
	}
}
