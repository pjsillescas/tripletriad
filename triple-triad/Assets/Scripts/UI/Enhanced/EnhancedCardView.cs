using cards;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnhancedCardView : MonoBehaviour
{
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
	private TextMeshProUGUI NameText;

	private ImageLoader imageLoaderFront;
	private ImageLoader imageLoaderElementFront;

	private Image cardImage;
	private Image elementImage;

	public Image GetCardImage() => cardImage;

	private Material material;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		/*
		Debug.Log("start cardview");
		var imageLoaders = GetComponents<ImageLoader>();
		imageLoaderFront = imageLoaders[0];
		imageLoaderElementFront = imageLoaders[1];

		cardImage = Side1.GetComponent<Image>();
		elementImage = FrontElementPlane.GetComponent<Image>();

		Side1.gameObject.SetActive(false);
		*/
	}


	private void LoadElement(GameObject plane, string set, Card.Element element)
	{
		if (element.Equals(Card.Element.none))
		{
			plane.SetActive(false);
		}
		else
		{
			plane.SetActive(true);
			var elementFileName = $"Sets/{set}/Icons/Tripletriad-{element}.png";
			imageLoaderElementFront.Load(elementFileName,
				(texture) => { elementImage.material.mainTexture = texture; elementImage.SetMaterialDirty(); });
		}
	}

	private void Init()
	{
		//gameObject.SetActive(true);
		
		var imageLoaders = GetComponents<ImageLoader>();
		imageLoaderFront = imageLoaders[0];
		imageLoaderElementFront = imageLoaders[1];

		cardImage = Side1.GetComponent<Image>();
		elementImage = FrontElementPlane.GetComponent<Image>();

		//Side1.gameObject.SetActive(false);
		material = cardImage.material;
	}

	public void Load(Card card, Color backgroundColor)
	{
		Init();
		Debug.Log($"load cardview {card.name}");
		//Side1.gameObject.SetActive(true);

		var cardName = card.name;
		var element = card.elemental;

		var chars = card.values.ToCharArray();

		cardImage.material.SetColor("_Color", backgroundColor);
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

		NameText.text = cardName;
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
		Debug.Log($"active {gameObject.name} {gameObject.activeSelf} {gameObject.activeInHierarchy}");
		imageLoaderFront.Load(filename, (texture) => {
			Debug.Log($"image got {filename} '{texture.name}'");

			image.sprite = TextureUtils.TextureToSprite(texture);
			
			image.SetMaterialDirty();
		});
	}
}
