using cards;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardSide : MonoBehaviour
{
	public enum DisplayMode { FRONT, BACK, NONE }
	private const float OUTLINE_WIDTH = 0.05f;

	private readonly Color SELECTED_COLOR = Color.purple;
	private readonly Color HOVERED_COLOR = Color.cyan;

	[SerializeField]
	private Transform Plane;

	[SerializeField]
	private TextMeshPro NorthText;
	[SerializeField]
	private TextMeshPro EastText;
	[SerializeField]
	private TextMeshPro SouthText;
	[SerializeField]
	private TextMeshPro WestText;

	[SerializeField]
	private TextMeshPro ModifierText;

	[SerializeField]
	private GameObject ElementPlane;

	private Material sideMat;

	private string imageFileName;
	private string set;
	private DisplayMode displayMode;
	private Card.Element element;
	private Color backgroundColor;

	private ImageLoader imageLoader;
	private ImageLoader imageLoaderElement;

	private void Awake()
	{
		sideMat = Plane.GetComponent<Renderer>().material;
		displayMode = DisplayMode.NONE;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		var imageLoaders = GetComponents<ImageLoader>();

		imageLoader = imageLoaders[0];
		imageLoaderElement = imageLoaders[1];
	}

	private void LoadElement(GameObject plane, string set, Color color, Card.Element element)
	{
		var elementFileName = $"Sets/{set}/Icons/Tripletriad-{element}.png";
		if (element.Equals(Card.Element.none))
		{
			plane.SetActive(false);
		}
		else
		{
			plane.SetActive(true);
			LoadImageElement(elementFileName, plane.GetComponent<MeshRenderer>().material, color);
		}
	}

	public DisplayMode GetDisplayMode() => displayMode;

	private void SetFileName(Card card)
	{
		var image = ToFileName(card.name);
		var cardFileName = string.Format(card.nameFormat, (set.Equals("Inmoba")) ? image.ToLower() : image);
		imageFileName = $"Sets/{set}/Images/{cardFileName}";
	}

	public void Load(Card card, Color backgroundColor)
	{
		this.backgroundColor = backgroundColor;
		element = card.elemental;
		set = card.set;

		SetModifier(0);

		sideMat.SetColor("_Color", backgroundColor);
		sideMat.SetInt("_UseBackground", 1);

		SetFileName(card);

		var chars = card.values.ToCharArray();
		NorthText.text = $"{chars[0]}";
		EastText.text = $"{chars[1]}";
		SouthText.text = $"{chars[2]}";
		WestText.text = $"{chars[3]}";
	}

	public void Display(DisplayMode displayMode)
	{
		if (displayMode == this.displayMode || displayMode == DisplayMode.NONE)
		{
			return;
		}

		this.displayMode = displayMode;
		if (displayMode == DisplayMode.BACK)
		{
			var backImageFileName = $"Sets/{set}/Images/cardback.png";
			LoadImage(backImageFileName, sideMat);
			ElementPlane.SetActive(false);
		}
		else
		{
			LoadImage(imageFileName, sideMat);
			LoadElement(ElementPlane, set, backgroundColor, element);
		}

		var isActive = DisplayMode.FRONT == displayMode;

		NorthText.gameObject.SetActive(isActive);
		EastText.gameObject.SetActive(isActive);
		SouthText.gameObject.SetActive(isActive);
		WestText.gameObject.SetActive(isActive);

		SetIsSelected(false);
	}

	private string ToFileName(string name)
	{
		Dictionary<string, string> dict = new() {
			{ "Shumi Tribe", "NORG" },
			{ "Tri-Point", "TriPoint" },
			{ "Sphinxara", "Sphinxaur" },
			{ "Blood Soul", "BloudSoul" },
			{ "Tri-Face", "TriFace" },
			{ "T-Rexaur", "TRexaur"},
			{ "Fastitocalon-F", "FastitocalonF"},
			{ "Fujin, Raijin", "FujinRaijin"},
			{ "Wedge, Biggs", "BiggsWedge"},
			{ "Abadon", "Abaddon"},
		};

		return dict.ContainsKey(name) ? dict[name] : name.Replace(" ", "");
	}

	private void LoadImage(string filename, Material material)
	{
		imageLoader.Load(filename, (texture) => material.mainTexture = texture);
	}

	private void LoadImageElement(string filename, Material material, Color color)
	{
		material.SetColor("_Color", color);
		imageLoaderElement.Load(filename, (texture) => material.mainTexture = texture);
	}

	public void SetModifier(int modifier)
	{
		var sign = modifier < 0 ? "-" : "+";
		var modifierText = (modifier == 0) ? "" : $"{sign}{Mathf.Abs(modifier)}";
		ModifierText.text = modifierText;
	}

	public void SetIsSelected(bool isSelected)
	{
		SetOutline(sideMat, (isSelected) ? OUTLINE_WIDTH : 0, SELECTED_COLOR);
	}

	public void SetIsHovered(bool isHovered)
	{
		SetOutline(sideMat, (isHovered) ? OUTLINE_WIDTH : 0, HOVERED_COLOR);
	}

	private void SetOutline(Material material, float width, Color outlineColor)
	{
		material.SetFloat("_OutlineWidth", width);
		material.SetColor("_OutlineColor", outlineColor);
	}
}
