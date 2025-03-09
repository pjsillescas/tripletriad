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
		isFlipping = false;
	}

	// Update is called once per frame
	void Update()
	{
		if (Team.Red.Equals(gameManager.GetCurrentTeamTurn()) || !isControllerEnabled || isFlipping)
		{
			return;
		}

		var mousePosition = inputs.UI.Point.ReadValue<Vector2>();
		Ray ray = mainCamera.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, 0));
		Debug.DrawLine(ray.origin, ray.direction);
		PlayingCard currentPlayingCard = null;
		BoardTile currentBoardTile = null;
		if (Physics.Raycast(ray, out RaycastHit hit, MAX_RAYCAST_DISTANCE, layers))
		{
			var playingCard = hit.collider.gameObject.GetComponentInParent<PlayingCard>();
			if (playingCard != null)
			{
				currentPlayingCard = playingCard;
			}

			if (hit.collider.gameObject.TryGetComponent<BoardTile>(out var boardTile))
			{
				currentBoardTile = boardTile;
			}
		}

		if (inputs.Player.Interact.IsPressed() && numCardsToFlip == 0)
		{
			if (currentPlayingCard != null && Hand.GetPlayingCards().Contains(currentPlayingCard))
			{
				selectedPlayingCard = currentPlayingCard;
			}

			selectedBoardTile = currentBoardTile;

			var board = Board.GetInstance();
			if (selectedPlayingCard != null && selectedBoardTile != null && board.CanPlaceCard(selectedBoardTile))
			{
				var flippedCards = gameManager.PlayCard(selectedPlayingCard, selectedBoardTile, Hand);
				if (flippedCards != null && flippedCards.Count > 0)
				{
					isFlipping = true;
					numCardsToFlip = flippedCards.Count;
					flippedCards.ForEach(card => card.Flip(OnEndFlip));
				}
				else
				{
					EndTurn();
				}
			}
		}
	}

	private void EndTurn()
	{
		gameManager.StartNextTurn();
	}

	private int numCardsToFlip;
	private bool isFlipping;
	private void OnEndFlip()
	{
		numCardsToFlip--;

		if (numCardsToFlip == 0)
		{
			isFlipping = false;
			EndTurn();
		}
	}
}
