using cards;
using Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static HandSelector;

public class GameManager : MonoBehaviour
{
	[SerializeField]
	private SetLoader Loader;
	[SerializeField]
	private int NumCardsPerHand;
	[SerializeField]
	private Hand PlayerHand;
	[SerializeField]
	private Hand AdversaryHand;
	[SerializeField]
	private PlayerController PlayerController;
	[SerializeField]
	private AdversaryController AdversaryController;
	//[SerializeField]
	//private WinnerManager WinnerManager;
	[SerializeField]
	private ScoreManager ScoreManager;
	[SerializeField]
	private ManualHandWidget ManualHandWidget;
	[SerializeField]
	private NewGameWidget NewGameWidget;

	public class Score
	{
		public int player;
		public int adversary;
	};

	public event EventHandler<Team> OnNewTurn;
	public event EventHandler OnFinishGame;
	public event EventHandler OnStartGame;
	public event EventHandler<Score> OnScoreChange;


	private static GameManager Instance = null;

	private Team currentTeamTurn;
	private int playerScore;
	private int adversaryScore;

	private bool playerHandLoaded;
	private bool adversaryHandLoaded;

	private TurnManager turnManager;
	private bool isGameOver;

	private HandSelectionType handSelectionType;

	private List<IRuleVariation> rules;

	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("GameManager instance duplicated");
		}

		Instance = this;
		currentTeamTurn = Team.None;
	}

	public static GameManager GetInstance() => Instance;

	public Team GetCurrentTeamTurn() => currentTeamTurn;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		currentTeamTurn = Team.None;
		//SetLoader.OnSetLoaded += (sender, cards) => Initialize();

		SetLoader.OnSetLoaded += (sender, cards) => {
			switch (handSelectionType)
			{
				case HandSelectionType.Manual:
					ManualHandWidget.ActivateWidget(OnPlayerHandChosen);
					break;
				case HandSelectionType.Random:
				default:
					OnPlayerHandChosen(GetRandomHand());
					break;
			}
		};

		PlayerHand.OnHandLoaded += OnHandLoaded;
		AdversaryHand.OnHandLoaded += OnHandLoaded;

		turnManager = GetComponent<TurnManager>();

		Initialize();
	}

	private void OnHandLoaded(object sender, Hand hand)
	{
		if (PlayerHand.Equals(hand))
		{
			playerHandLoaded = true;
		}

		if (AdversaryHand.Equals(hand))
		{
			adversaryHandLoaded = true;
		}

		if (playerHandLoaded && adversaryHandLoaded)
		{
			FinishInitialization();
		}
	}

	public void NewGame()
	{
		Initialize();
	}

	public Score GetScore() => new Score { player = playerScore, adversary = adversaryScore };

	public List<Card> GetRandomHand()
	{
		var allCards = Loader.GetCards();

		var cards = new List<Card>();
		for (int i = 0; i < NumCardsPerHand; i++)
		{
			cards.Add(allCards[UnityEngine.Random.Range(0, allCards.Count - 1)]);
		}

		return cards;
	}

	public void Initialize()
	{
		NewGameWidget.ActivateWidget(OnNewGame);
		//ManualHandWidget.ActivateWidget(OnPlayerHandChosen);
	}

	private void OnNewGame(string setName, HandSelectionType handSelectionType, List<IRuleVariation> rules)
	{
		this.handSelectionType = handSelectionType;
		this.rules = rules;

		SetLoader.GetInstance().LoadSet(setName);
	}

	private bool GetUseCardBackAdversary()
	{
		if(rules?.Count > 0)
		{
			return rules.Select(rule => rule.UseCardBack()).Aggregate(true, (acc, value) => acc && value);
		}

		return true;
	}

	private void OnPlayerHandChosen(List<Card> cards)
	{
		currentTeamTurn = turnManager.ResetTurn();
		isGameOver = false;

		playerHandLoaded = false;
		adversaryHandLoaded = false;

		PlayerHand.Initialize(cards, false);
		AdversaryHand.Initialize(GetRandomHand(), GetUseCardBackAdversary());

		playerScore = 5;
		adversaryScore = 5;
		OnScoreChange?.Invoke(this, GetScore());
		Board.GetInstance().Initialize();

		// UI
		ScoreManager.Initialize();
		//WinnerManager.Initialize();

		rules?.ForEach(rule => rule.Initialize());

		OnStartGame?.Invoke(this, EventArgs.Empty);
	}

	private void FinishInitialization()
	{
		currentTeamTurn = Team.None;
		turnManager.ChooseRandomTeam(team => { currentTeamTurn = team; });

		// Controllers
		EnableControllers();
	}

	private void EnableControllers()
	{
		PlayerController.ResetController();
		AdversaryController.ResetController();
	}

	public void StartNextTurn()
	{
		if (isGameOver)
		{
			return;
		}

		currentTeamTurn = Team.None;
		turnManager.SetNextTurn(team => {
			currentTeamTurn = team;
			OnNewTurn?.Invoke(this, currentTeamTurn);
		});
	}

	private bool RulesImplementWinsDirection()
	{
		return rules.Where(rule => rule.ImplementsWinsDirection()).Count() > 0;
	}

	private bool RulesWinsDirection(PlayingCard card1, PlayingCard card2, Board.Direction direction)
	{
		return rules.Select(rule => rule.WinsDirection(card1, card2, direction)).Aggregate(false, (acc, value) => acc || value);
	}

	private bool WinsDirection(PlayingCard card1, PlayingCard card2, Board.Direction direction)
	{
		if(RulesImplementWinsDirection())
		{
			return RulesWinsDirection(card1, card2, direction);
		}

		return direction switch
		{
			Board.Direction.North => card1.GetNorth() > card2.GetSouth(),
			Board.Direction.South => card1.GetSouth() > card2.GetNorth(),
			Board.Direction.West => card1.GetWest() > card2.GetEast(),
			Board.Direction.East => card1.GetEast() > card2.GetWest(),
			_ => throw new Exception($"Invalid direction to check '{direction}'"),
		};
	}

	public List<PlayingCard> PlayCard(PlayingCard playingCard, BoardTile boardTile, Hand hand)
	{
		var board = Board.GetInstance();

		playingCard.Play();

		var numFreeTiles = board.AddCard(playingCard, boardTile);
		isGameOver = numFreeTiles == 0;
		var flippedCards = board.GetDirections().Select(direction =>
		{
			var card = board.GetNeighbour(playingCard, direction);
			var isFlipped = card != null &&
				!card.GetCurrentTeam().Equals(playingCard.GetCurrentTeam()) &&
				WinsDirection(playingCard, card, direction);
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
		}
		
		if (IsGameOver())
		{
			OnFinishGame?.Invoke(this, EventArgs.Empty);
		}

		hand.Drop(playingCard);

		return flippedCards;
	}

	private bool IsGameOver() => isGameOver;

	// Update is called once per frame
	void Update()
	{

	}
}
