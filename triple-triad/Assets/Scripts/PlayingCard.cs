using cards;
using NUnit.Framework;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayingCard : MonoBehaviour
{
	[SerializeField]
	private Transform Side1;
	[SerializeField]
	private Transform Side2;

	[SerializeField]
	private TextMeshPro NorthText;
	[SerializeField]
	private TextMeshPro EastText;
	[SerializeField]
	private TextMeshPro SouthText;
	[SerializeField]
	private TextMeshPro WestText;

	[SerializeField]
	private TextMeshPro NorthBackText;
	[SerializeField]
	private TextMeshPro EastBackText;
	[SerializeField]
	private TextMeshPro SouthBackText;
	[SerializeField]
	private TextMeshPro WestBackText;
	
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

		var chars = card.values.ToCharArray();
		var values = chars.Select(c => c == 'A' ? 10 : c - '0').ToList();
		north = values[0];
		east = values[1];
		south = values[2];
		west = values[3];

		LoadImage("FFVIII", ToFileName(cardName), sideMat1);
		LoadImage("FFVIII", ToFileName(cardName), sideMat2);

		NorthText.text = $"{chars[0]}";
		EastText.text = $"{chars[1]}";
		SouthText.text = $"{chars[2]}";
		WestText.text = $"{chars[3]}";

		NorthBackText.text = $"{chars[0]}";
		EastBackText.text = $"{chars[1]}";
		SouthBackText.text = $"{chars[2]}";
		WestBackText.text = $"{chars[3]}";
	}

	private string ToFileName(string name)
	{
		return name.Replace(" ","");
	}

	private void LoadImage(string set, string image, Material material)
	{
		
		string filename = $"Assets/Data/Sets/{set}/Images/TT{image}.jpg";
		var bytes = System.IO.File.ReadAllBytes(filename);
		var myTexture = new Texture2D(1, 1);
		myTexture.LoadImage(bytes);

		material.mainTexture = myTexture;
	}

	public override string ToString()
	{
		var elementStr = Card.Element.none.Equals(element) ? "" : element.ToString();
		return $"{cardName}[{elementStr}] => north: {north} east: {east} south: {south} west: {west}";
	}
}
