using Enums;

public class AdversaryController : Controller
{
	protected override void OnTurnStart(object sender, Team currentTeam)
	{
		;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		controlledTeam = Team.Red;
		Initialize();
	}

	// Update is called once per frame
	void Update()
	{
		if (!IsTeamTurn(controlledTeam))
		{
			return;
		}

		var selectedPlayingCard = SelectPlayingCard();

		var selectedBoardTile = SelectTile();

		PlayCard(selectedPlayingCard, selectedBoardTile);
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
