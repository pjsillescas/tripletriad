using Enums;
using System;
using UnityEngine;

public class PlayerController : Controller
{
	private const float MAX_RAYCAST_DISTANCE = 20;

	[SerializeField]
	private Camera mainCamera;

	private InputActions inputs;
	private int cardLayer;
	private int boardTileLayer;
	private int layers;

	private PlayingCard selectedPlayingCard;
	private PlayingCard selectedHoveredCard;
	private BoardTile selectedBoardTile;

	private void Awake()
	{
		inputs = new InputActions();
		inputs.Enable();

		boardTileLayer = LayerMask.NameToLayer("BoardTile");
		cardLayer = LayerMask.NameToLayer("Card");
		layers = (1 << cardLayer) | (1 << boardTileLayer);
	}

	protected override void OnTurnStart(object sender, Team teamTurn)
	{
		selectedPlayingCard = null;
		selectedBoardTile = null;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		Initialize();
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
		if (!IsTeamTurn(Team.Blue))
		{
			return;
		}

		var mousePosition = inputs.UI.Point.ReadValue<Vector2>();
		Ray ray = mainCamera.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, 0));
		BoardTile currentBoardTile = null;
		PlayingCard playingCard = null;
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

			selectedBoardTile = currentBoardTile;

			var board = Board.GetInstance();
			if (selectedPlayingCard != null && selectedBoardTile != null && board.CanPlaceCard(selectedBoardTile))
			{
				PlayCard(selectedPlayingCard, selectedBoardTile);
			}
		}
	}
}
