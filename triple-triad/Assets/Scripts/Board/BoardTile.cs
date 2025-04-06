using cards;
using UnityEngine;

public class BoardTile : MonoBehaviour
{
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

	private void Start()
	{
		imageLoader = GetComponent<ImageLoader>();
		if(Element == Card.Element.none)
		{
			ElementPlane.gameObject.SetActive(false);
		}
	}

	public void SetElement(Card.Element element)
	{
		var set = SetLoader.GetInstance().Set;
		Element = element;

		if (Element != Card.Element.none)
		{
			ElementPlane.gameObject.SetActive(true);
			var imageFileName = $"Sets/{set}/Icons/Tripletriad-{Element}.jpeg";
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
