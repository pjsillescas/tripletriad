using cards;
using Enums;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardViewer : MonoBehaviour
{
	private readonly Color BACKGROUND_COLOR = Color.blue;

	[SerializeField]
	private Transform Side1;

	[SerializeField]
	private TextMeshProUGUI NorthText;
	[SerializeField]
	private TextMeshProUGUI EastText;
	[SerializeField]
	private TextMeshProUGUI SouthText;
	[SerializeField]
	private TextMeshProUGUI WestText;

	[SerializeField]
	private GameObject FrontElementPlane;

	[SerializeField]
	private Button ButtonAdd;
	[SerializeField]
	private Button ButtonRemove;

	//private Material sideMat1;

	private ImageLoader imageLoaderFront;
	private ImageLoader imageLoaderElementFront;

	private CardItem cardItem;

	private Func<Card, bool> addCard;
	private Func<Card, bool> removeCard;

	private Image cardImage;
	private Image elementImage;

	/*
	private Material GetMaterial(Transform side)
	{
		return (side != null) ? side.GetComponent<Image>().material : null;
	}
	*/

	public void SetCallbacks(Func<Card, bool> addCard, Func<Card, bool> removeCard)
	{
		this.addCard = addCard;
		this.removeCard = removeCard;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		var imageLoaders = GetComponents<ImageLoader>();
		imageLoaderFront = imageLoaders[0];
		imageLoaderElementFront = imageLoaders[1];

		//sideMat1 = GetMaterial(Side1);
		cardImage = Side1.GetComponent<Image>();
		elementImage = FrontElementPlane.GetComponent<Image>();

		CardItem.OnCardItemClick += OnCardItemClick;
		Side1.gameObject.SetActive(false);

		ButtonAdd.onClick.AddListener(ButtonAddClick);
		ButtonRemove.onClick.AddListener(ButtonRemoveClick);
	}

	private void ButtonRemoveClick()
	{
		int leftUnits = cardItem.GetLeftUnits();
		int numUnits = cardItem.GetNumUnits();
		if (leftUnits >= numUnits)
		{
			return;
		}

		if (removeCard(cardItem.GetCard()))
		{
			SoundManager.GetInstance().Click();

			leftUnits++;
			cardItem.SetLeftUnits(leftUnits);

			ButtonAdd.enabled = leftUnits <= numUnits;
		}
	}

	private void ButtonAddClick()
	{
		int leftUnits = cardItem.GetLeftUnits();
		if (leftUnits <= 0)
		{
			return;
		}

		if (addCard(cardItem.GetCard()))
		{
			SoundManager.GetInstance().Click();

			leftUnits--;
			cardItem.SetLeftUnits(leftUnits);

			ButtonRemove.enabled = leftUnits >= 0;
		}

	}

	private void OnCardItemClick(object sender, CardItem item)
	{
		cardItem = item;
		Load(cardItem.GetCard());
	}

	private void LoadElement(GameObject plane, string set, Card.Element element)
	{
		var elementFileName = $"Sets/{set}/Icons/Tripletriad-{element}.jpeg";
		if (element.Equals(Card.Element.none))
		{
			plane.SetActive(false);
		}
		else
		{
			plane.SetActive(true);
			imageLoaderElementFront.Load(elementFileName,
				(texture) => { elementImage.material.mainTexture = texture; elementImage.SetMaterialDirty(); });
		}
	}
	public void Load(Card card)
	{
		Side1.gameObject.SetActive(true);
		
		var cardName = card.name;
		var element = card.elemental;

		var chars = card.values.ToCharArray();

		cardImage.material.SetColor("_Color", BACKGROUND_COLOR);
		cardImage.SetMaterialDirty();

		var set = card.set;
		var image = ToFileName(cardName);
		var cardFileName = string.Format(card.nameFormat, (set.Equals("Inmoba")) ? image.ToLower() : image);
		var imageName = $"Sets/{set}/Images/{cardFileName}";
		Debug.Log($"loading image '{imageName}'");
		LoadImage(imageName, cardImage);

		LoadElement(FrontElementPlane, set, element);

		NorthText.gameObject.SetActive(true);
		NorthText.text = $"{chars[0]}";
		
		EastText.gameObject.SetActive(true);
		EastText.text = $"{chars[1]}";

		SouthText.gameObject.SetActive(true);
		SouthText.text = $"{chars[2]}";

		WestText.gameObject.SetActive(true);
		WestText.text = $"{chars[3]}";
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

	private void LoadImage(string filename, Image image)
	{
		imageLoaderFront.Load(filename, (texture) => { image.material.mainTexture = texture; image.SetMaterialDirty(); });
	}
}
