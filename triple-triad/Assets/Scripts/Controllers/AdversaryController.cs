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
			gameManager.PlayCard(selectedPlayingCard, selectedBoardTile, Hand);
		}

		gameManager.StartNextTurn();

        Debug.Log("Passing turn for now");
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
}
