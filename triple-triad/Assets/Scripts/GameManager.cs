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
	private enum GameManagerType { CLASSIC, ENHANCED }
	private enum GameState { INITIALIZATION, PLAYING, FINISH_GAME }
	private enum InitGameState { NONE, PLAYER_INIT, ADVERSARY_INIT, FINISH_INIT }

	[SerializeField]
	private GameManagerType ManagerType;

	[SerializeField]
	private SetLoader Loader;
	[SerializeField]
	private int NumCardsPerHand;
	[SerializeField]
	private Hand PlayerHand;
	[SerializeField]
	private Hand AdversaryHand;
	[SerializeField]
	private Controller BlueController;
	[SerializeField]
	private Controller RedController;
	[SerializeField]
	private ManualHandWidget ManualHandWidget;
	[SerializeField]
	private NewGameWidget NewGameWidget;
	[SerializeField]
	private CreateGameWidget CreateGameWidget;
	[SerializeField]
	private EnhancedHandWidget EnhancedHandWidget;

	public event EventHandler<Team> OnNewTurn;
	public event EventHandler OnFinishGame;
	public event EventHandler<Score> OnEnhancedFinishGame;
	public event EventHandler OnStartGame;
	public event EventHandler<Score> OnScoreChange;


	private static GameManager Instance = null;

	private Team currentTeamTurn;
	private int blueScore;
	private int redScore;

	private GameState gameState;
	private InitGameState initGameState;

	private TurnManager turnManager;

	private HandSelectionType handSelectionType;

	private List<IRuleVariation> rules;

	private Team localTeam;
	private List<Card> cards;


	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("GameManager instance duplicated");
		}

		Instance = this;
		currentTeamTurn = Team.None;

		localTeam = Team.Blue;
	}

	private bool IsClassicManager() => ManagerType == GameManagerType.CLASSIC;

	public static GameManager GetInstance() => Instance;

	public Team GetCurrentTeamTurn() => currentTeamTurn;

	public Team GetLocalTeam() => localTeam;

	private void OnSetLoadedClassic(object sender, List<Card> cards)
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
	}

	private void OnSetLoadedEnhanced(object sender, List<Card> cards)
	{
		EnhancedHandWidget.ActivateWidget();
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		currentTeamTurn = Team.None;

		SetLoader.OnSetLoaded += IsClassicManager() ? OnSetLoadedClassic : OnSetLoadedEnhanced;

		PlayerHand.OnHandLoaded += OnHandLoaded;
		AdversaryHand.OnHandLoaded += OnHandLoaded;

		turnManager = GetComponent<TurnManager>();

		if (!IsClassicManager())
		{
			EnhancedHandWidget.OnHandChosen += OnEnhancedPlayerHandChosen;
			CreateGameWidget.OnNewGame += OnEnhancedNewGame;
		}

		NewGame();
	}

	private void OnDestroy()
	{
		SetLoader.OnSetLoaded -= IsClassicManager() ? OnSetLoadedClassic : OnSetLoadedEnhanced;
		EnhancedHandWidget.OnHandChosen -= OnEnhancedPlayerHandChosen;
		CreateGameWidget.OnNewGame -= OnEnhancedNewGame;
	}

	private Controller CreateController(Team team, GameConfiguration.TeamConfiguration teamConfiguration)
	{
		Controller controller = null;
		switch(teamConfiguration.controllerType)
		{
			case GameConfiguration.ControllerType.LOCAL_PLAYER:
				controller = new GameObject("local player controller").AddComponent<PlayerController>();
				break;
			case GameConfiguration.ControllerType.REMOTE_PLAYER:
				controller = new GameObject("remote player controller").AddComponent<PlayerController>();
				break;
			case GameConfiguration.ControllerType.IA:
				controller = new GameObject("ia controller").AddComponent<AdversaryController>();
				break;
		}

		controller.Init(team, teamConfiguration.hand, teamConfiguration.middlePosition);

		return controller;
	}

	private void InitializeControllers()
	{
		var gameConfiguration = FindAnyObjectByType<GameConfiguration>();

		if (gameConfiguration == null)
		{
			return;
		}

		var blueConfiguration = gameConfiguration.GetBlueConfiguration();
		var redConfiguration = gameConfiguration.GetRedConfiguration();

		if (blueConfiguration.controllerType == GameConfiguration.ControllerType.LOCAL_PLAYER)
		{
			localTeam = Team.Blue;
		}
		else
		{
			localTeam = Team.Red;
		}

		BlueController = CreateController(Team.Blue, blueConfiguration);
		RedController = CreateController(Team.Red, redConfiguration);
	}

	public void NewGame()
	{
		gameState = GameState.INITIALIZATION;
		initGameState = InitGameState.NONE;

		if (IsClassicManager())
		{
			NewGameWidget.ActivateWidget(OnClassicNewGame);
		}
		else
		{
			CreateGameWidget.ActivateWidget();
		}
	}

	private void OnHandLoaded(object sender, Hand hand)
	{
		Debug.Log("on hand loaded");
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
		BlueController.ResetController();
		RedController.ResetController();
	}

	private Score GetScore() => new() { blue = blueScore, red = redScore, localTeam = localTeam };

	private List<Card> GetRandomHand()
	{
		return HandUtils.GetRandomHand(Loader, NumCardsPerHand);
	}

	private void OnEnhancedNewGame(object sender, CreateGameWidget.OnNewGameData data)
	{
		InitializeControllers();
		rules = data.rules;
		Board.GetInstance().Initialize();

		FindAnyObjectByType<SetLoader>().LoadSet(data.set);
	}

	private void OnClassicNewGame(string setName, HandSelectionType handSelectionType, List<IRuleVariation> rules)
	{
		this.handSelectionType = handSelectionType;
		this.rules = rules;
		Board.GetInstance().Initialize();

		FindAnyObjectByType<SetLoader>().LoadSet(setName);
	}

	private bool GetUseCardBack()
	{
		if (rules?.Count > 0)
		{
			return rules.Select(rule => rule.UseCardBack()).Aggregate(true, (acc, value) => acc && value);
		}

		return true;
	}

	public bool ControllerUsesCardBack(Controller controller)
	{
		return (controller is PlayerController) ? false : GetUseCardBack();
	}

	public void InitializeGame()
	{
		currentTeamTurn = turnManager.ResetTurn();

		gameState = GameState.INITIALIZATION;
		initGameState = InitGameState.NONE;

		//var blueUseCardback = (BlueController is PlayerController) ? false : GetUseCardBack();
		//var redUseCardback = (RedController is PlayerController) ? false : GetUseCardBack();
		var blueUseCardback = ControllerUsesCardBack(BlueController);
		var redUseCardback = ControllerUsesCardBack(RedController);

		var blueIsPlayer = BlueController is PlayerController;

		var iaCards = GetRandomHand();
		var blueCards = blueIsPlayer ? cards : iaCards;
		var redCards = !blueIsPlayer ? cards : iaCards;

		PlayerHand.Initialize(blueCards, blueUseCardback, blueIsPlayer);
		AdversaryHand.Initialize(redCards, redUseCardback, RedController is PlayerController);

		ResetScore();
		OnScoreChange?.Invoke(this, GetScore());

		rules?.ForEach(rule => rule.Initialize());
	}

	private void ResetScore()
	{
		blueScore = 5;
		redScore = 5;
	}

	public void FinalizeGame()
	{
		currentTeamTurn = turnManager.ResetTurn();
	}

	private void OnPlayerHandChosen(List<Card> cards)
	{
		this.cards = cards;

		if (IsClassicManager())
		{
			InitializeGame();
		}

		OnStartGame?.Invoke(this, EventArgs.Empty);
	}

	private void OnEnhancedPlayerHandChosen(object sender, List<Card> cards)
	{
		OnPlayerHandChosen(cards);
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

		hand.Drop(playingCard);
		var board = Board.GetInstance();

		playingCard.Play();

		board.AddCard(playingCard, boardTile);

		var flippedCards = board.GetFlippedCards(playingCard, rules);
		if (flippedCards.Count > 0)
		{
			if (playingCard.GetCurrentTeam().Equals(Team.Blue))
			{
				blueScore += flippedCards.Count;
				redScore -= flippedCards.Count;
			}
			else
			{
				blueScore -= flippedCards.Count;
				redScore += flippedCards.Count;
			}

			flippedCards.ForEach(card => card.SetCurrentTeam(playingCard.GetCurrentTeam()));
			OnScoreChange?.Invoke(this, GetScore());
		}

		if (board.GetNumFreeTiles() == 0)
		{
			gameState = GameState.FINISH_GAME;
			OnFinishGame?.Invoke(this, EventArgs.Empty);
			OnEnhancedFinishGame?.Invoke(this, GetScore());
		}

		return flippedCards;
	}
}
