using Enums;

public class EnhancedIAController : EnhancedController
{
	protected override void OnTurnStart(object sender, Team currentTeam)
	{
		;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		;
	}

	// Update is called once per frame
	void Update()
	{
		if (!IsMyTurn())
		{
			return;
		}

		var selectedPlayingCard = SelectPlayingCard();

		var selectedBoardTile = SelectTile();

		PlayCard(selectedPlayingCard, selectedBoardTile);
	}

	private PlayingCard SelectPlayingCard()
	{
		var cards = GetHand().GetPlayingCards();
		return (cards != null && cards.Count > 0) ? cards[UnityEngine.Random.Range(0, cards.Count)] : null;
	}

	private BoardTile SelectTile()
	{
		var tiles = Board.GetInstance().GetFreeBoardTiles();
		return (tiles != null && tiles.Count > 0) ? tiles[UnityEngine.Random.Range(0, tiles.Count)] : null;
	}
}
