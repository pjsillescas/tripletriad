using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	private const float MAX_RAYCAST_DISTANCE = 20;

	[SerializeField]
	private Camera mainCamera;

	private InputActions inputs;
	private int cardLayer;
	private int boardTileLayer;
	private int layers;
	private void Awake()
	{
		inputs = new InputActions();
		inputs.Enable();

		boardTileLayer = LayerMask.NameToLayer("BoardTile");
		cardLayer = LayerMask.NameToLayer("Card");
		layers = (1 << cardLayer) | (1 << boardTileLayer);
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		//Vector2 mousePosition1 = Mouse.current.position.ReadValue();
		var mousePosition = inputs.UI.Point.ReadValue<Vector2>();
		//Debug.Log($"{mousePosition} {mousePosition1}");
		Ray ray = mainCamera.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, 0));
		Debug.DrawLine(ray.origin, ray.direction);
		if (Physics.Raycast(ray, out RaycastHit hit, MAX_RAYCAST_DISTANCE, layers))
		{
			var playingCard = hit.collider.gameObject.GetComponentInParent<PlayingCard>();
			if (playingCard != null)
			{
				Debug.Log($"card {playingCard.GetCardName()}");
			}

			if (hit.collider.gameObject.TryGetComponent<BoardTile>(out var boardTile))
			{
				var vec = boardTile.GetTileRow();
				Debug.Log($"tile ({vec.x},{vec.y})");
			}
		}
	}
}
