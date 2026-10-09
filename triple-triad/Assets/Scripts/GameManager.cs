using cards;
using Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static HandSelector;

[RequireComponent(typeof(TurnManager))]
public class GameManager : MonoBehaviour
{
	private enum GameState { INITIALIZATION, PLAYING, FINISH_GAME }
	private enum InitGameState { NONE, PLAYER_INIT, ADVERSARY_INIT, FINISH_INIT }

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

	private GameState gameState;
	private InitGameState initGameState;

	private TurnManager turnManager;

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

		SetLoader.OnSetLoaded += (sender, cards) =>
		{
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

		NewGame();
	}

	public void NewGame()
	{
		gameState = GameState.INITIALIZATION;
		initGameState = InitGameState.NONE;

		NewGameWidget.ActivateWidget(OnNewGame);
	}

	private void OnHandLoaded(object sender, Hand hand)
	{
		if (gameState != GameState.INITIALIZATION)
		{
			return;
		}

		if (PlayerHand.Equals(hand) && initGameState != InitGameState.PLAYER_INIT)
		{
			if (initGameState == InitGameState.NONE)
			{
				initGameState = InitGameState.PLAYER_INIT;
			}
			else // initGameState == InitGameState.ADVERSARY_INIT
			{
				initGameState = InitGameState.FINISH_INIT;
			}
		}

		if (AdversaryHand.Equals(hand) && initGameState != InitGameState.ADVERSARY_INIT)
		{
			if (initGameState == InitGameState.NONE)
			{
				initGameState = InitGameState.ADVERSARY_INIT;
			}
			else // initGameState == InitGameState.PLAYER_INIT
			{
				initGameState = InitGameState.FINISH_INIT;
			}
		}

		if (initGameState == InitGameState.FINISH_INIT)
		{
			FinishInitialization();
		}
	}

	private void FinishInitialization()
	{
		initGameState = InitGameState.NONE;
		gameState = GameState.PLAYING;

		currentTeamTurn = Team.None;
		turnManager.ChooseRandomTeam(team => { currentTeamTurn = team; });

		// Controllers
		PlayerController.ResetController();
		AdversaryController.ResetController();
	}

	public Score GetScore() => new() { player = playerScore, adversary = adversaryScore };

	private List<Card> GetRandomHand()
	{
		var allCards = Loader.GetCards();

		var cards = new List<Card>();
		for (int i = 0; i < NumCardsPerHand; i++)
		{
			cards.Add(allCards[UnityEngine.Random.Range(0, allCards.Count)]);
		}

		return cards;
	}

	private void OnNewGame(string setName, HandSelectionType handSelectionType, List<IRuleVariation> rules)
	{
		this.handSelectionType = handSelectionType;
		this.rules = rules;

		SetLoader.GetInstance().LoadSet(setName);
	}

	private bool GetUseCardBackAdversary()
	{
		if (rules?.Count > 0)
		{
			return rules.Select(rule => rule.UseCardBack()).Aggregate(true, (acc, value) => acc && value);
		}

		return true;
	}

	private void OnPlayerHandChosen(List<Card> cards)
	{
		currentTeamTurn = turnManager.ResetTurn();

		gameState = GameState.INITIALIZATION;
		initGameState = InitGameState.NONE;

		PlayerHand.Initialize(cards, false);
		AdversaryHand.Initialize(GetRandomHand(), GetUseCardBackAdversary());

		playerScore = 5;
		adversaryScore = 5;
		OnScoreChange?.Invoke(this, GetScore());
		Board.GetInstance().Initialize();

		rules?.ForEach(rule => rule.Initialize());

		OnStartGame?.Invoke(this, EventArgs.Empty);
	}

	public void StartNextTurn()
	{
		if (gameState == GameState.FINISH_GAME)
		{
			return;
		}

		currentTeamTurn = Team.None;
		turnManager.SetNextTurn(team =>
		{
			currentTeamTurn = team;
			OnNewTurn?.Invoke(this, currentTeamTurn);
		});
	}

	public List<PlayingCard> PlayCard(PlayingCard playingCard, BoardTile boardTile, Hand hand)
	{
		if (gameState != GameState.PLAYING)
		{
			return new();
		}

		var board = Board.GetInstance();

		playingCard.Play();

		board.AddCard(playingCard, boardTile);

		var flippedCards = board.GetFlippedCards(playingCard, rules);
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

		if (board.GetNumFreeTiles() == 0)
		{
			gameState = GameState.FINISH_GAME;
			OnFinishGame?.Invoke(this, EventArgs.Empty);
		}

		hand.Drop(playingCard);

		return flippedCards;
	}
}
