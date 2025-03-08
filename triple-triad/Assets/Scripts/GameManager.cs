using Enums;
using System;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField]
	private SetLoader Loader;
	[SerializeField]
	private Hand PlayerHand;
	[SerializeField]
	private Hand AdversaryHand;
	[SerializeField]
	private PlayerController PlayerController;
	[SerializeField]
	private AdversaryController AdversaryController;
	[SerializeField]
	private WinnerManager WinnerManager;
	[SerializeField]
	private ScoreManager ScoreManager;

	public class Score
	{
		public int player;
		public int adversary;
	};

	public event EventHandler<Team> OnNewTurn;
	public event EventHandler OnFinishGame;
	public event EventHandler<Score> OnScoreChange;


	private static GameManager Instance = null;

	private Team currentTeamTurn;
	private int playerScore;
	private int adversaryScore;

	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("GameManager instance duplicated");
		}

		Instance = this;
	}

	public static GameManager GetInstance() => Instance;

	public Team GetCurrentTeamTurn() => currentTeamTurn;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		Initialize();
	}

	public void NewGame()
	{
		Initialize();
	}

	public Score GetScore() => new Score { player = playerScore, adversary = adversaryScore };

	public void Initialize()
	{
		PlayerHand.Initialize(false);
		AdversaryHand.Initialize(true);
		
		currentTeamTurn = (UnityEngine.Random.Range(0, 2) == 0) ? Team.Blue : Team.Red;
		playerScore = 5;
		adversaryScore = 5;
		OnScoreChange?.Invoke(this, GetScore());
		Board.GetInstance().Initialize();
		
		// Controllers
		PlayerController.ResetController();
		AdversaryController.ResetController();

		// UI
		ScoreManager.Initialize();
		WinnerManager.Initialize();
	}

	public void StartNextTurn()
	{
		currentTeamTurn = Team.Red.Equals(currentTeamTurn) ? Team.Blue : Team.Red;
		OnNewTurn?.Invoke(this, currentTeamTurn);
	}

	private bool WinsDirection(PlayingCard card1, PlayingCard card2, Board.Direction direction)
	{
		return direction switch
		{
			Board.Direction.North => card1.GetNorth() > card2.GetSouth(),
			Board.Direction.South => card1.GetSouth() > card2.GetNorth(),
			Board.Direction.West => card1.GetWest() > card2.GetEast(),
			Board.Direction.East => card1.GetEast() > card2.GetWest(),
			_ => throw new Exception($"Invalid direction to check '{direction}'"),
		};
	}

	public void PlayCard(PlayingCard playingCard, BoardTile boardTile, Hand hand)
	{
		var board = Board.GetInstance();

		playingCard.Play();

		var numFreeTiles = board.AddCard(playingCard, boardTile);
		//Debug.Log($"playing card {playingCard.GetCardName()}");
		var flippedCards = board.GetDirections().Select(direction =>
		{
			var card = board.GetNeighbour(playingCard, direction);
			//var n = card == null ? "none" : card.GetCardName();
			//Debug.Log($"neighbour {direction} {n}");
			var isFlipped = card != null &&
				!card.GetCurrentTeam().Equals(playingCard.GetCurrentTeam()) &&
				WinsDirection(playingCard, card, direction);
			//Debug.Log(isFlipped ? "flipped" : "not flipped");
			return (isFlipped) ? card : null;
		}).Where(card => card != null).ToList();

		if (flippedCards.Count > 0)
		{
			if (playingCard.GetCurrentTeam().Equals(Team.Blue))
			{
				playerScore += flippedCards.Count;
				adversaryScore -= flippedCards.Count;
			}
			else
			{
				playerScore -= flippedCards.Count;
				adversaryScore += flippedCards.Count;
			}

			flippedCards.ForEach(card => card.SetCurrentTeam(playingCard.GetCurrentTeam()));
			OnScoreChange?.Invoke(this, GetScore());

			// Debug.Log($"player {playerScore} adversary {adversaryScore}");
		}

		if (numFreeTiles == 0)
		{
			OnFinishGame?.Invoke(this, EventArgs.Empty);
		}

		hand.Drop(playingCard);
	}

	// Update is called once per frame
	void Update()
	{

	}
}
