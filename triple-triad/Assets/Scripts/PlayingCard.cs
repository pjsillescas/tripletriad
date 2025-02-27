using cards;
using NUnit.Framework;
using System.Linq;
using UnityEngine;

public class PlayingCard : MonoBehaviour
{
	[SerializeField]
	private Transform Side1;
	[SerializeField]
	private Transform Side2;

	private Material sideMat1;
	private Material sideMat2;

	private int north;
	private int east;
	private int south;
	private int west;
	private string cardName;
	private Card.Element element;


	private Material GetMaterial(Transform side)
	{
		return (side != null) ? side.GetComponent<Renderer>().material : null;
	}

	private void Awake()
	{
		sideMat1 = GetMaterial(Side1);
		sideMat2 = GetMaterial(Side2);
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	public void Load(Card card)
	{
		cardName = card.name;
		element = card.element;

		var values = card.values.ToCharArray().Select(c => c == 'A' ? 10 : c - '0').ToList();
		north = values[0];
		east = values[1];
		south = values[2];
		west = values[3];
	}

	public override string ToString()
	{
		var elementStr = Card.Element.none.Equals(element) ? "" : element.ToString();
		return $"{cardName}[{elementStr}] => north: {north} east: {east} south: {south} west: {west}";
	}
}
