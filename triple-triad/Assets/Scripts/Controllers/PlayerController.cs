using Enums;

public class PlayerController : Controller
{
	private InputActions inputs;

	private PlayingCard selectedPlayingCard;
	private PlayingCard selectedHoveredCard;
	private SelectionManager selectionManager;

	private void Awake()
	{
		controlledTeam = Team.Blue;
		
		inputs = new InputActions();
		inputs.Enable();
	}

	protected override void OnTurnStart(object sender, Team teamTurn)
	{
		selectedPlayingCard = null;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		Initialize();

		selectionManager = FindAnyObjectByType<SelectionManager>();
	}

	private void OnDestroy()
	{
		inputs.Disable();
	}

	private void SetSelectedHoveredCard(PlayingCard playingCard)
	{
		if (selectedHoveredCard != null)
		{
			selectedHoveredCard.SetIsHovered(false);
		}
		selectedHoveredCard = playingCard;
		selectedHoveredCard.SetIsHovered(true);
	}
	
	private void SelectCurrentHoveredCard()
	{
		if (selectedPlayingCard != null)
		{
			selectedPlayingCard.SetIsSelected(false);
		}
		
		selectedPlayingCard = selectedHoveredCard;

		selectedPlayingCard.SetIsSelected(true);
	}

	// Update is called once per frame
	void Update()
	{
		if (!IsTeamTurn(controlledTeam))
		{
			return;
		}

		var playingCard = selectionManager.GetHoveredPlayingCard();
		var currentBoardTile = selectionManager.GetHoveredBoardTile();

		if (playingCard != null && selectedHoveredCard != playingCard)
		{
			SetSelectedHoveredCard(playingCard);
		}

		if (playingCard == null && selectedHoveredCard != null)
		{
			selectedHoveredCard.SetIsHovered(false);
			selectedHoveredCard = null;
		}

		if (inputs.Player.Interact.IsPressed() && !ThereAreCardsToFlip())
		{
			if (selectedHoveredCard != null && selectedHoveredCard != selectedPlayingCard &&
				Hand.GetPlayingCards().Contains(selectedHoveredCard))
			{
				SelectCurrentHoveredCard();
				SoundManager.GetInstance().Click();
			}

			PlayCard(selectedPlayingCard, currentBoardTile);
		}
	}
}
