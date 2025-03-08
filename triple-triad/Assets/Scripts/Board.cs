using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
	private const int BOARD_DIMENSION = 3;
	private const int MAX_CARDS = BOARD_DIMENSION * BOARD_DIMENSION;


	public enum Direction { North, East, South, West }

	[SerializeField]
	private List<BoardTile> Tiles;

	private PlayingCard[] cards;

	private static Board Instance = null;

	public static Board GetInstance()
	{
		return Instance;
	}

	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("Board duplicated");
			return;
		}

		Instance = this;
		cards = new PlayingCard[MAX_CARDS];
		//Initialize();
	}

	public void Initialize()
	{
		for (int i = 0; i < MAX_CARDS; i++)
		{
			if (cards[i] != null)
			{
				Destroy(cards[i].gameObject);
			}

			cards[i] = null;
		}
	}

	public int GetTileIndex(BoardTile tile)
	{
		var vector = tile.GetTileRow();

		return (int)(vector.x - 1) * BOARD_DIMENSION + (int)vector.y - 1;
	}

	public bool CanPlaceCard(BoardTile targetTile)
	{
		var index = GetTileIndex(targetTile);
		return cards[index] == null;
	}

	public int AddCard(PlayingCard playingCard, BoardTile targetTile)
	{
		if (CanPlaceCard(targetTile))
		{
			var index = GetTileIndex(targetTile);
			cards[index] = playingCard;

			playingCard.transform.position = targetTile.transform.position;
		}

		return new List<PlayingCard>(cards).Where(card => card == null).Count(); // ToList().Count;
	}

	public List<BoardTile> GetFreeBoardTiles()
	{
		return Tiles.Where(tile => cards[GetTileIndex(tile)] == null).ToList();
	}

	public PlayingCard GetNeighbour(PlayingCard playingCard, Direction direction)
	{
		var index = 0;
		while (index < MAX_CARDS && !playingCard.Equals(cards[index]))
		{
			index++;
		}

		if (index == MAX_CARDS)
		{
			return null;
		}

		var card = cards[index];
		var cardColumn = index % BOARD_DIMENSION;
		var cardRow = (index - cardColumn) / BOARD_DIMENSION;

		switch (direction)
		{
			case Direction.North:
				cardRow--;
				break;
			case Direction.South:
				cardRow++;
				break;
			case Direction.West:
				cardColumn--;
				break;
			case Direction.East:
				cardColumn++;
				break;
		}

		var isValidNeighbour = 0 <= cardRow && cardRow < BOARD_DIMENSION &&
			0 <= cardColumn && cardColumn < BOARD_DIMENSION;

		return (isValidNeighbour) ? cards[cardRow * BOARD_DIMENSION + cardColumn] : null;
	}

	public List<Direction> GetDirections()
	{
		return new() { Direction.North, Direction.South, Direction.East, Direction.West };
	}
}
