using System;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
	public static event EventHandler<PlayingCard> OnCardHovered;
	public static event EventHandler<BoardTile> OnTileHovered;
	public static event EventHandler<PlayingCard> OnCardUnhovered;
	public static event EventHandler<BoardTile> OnTileUnhovered;

	private const float MAX_RAYCAST_DISTANCE = 20;

	private InputActions inputs;
	private int layers;
	private Camera mainCamera;

	private PlayingCard hoveredCard;
	private BoardTile hoveredBoardTile;

	private void Awake()
	{
		inputs = new InputActions();
		inputs.Enable();

		var boardTileLayer = LayerMask.NameToLayer("BoardTile");
		var cardLayer = LayerMask.NameToLayer("Card");
		layers = (1 << cardLayer) | (1 << boardTileLayer);
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		mainCamera = Camera.main;

	}

	private void OnDestroy()
	{
		inputs.Disable();
	}

	// Update is called once per frame
	void Update()
    {
		var mousePosition = inputs.UI.Point.ReadValue<Vector2>();
		Ray ray = mainCamera.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, 0));
		if (Physics.Raycast(ray, out RaycastHit hit, MAX_RAYCAST_DISTANCE, layers))
		{
			var playingCard = hit.collider.gameObject.GetComponentInParent<PlayingCard>();
			SetHoveredCard(playingCard);

			var boardTile = hit.collider.gameObject.GetComponent<BoardTile>();
			SetHoveredTile(boardTile);
		}
	}

	public PlayingCard GetHoveredPlayingCard() => hoveredCard;
	public BoardTile GetHoveredBoardTile() => hoveredBoardTile;

	private void SetHoveredCard(PlayingCard card)
	{
		if (card == null && hoveredCard != null || !Equals(card, hoveredCard))
		{
			if (hoveredCard != null)
			{
				OnCardUnhovered?.Invoke(this, hoveredCard);
			}
			
			hoveredCard = card;
			OnCardHovered?.Invoke(this, hoveredCard);
		}
	}

	private void SetHoveredTile(BoardTile boardTile)
	{
		if (hoveredBoardTile != null && !Equals(boardTile, hoveredBoardTile))
		{
			OnTileUnhovered?.Invoke(this, hoveredBoardTile);
		}
		
		if (boardTile == null && hoveredBoardTile != null || !Equals(boardTile, hoveredBoardTile))
		{
			hoveredBoardTile = boardTile;
			OnTileHovered?.Invoke(this, hoveredBoardTile);
		}

	}
}
