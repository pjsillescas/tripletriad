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
	private PlayerController PlayerController;
	[SerializeField]
	private AdversaryController AdversaryController;
	[SerializeField]
	private ManualHandWidget ManualHandWidget;
	[SerializeField]
	private NewGameWidget NewGameWidget;
	[SerializeField]
	private CreateGameWidget CreateGameWidget;
	[SerializeField]
	private EnhancedHandWidget EnhancedHandWidget;

	public class Score
	{
		public int player;
		public int adversary;
	};

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

	private EnhancedController blueController;
	private EnhancedController redController;
	private List<Card> cards;
	private Team team;
	private GameConfiguration gameConfiguration;
	private EnhancedHandWidget.HandWidgetMode handWidgetMode;

	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("GameManager instance duplicated");
		}

		Instance = this;
		currentTeamTurn = Team.None;
		SetTeam(Team.None);
	}

	public void SetTeam(Team team)
	{
		this.team = team;
	}

	public void SetHandWidgetMode(EnhancedHandWidget.HandWidgetMode handWidgetMode)
	{
		this.handWidgetMode = handWidgetMode;
	}

	private bool IsClassicManager() => ManagerType == GameManagerType.CLASSIC;

	public static GameManager GetInstance() => Instance;

	public Team GetCurrentTeamTurn() => currentTeamTurn;

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
		if(team == Team.None)
		{
			Debug.LogError("Team not assigned in manager");
			return;
		}

		EnhancedHandWidget.ActivateWidget(team, handWidgetMode);
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		currentTeamTurn = Team.None;

		SetLoader.OnSetLoaded += IsClassicManager() ? OnSetLoadedClassic : OnSetLoadedEnhanced;

		if (IsClassicManager())
		{
			PlayerHand.OnHandLoaded += OnHandLoaded;
			AdversaryHand.OnHandLoaded += OnHandLoaded;
		}
		else
		{
			EnhancedHandWidget.OnHandChosen += OnEnhancedPlayerHandChosen;
			CreateGameWidget.OnNewGame += OnEnhancedNewGame;
		}

		turnManager = GetComponent<TurnManager>();

		gameConfiguration = FindAnyObjectByType<GameConfiguration>();

		NewGame();
	}

	private void OnDestroy()
	{
		SetLoader.OnSetLoaded -= IsClassicManager() ? OnSetLoadedClassic : OnSetLoadedEnhanced;
		EnhancedHandWidget.OnHandChosen -= OnEnhancedPlayerHandChosen;
		CreateGameWidget.OnNewGame -= OnEnhancedNewGame;
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
		if (IsClassicManager())
		{
			PlayerController.ResetController();
			AdversaryController.ResetController();
		}
		else
		{
			blueController.ResetController();
			redController.ResetController();
		}
	}

	public Score GetScore() => new() { player = blueScore, adversary = redScore };

	private List<Card> GetRandomHand()
	{
		return HandUtils.GetRandomHand(Loader, NumCardsPerHand);
	}

	private void OnEnhancedNewGame(object sender, CreateGameWidget.OnNewGameData data)
	{
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

	private bool GetUseCardBackAdversary()
	{
		if (rules?.Count > 0)
		{
			return rules.Select(rule => rule.UseCardBack()).Aggregate(true, (acc, value) => acc && value);
		}

		return true;
	}


	public void InitializeGame()
	{
		//currentTeamTurn = turnManager.ResetTurn();

		gameState = GameState.INITIALIZATION;
		initGameState = InitGameState.NONE;
		
		if (IsClassicManager())
		{
			PlayerHand.Initialize(cards, false);
			AdversaryHand.Initialize(GetRandomHand(), GetUseCardBackAdversary());
		}

		PlayerHand.Display();
		AdversaryHand.Display();

		ResetScore();
		OnScoreChange?.Invoke(this, GetScore());

		rules?.ForEach(rule => rule.Initialize());
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

	private void OnEnhancedPlayerHandChosen(object sender, EnhancedHandWidget.HandConfigurationData handData)
	{
		InitializeEnhancedGame(handData);
		OnStartGame?.Invoke(this, EventArgs.Empty);
	}

	private void InitializeEnhancedGame(EnhancedHandWidget.HandConfigurationData handData)
	{
		var team = handData.team;
		currentTeamTurn = turnManager.ResetTurn();

		gameState = GameState.INITIALIZATION;
		var controllerType = handData.type;
		var hand = gameConfiguration.GetHand(team);
		var middlePosition = gameConfiguration.GetMiddlePosition(team);
			
		EnhancedController controller = null;

		if (controllerType == ControllerType.PLAYER)
		{
			cards = handData.cards;
			controller = new GameObject("player controller").AddComponent<EnhancedPlayerController>();
			hand.Initialize(cards, false);
		}
		else if (controllerType == ControllerType.IA)
		{
			controller = new GameObject("ia controller").AddComponent<EnhancedIAController>();
			hand.Initialize(GetRandomHand(), GetUseCardBackAdversary());
		}
		else if (controllerType == ControllerType.REMOTE_PLAYER)
		{
			controller = new GameObject("remote player controller").AddComponent<EnhancedIAController>();
			hand.Initialize(GetRandomHand(), GetUseCardBackAdversary());
		}
		else
		{
			Debug.LogError($"wrong controller '{controllerType}'");
		}

		controller.Initialize(hand, middlePosition, team);

		if (handData.team == Team.Blue)
		{
			blueController = controller;
		}
		else if (handData.team == Team.Red)
		{
			redController = controller;
		}
		else
		{
			throw new Exception("Invalid team in hand");
		}

		if (blueController != null && redController != null)
		{
			ResetScore();
			OnScoreChange?.Invoke(this, GetScore());

			rules?.ForEach(rule => rule.Initialize());

			FinishInitialization();
		}
	}

	private void ResetScore()
	{
		blueScore = 5;
		redScore = 5;
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
			if (playingCard.GetCurrentTeam() == Team.Blue)
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
			SetTeam(Team.None);
			gameState = GameState.FINISH_GAME;
			OnFinishGame?.Invoke(this, EventArgs.Empty);
			OnEnhancedFinishGame?.Invoke(this, GetScore());
		}

		return flippedCards;
	}
}
