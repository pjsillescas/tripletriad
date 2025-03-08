using Enums;
using System;
using UnityEngine;

public class PlayerController : Controller
{
	private const float MAX_RAYCAST_DISTANCE = 20;

	private enum PlayerTurnState { SelectCard, FinishTurn }

	[SerializeField]
	private Camera mainCamera;

	private InputActions inputs;
	private int cardLayer;
	private int boardTileLayer;
	private int layers;

	private PlayingCard selectedPlayingCard;
	private BoardTile selectedBoardTile;
	private PlayerTurnState turnState;

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
		turnState = PlayerTurnState.SelectCard;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		Initialize();
	}


	// Update is called once per frame
	void Update()
	{
		if (Team.Red.Equals(gameManager.GetCurrentTeamTurn()))
		{
			return;
		}

		//Vector2 mousePosition1 = Mouse.current.position.ReadValue();
		var mousePosition = inputs.UI.Point.ReadValue<Vector2>();
		//Debug.Log($"{mousePosition} {mousePosition1}");
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
				//Debug.Log($"card {playingCard.GetCardName()}");
			}

			if (hit.collider.gameObject.TryGetComponent<BoardTile>(out var boardTile))
			{
				currentBoardTile = boardTile;
				//var vec = boardTile.GetTileRow();
				//Debug.Log($"tile ({vec.x},{vec.y})");
			}
		}

		if (inputs.Player.Interact.IsPressed())
		{
			switch (turnState)
			{
				case PlayerTurnState.SelectCard:
					if(currentPlayingCard != null && Hand.GetPlayingCards().Contains(currentPlayingCard))
					{
						selectedPlayingCard = currentPlayingCard;
						//Debug.Log($"selected card {selectedPlayingCard.GetCardName()}");
						//turnState = PlayerTurnState.SelectTile;
					}

					selectedBoardTile = currentBoardTile;

					var board = Board.GetInstance();
					if (selectedPlayingCard != null && selectedBoardTile != null && board.CanPlaceCard(selectedBoardTile))
					{
						// 1. Place card in board
						gameManager.PlayCard(selectedPlayingCard, selectedBoardTile, Hand);

						// 3. Finish turn
						turnState = PlayerTurnState.FinishTurn;
					}

					break;
				case PlayerTurnState.FinishTurn:
				default:
					gameManager.StartNextTurn();
					break;
			}
		}
	}
}
