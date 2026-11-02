using cards;
using UnityEngine;

public class BoardTile : MonoBehaviour
{
	private readonly Color HOVER_COLOR = new(0, 0, 0, 0.5f);
	private readonly Color UNHOVER_COLOR = new(0, 0, 0, 0);

	[SerializeField]
	private int Row;
	[SerializeField]
	private int Col;
	[SerializeField]
	private Transform ElementPlane;

	private Card.Element Element = Card.Element.none;
	private ImageLoader imageLoader;

	public Vector2 GetTileRow() => new(Row, Col);
	public Vector3 GetTile() => transform.position;

	private Material material;

	private void OnEnable()
	{
		SelectionManager.OnTileHovered += OnTileHovered;
		SelectionManager.OnTileUnhovered += OnTileUnhovered;
	}

	private void OnDisable()
	{
		SelectionManager.OnTileHovered += OnTileHovered;
		SelectionManager.OnTileUnhovered += OnTileUnhovered;
	}

	private void OnTileHovered(object sender, BoardTile boardTile)
	{
		if (!Equals(this, boardTile))
		{
			return;
		}
		Hover();
	}

	private void OnTileUnhovered(object sender, BoardTile boardTile)
	{
		if (!Equals(this, boardTile))
		{
			return;
		}
		
		Unhover();
	}

	private void Hover()
	{
		material.SetColor("_BaseColor", HOVER_COLOR);
	}

	private void Unhover()
	{
		material.SetColor("_BaseColor", UNHOVER_COLOR);
	}

	private void Start()
	{
		material = GetComponent<MeshRenderer>().material;

		Unhover();
		imageLoader = GetComponent<ImageLoader>();
		if(Element == Card.Element.none)
		{
			ElementPlane.gameObject.SetActive(false);
		}
	}

	public void SetElement(Card.Element element)
	{
		var set = FindAnyObjectByType<SetLoader>().Set;
		Element = element;

		if (Element != Card.Element.none)
		{
			ElementPlane.gameObject.SetActive(true);
			var imageFileName = $"Sets/{set}/Icons/Tripletriad-{Element}.png";
			imageLoader.Load(imageFileName, OnTextureLoad);
		}
		else
		{
			ElementPlane.gameObject.SetActive(false);
		}
	}

	public Card.Element GetElement() => Element;

	private void OnTextureLoad(Texture2D texture)
	{
		ElementPlane.GetComponent<MeshRenderer>().material.mainTexture = texture;
	}
}
