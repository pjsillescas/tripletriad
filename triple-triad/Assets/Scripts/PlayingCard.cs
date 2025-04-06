using cards;
using Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayingCard : MonoBehaviour
{
	private const float OUTLINE_WIDTH = 0.05f;

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

	[SerializeField]
	private TextMeshPro FrontModifierText;
	[SerializeField]
	private TextMeshPro BackModifierText;

	[SerializeField]
	private GameObject FrontElementPlane;
	[SerializeField]
	private GameObject BackElementPlane;

	private Material sideMat1;
	private Material sideMat2;

	private int north;
	private int east;
	private int south;
	private int west;
	private int modifier;
	private string cardName;
	private Card.Element element;
	private Team team;
	private string imageFileName;
	private string set;

	private Team currentTeam;
	private bool isPlayed;
	private CardFlip cardFlip;
	private CardTravel cardTravel;
	private bool isBackShown;
	private bool isSelected;

	private ImageLoader imageLoaderFront;
	private ImageLoader imageLoaderBack;
	private ImageLoader imageLoaderElementFront;
	private ImageLoader imageLoaderElementBack;

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
		cardFlip = GetComponent<CardFlip>();
		cardTravel = GetComponent<CardTravel>();
		var imageLoaders = GetComponents<ImageLoader>();

		imageLoaderFront = imageLoaders[0];
		imageLoaderBack = imageLoaders[1];
		imageLoaderElementFront = imageLoaders[2];
		imageLoaderElementBack = imageLoaders[3];
		isBackShown = false;
	}

	public void FlipIsBackShown()
	{
		isBackShown = !isBackShown;

		RefreshTexts();
	}

	private void RefreshTexts()
	{
		NorthText.gameObject.SetActive(!isBackShown);
		EastText.gameObject.SetActive(!isBackShown);
		SouthText.gameObject.SetActive(!isBackShown);
		WestText.gameObject.SetActive(!isBackShown);
		FrontTeamText.gameObject.SetActive(!isBackShown);

		NorthBackText.gameObject.SetActive(isPlayed && isBackShown);
		EastBackText.gameObject.SetActive(isPlayed && isBackShown);
		SouthBackText.gameObject.SetActive(isPlayed && isBackShown);
		WestBackText.gameObject.SetActive(isPlayed && isBackShown);
		BackTeamText.gameObject.SetActive(isPlayed && isBackShown);
	}

	public void Flip(Action onEndFlip)
	{
		FlipIsBackShown();
		cardFlip.Flip(onEndFlip);
	}

	public void Travel(Vector3 endPosition, Vector3 middlePosition, Action onEndTravel)
	{
		cardTravel.Travel(endPosition, middlePosition, onEndTravel);
	}

	// Update is called once per frame
	void Update()
	{

	}

	private readonly Color PLAYER_BACKGROUND_COLOR = Color.blue;
	private readonly Color ADVERSARY_BACKGROUND_COLOR = Color.red;

	private void LoadElement(GameObject plane, string set, bool useFront)
	{
		var elementFileName = $"Sets/{set}/Icons/Tripletriad-{element}.jpeg";
		if (element.Equals(Card.Element.none))
		{
			plane.SetActive(false);
		}
		else
		{
			plane.SetActive(true);
			LoadImageElement(elementFileName, plane.GetComponent<MeshRenderer>().material, useFront);
		}
	}
	public PlayingCard Load(Card card, Team team, bool useBackImage)
	{
		cardName = card.name;
		element = card.elemental;

		Debug.Log(card);
		//element = Card.Element.wind;

		var chars = card.values.ToCharArray();
		var values = chars.Select(c => c == 'A' ? 10 : c - '0').ToList();
		north = values[0];
		east = values[1];
		south = values[2];
		west = values[3];

		SetModifier(0);

		sideMat1.SetColor("_Color", PLAYER_BACKGROUND_COLOR);
		sideMat2.SetColor("_Color", ADVERSARY_BACKGROUND_COLOR);

		set = card.set;
		var image = ToFileName(cardName);
		var cardFileName = string.Format(card.nameFormat, (set.Equals("Inmoba")) ? image.ToLower() : image);
		imageFileName = $"Sets/{set}/Images/{cardFileName}";
		LoadImage(imageFileName, sideMat1, true);

		LoadElement(FrontElementPlane, set, true);

		NorthText.text = $"{chars[0]}";
		EastText.text = $"{chars[1]}";
		SouthText.text = $"{chars[2]}";
		WestText.text = $"{chars[3]}";

		isPlayed = false;
		isBackShown = useBackImage;
		if (useBackImage)
		{
			var backImageFileName = $"Sets/{set}/Images/cardback.png";
			LoadImage(backImageFileName, sideMat2, false);
			BackElementPlane.SetActive(false);

			NorthBackText.text = "";
			EastBackText.text = "";
			SouthBackText.text = "";
			WestBackText.text = "";
		}
		else
		{
			LoadImage(imageFileName, sideMat2, false);
			LoadElement(BackElementPlane, set, false);
			NorthBackText.text = NorthText.text;
			EastBackText.text = EastText.text;
			SouthBackText.text = SouthText.text;
			WestBackText.text = WestText.text;
		}

		RefreshTexts();

		this.team = team;
		SetCurrentTeam(team);
		SetIsSelected(false);
		return this;
	}

	public void Play()
	{
		SetCurrentTeam(team);

		LoadImage(imageFileName, sideMat2, false);
		LoadElement(BackElementPlane, set, false);

		Debug.Log("playing card " + cardName);

		NorthBackText.gameObject.SetActive(true);
		EastBackText.gameObject.SetActive(true);
		SouthBackText.gameObject.SetActive(true);
		WestBackText.gameObject.SetActive(true);

		NorthText.gameObject.SetActive(true);
		EastText.gameObject.SetActive(true);
		SouthText.gameObject.SetActive(true);
		WestText.gameObject.SetActive(true);
		
		BackTeamText.gameObject.SetActive(true);
		FrontTeamText.gameObject.SetActive(true);

		NorthBackText.text = NorthText.text;
		EastBackText.text = EastText.text;
		SouthBackText.text = SouthText.text;
		WestBackText.text = WestText.text;

		/*
		NorthText.text = NorthText.text;
		EastText.text = EastText.text;
		SouthText.text = SouthText.text;
		WestText.text = WestText.text;
		*/

		isPlayed = true;
		SetIsSelected(false);
	}

	private string ToFileName(string name)
	{
		Dictionary<string, string> dict = new() {
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
			{ "Tripoint", "TTTriPoint" },
		};

		return dict.ContainsKey(name) ? dict[name] : name.Replace(" ", "");
	}

	private void LoadImage(string filename, Material material, bool useFront)
	{
		((useFront) ? imageLoaderFront : imageLoaderBack).Load(filename, (texture) => material.mainTexture = texture);
		//material.mainTexture = myTexture;
	}

	private void LoadImageElement(string filename, Material material, bool useFront)
	{
		((useFront) ? imageLoaderElementFront : imageLoaderElementBack).Load(filename, (texture) => material.mainTexture = texture);
		//material.mainTexture = myTexture;
	}

	public void SetModifier(int modifier)
	{
		this.modifier = modifier;

		var sign = modifier < 0 ? "-" : "+";
		var modifierText = (modifier == 0) ? "" : $"{sign}{Math.Abs(modifier)}";
		FrontModifierText.text = modifierText;
		BackModifierText.text = modifierText;
	}

	public string GetCardName() => cardName;

	public int GetNorth() => north + modifier;
	public int GetEast() => east + modifier;
	public int GetSouth() => south + modifier;
	public int GetWest() => west + modifier;

	public bool IsPlayed() => isPlayed;

	public Team GetCurrentTeam() => currentTeam;
	public Card.Element GetElement() => element;
	public void SetCurrentTeam(Team newTeam)
	{
		currentTeam = newTeam;
		BackTeamText.text = newTeam.ToString();
		FrontTeamText.text = newTeam.ToString();
	}

	public void SetIsPlayed(bool isPlayed)
	{
		this.isPlayed = isPlayed;

		if (isPlayed)
		{
			SetIsSelected(false);
		}
	}

	private readonly Color SELECTED_COLOR = Color.red;
	private readonly Color HOVERED_COLOR = Color.cyan;

	public void SetIsSelected(bool isSelected)
	{
		this.isSelected = isSelected;
		if (isSelected)
		{
			//sideMat1.SetFloat("Outline Width", 0.05f);
			//sideMat1.SetColor("Outline Color", Color.red);
			SetOutline(sideMat1, OUTLINE_WIDTH, SELECTED_COLOR);
			SetOutline(sideMat2, OUTLINE_WIDTH, SELECTED_COLOR);
		}
		else
		{
			//sideMat1.SetFloat("Outline Width", 0);
			SetOutline(sideMat1, 0, SELECTED_COLOR);
			SetOutline(sideMat2, 0, SELECTED_COLOR);
		}
	}

	public void SetIsHovered(bool isHovered)
	{
		if (CanBeHovered())
		{
			if (isHovered)
			{
				SetOutline(sideMat1, OUTLINE_WIDTH, HOVERED_COLOR);
				SetOutline(sideMat2, OUTLINE_WIDTH, HOVERED_COLOR);
			}
			else
			{
				//sideMat1.SetFloat("Outline Width", 0);
				SetOutline(sideMat1, 0, HOVERED_COLOR);
				SetOutline(sideMat2, 0, HOVERED_COLOR);
			}
		}
	}

	private bool CanBeHovered()
	{
		return !isBackShown && !isSelected && team.Equals(Team.Blue);
	}

	private void SetOutline(Material material, float width, Color outlineColor)
	{
		material.SetFloat("_OutlineWidth", width);
		material.SetColor("_OutlineColor", outlineColor);
	}
}