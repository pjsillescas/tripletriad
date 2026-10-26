using Enums;
using UnityEngine;

public class PlayerController : Controller
{
	private const float MAX_RAYCAST_DISTANCE = 20;

	private InputActions inputs;
	private int layers;

	private PlayingCard selectedPlayingCard;
	private PlayingCard selectedHoveredCard;
	private Camera mainCamera;

	private void Awake()
	{
		inputs = new InputActions();
		inputs.Enable();

		var boardTileLayer = LayerMask.NameToLayer("BoardTile");
		var cardLayer = LayerMask.NameToLayer("Card");
		layers = (1 << cardLayer) | (1 << boardTileLayer);
	}

	protected override void OnTurnStart(object sender, Team teamTurn)
	{
		selectedPlayingCard = null;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		controlledTeam = Team.Blue;
		mainCamera = Camera.main;
		Initialize();
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

		BoardTile currentBoardTile = null;
		PlayingCard playingCard = null;
		
		
		var mousePosition = inputs.UI.Point.ReadValue<Vector2>();
		Ray ray = mainCamera.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, 0));
		if (Physics.Raycast(ray, out RaycastHit hit, MAX_RAYCAST_DISTANCE, layers))
		{
			playingCard = hit.collider.gameObject.GetComponentInParent<PlayingCard>();
			if (playingCard != null && selectedHoveredCard != playingCard)
			{
				SetSelectedHoveredCard(playingCard);
			}

			if (hit.collider.gameObject.TryGetComponent<BoardTile>(out var boardTile))
			{
				currentBoardTile = boardTile;
			}
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
