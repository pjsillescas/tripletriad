using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    private const int MAX_CARDS = 9;

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
        Initialize();
	}

	public void Initialize()
	{
        for (int i = 0; i < MAX_CARDS; i++)
        {
            if (cards[i] != null)
            {
                Destroy(cards[i]);
            }

            cards[i] = null;
        }
	}

    public int GetTileIndex(BoardTile tile)
    {
        var vector = tile.GetTileRow();

        return (int)(vector.x - 1) * 3 + (int)vector.y - 1;
    }

    public bool CanPlaceCard(BoardTile targetTile)
    {
        var index = GetTileIndex(targetTile);
        return cards[index] == null;
    }

	public void AddCard(PlayingCard playingCard, BoardTile targetTile)
    {
        if(CanPlaceCard(targetTile))
        {
			var index = GetTileIndex(targetTile);
			cards[index] = playingCard;

            playingCard.transform.position = targetTile.transform.position;
		}
    }

    public List<BoardTile> GetFreeBoardTiles()
    {
        return Tiles.Where(tile => cards[GetTileIndex(tile)] == null).ToList();
    }
}
