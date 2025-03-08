using cards;
using Enums;
using NUnit.Framework;
using System.Collections.Generic;
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

	[SerializeField]
	private TextMeshPro FrontTeamText;
	[SerializeField]
	private TextMeshPro BackTeamText;

	private Material sideMat1;
	private Material sideMat2;

	private int north;
	private int east;
	private int south;
	private int west;
	private string cardName;
	private Card.Element element;
	private Team team;
	private string imageFileName;

	private Team currentTeam;
	private bool isPlayed;


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

	public PlayingCard Load(Card card, Team team, bool useBackImage)
	{
		cardName = card.name;
		element = card.element;

		var chars = card.values.ToCharArray();
		var values = chars.Select(c => c == 'A' ? 10 : c - '0').ToList();
		north = values[0];
		east = values[1];
		south = values[2];
		west = values[3];

		var set = "FFVIII";
		var image = ToFileName(cardName);
		imageFileName = $"Assets/Data/Sets/{set}/Images/TT{image}.jpg";
		LoadImage(imageFileName, sideMat1);

		NorthText.text = $"{chars[0]}";
		EastText.text = $"{chars[1]}";
		SouthText.text = $"{chars[2]}";
		WestText.text = $"{chars[3]}";

		if (useBackImage)
		{
			var backImageFileName = $"Assets/Data/Sets/{set}/Images/cardback.png";
			LoadImage(backImageFileName, sideMat2);
			NorthBackText.text = "";
			EastBackText.text = "";
			SouthBackText.text = "";
			WestBackText.text = "";
		}
		else
		{
			LoadImage(imageFileName, sideMat2);
			NorthBackText.text = NorthText.text;
			EastBackText.text = EastText.text;
			SouthBackText.text = SouthText.text;
			WestBackText.text = WestText.text;
		}

		this.team = team;
		SetCurrentTeam(team);
		isPlayed = false;

		return this;
	}

	public void Play()
	{
		SetCurrentTeam(team);

		LoadImage(imageFileName, sideMat2);
		NorthBackText.text = NorthText.text;
		EastBackText.text = EastText.text;
		SouthBackText.text = SouthText.text;
		WestBackText.text = WestText.text;
		
		isPlayed = true;
	}

	private string ToFileName(string name)
	{
		Dictionary<string, string> dict = new () {
			{ "Shumi Tribe", "NORG" },
			{ "Tri-Point", "Tripoint" },
			{ "Sphinxara", "Sphinxaur" },
			{ "Blood Soul", "Bloudsoul" },
			{ "Tri-Face", "TriFace" },
			{ "T-Rexaur", "TRexaur"},
			{ "Fastitocalon-F", "FastitocalonF"},
			{ "Fujin, Raijin", "FujinRaijin"},
			{ "Wedge, Biggs", "BiggsWedge"},
			{ "Abadon", "Abaddon"},
		};

		return dict.ContainsKey(name) ? dict[name] : name.Replace(" ","");
	}

	private void LoadImage(string set, string image, Material material)
	{
		
		string filename = $"Assets/Data/Sets/{set}/Images/TT{image}.jpg";
		LoadImage(filename, material) ;
	}
	private void LoadImage(string filename, Material material)
	{
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

	public string GetCardName() => cardName;

	public int GetNorth() => north;
	public int GetEast() => east;
	public int GetSouth() => south;
	public int GetWest() => west;

	public bool IsPlayed() => isPlayed;

	public Team GetCurrentTeam() => currentTeam;
	public void SetCurrentTeam(Team newTeam)
	{
		currentTeam = newTeam;
		BackTeamText.text = newTeam.ToString();
		FrontTeamText.text = newTeam.ToString();
	}

	public void SetIsPlayed(bool isPlayed)
	{
		this.isPlayed = isPlayed;
	}
}
