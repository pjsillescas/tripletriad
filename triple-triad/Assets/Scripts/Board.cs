using cards;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
	private static readonly int OpenHash = Animator.StringToHash("Open");
	private const int BOARD_DIMENSION = 3;
	private const int MAX_CARDS = BOARD_DIMENSION * BOARD_DIMENSION;


	public enum Direction { North, East, South, West }

	[SerializeField]
	private List<BoardTile> Tiles;

	private PlayingCard[] cards;
	private Animator animator;

	private static Board Instance = null;

	public static Board GetInstance()
	{
		return Instance;
	}

	public List<BoardTile> GetTiles() => Tiles;

	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("Board duplicated");
			return;
		}

		Instance = this;
		cards = new PlayingCard[MAX_CARDS];
		animator = GetComponent<Animator>();
	}

	private void Start()
	{
		GameManager.GetInstance().OnStartGame += OnStartGame;
		GameManager.GetInstance().OnFinishGame += OnFinishGame;
	}

	private void OnDisable()
	{
		GameManager.GetInstance().OnStartGame -= OnStartGame;
		GameManager.GetInstance().OnFinishGame -= OnFinishGame;
	}

	private void OnStartGame(object sender, EventArgs args)
	{
		SetOpen(true);
	}

	private void OnFinishGame(object sender, EventArgs args)
	{
		SetOpen(false);
	}

	private void SetOpen(bool open)
	{
		if (animator != null)
		{
			animator.SetBool(OpenHash, open);
		}
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

		foreach (BoardTile tile in Tiles)
		{
			tile.SetElement(Card.Element.none);
		}
	}

	private int GetTileIndex(BoardTile tile)
	{
		var vector = tile.GetTileRow();

		return (int)(vector.x - 1) * BOARD_DIMENSION + (int)vector.y - 1;
	}

	public bool CanPlaceCard(BoardTile targetTile)
	{
		var index = GetTileIndex(targetTile);
		return cards[index] == null;
	}

	public void AddCard(PlayingCard playingCard, BoardTile targetTile)
	{
		if (CanPlaceCard(targetTile))
		{
			var index = GetTileIndex(targetTile);
			cards[index] = playingCard;

			if (!Card.Element.none.Equals(targetTile.GetElement()))
			{
				var modifier = (targetTile.GetElement().Equals(playingCard.GetElement())) ? 1 : -1;
				playingCard.SetModifier(modifier);
			}
		}
	}

	public int GetNumFreeTiles()
	{
		return new List<PlayingCard>(cards).Count(card => card == null);
	}

	public List<BoardTile> GetFreeBoardTiles()
	{
		return Tiles.Where(tile => cards[GetTileIndex(tile)] == null).ToList();
	}

	private PlayingCard GetNeighbour(PlayingCard playingCard, Direction direction)
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

	private bool RulesImplementWinsDirection(List<IRuleVariation> rules)
	{
		return rules.Any(rule => rule.ImplementsWinsDirection());
	}

	private bool RulesWinsDirection(PlayingCard card1, PlayingCard card2, Direction direction, List<IRuleVariation> rules)
	{
		return rules.Select(rule => rule.WinsDirection(card1, card2, direction)).Aggregate(false, (acc, value) => acc || value);
	}

	private bool WinsDirection(PlayingCard card1, PlayingCard card2, Direction direction, List<IRuleVariation> rules)
	{
		if (RulesImplementWinsDirection(rules))
		{
			return RulesWinsDirection(card1, card2, direction, rules);
		}

		return direction switch
		{
			Direction.North => card1.GetNorth() > card2.GetSouth(),
			Direction.South => card1.GetSouth() > card2.GetNorth(),
			Direction.West => card1.GetWest() > card2.GetEast(),
			Direction.East => card1.GetEast() > card2.GetWest(),
			_ => throw new Exception($"Invalid direction to check '{direction}'"),
		};
	}

	public List<PlayingCard> GetFlippedCards(PlayingCard playingCard, List<IRuleVariation> rules)
	{
		List<Direction> directions = new () { Direction.North, Direction.South, Direction.East, Direction.West };
		return directions.Select(direction =>
		{
			var card = GetNeighbour(playingCard, direction);
			var isFlipped = card != null &&
				!card.GetCurrentTeam().Equals(playingCard.GetCurrentTeam()) &&
				WinsDirection(playingCard, card, direction, rules);
			return (isFlipped) ? card : null;
		}).Where(card => card != null).ToList();
	}
}
