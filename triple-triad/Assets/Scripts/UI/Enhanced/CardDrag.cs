using cards;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDrag : MonoBehaviour
{
	[SerializeField]
	private Image UnitImage;
	[SerializeField]
	private TextMeshProUGUI NameText;

	private RectTransform rectTransform;
	private CanvasGroup canvasGroup;
	private Card data;

	private void Awake()
	{
		UnitImage.enabled = false;
		UnitImage.gameObject.SetActive(false);

		rectTransform = GetComponent<RectTransform>();
		canvasGroup = GetComponent<CanvasGroup>();

		canvasGroup.alpha = 0.6f;
		canvasGroup.blocksRaycasts = false;
	}

	public void Init(EnhancedCardItem cardItem, EnhancedCardView cardView)
	{
		gameObject.SetActive(true);
		//originalCardItem = cardItem;
		//originalHandItem = null;
		ShowImage();
		data = cardItem.GetCard();
		if (data != null)
		{
			NameText.text = data.name;
		}

		if (cardView != null)
		{
			CopyCardImage(cardView.GetCardImage());
		}
	}

	public void Init(EnhancedHandItem handItem, EnhancedCardView cardView)
	{
		gameObject.SetActive(true);
		//originalHandItem = handItem;
		//originalCardItem = null;
		ShowImage();
		data = handItem.GetCard();
		if (data != null)
		{
			NameText.text = data.name;
		}

		if (cardView != null)
		{
			CopyCardImage(cardView.GetCardImage());
		}
	}

	public void AddAnchoredPosition(Vector2 offset)
	{
		rectTransform.anchoredPosition += offset;
	}

	public void FollowMouse(Vector2 mousePosition, Canvas canvas, PointerEventData eventData)
	{
		if (canvas == null || rectTransform == null)
		{
			return;
		}

		/*
		RectTransformUtility.ScreenPointToLocalPointInRectangle(
			canvas.transform as RectTransform,
			mousePosition,
			canvas.worldCamera,
			out Vector2 localPoint);

		rectTransform.anchoredPosition = localPoint;
		*/
		AddAnchoredPosition(eventData.delta / canvas.scaleFactor / 1.5f);
	}

	public void SetPosition(Vector3 position)
	{
		transform.position = position;
	}

	public string GetName() => NameText.text;

	public void SetHandItem(EnhancedHandItem handItem)
	{
		if (data == null)
		{
			handItem.ResetCard();
		}
		else
		{
			// Transfer data from the source card item to this hand item if needed
			// handItem.AddCardData(data, 1);
		}
	}

	public void SwapData(EnhancedHandItem newHandItem)
	{
		Deactivate();
	}

	public void SwapData(EnhancedCardItem newCardItem)
	{
		Deactivate();
	}

	private void ShowImage()
	{
		UnitImage.gameObject.SetActive(true);
	}

	private void CopyCardImage(Image sourceImage)
	{
		if (UnitImage == null || sourceImage == null)
		{
			return;
		}

		UnitImage.sprite = sourceImage.sprite;
		//UnitImage.preserveAspect = false;

		// Resize to fill the UnitImage rect
		/*
		if (UnitImage.sprite != null)
		{
			UnitImage.type = Image.Type.Simple;
			var rectTransform = UnitImage.rectTransform;
			var sprite = UnitImage.sprite;
			var spriteSize = sprite.rect.size;
			var rectSize = rectTransform.rect.size;
			if (rectSize.x <= 0 || rectSize.y <= 0 || spriteSize.x <= 0 || spriteSize.y <= 0)
			{
				return;
			}
			float xRatio = rectSize.x / spriteSize.x;
			float yRatio = rectSize.y / spriteSize.y;
			float scale = xRatio > yRatio ? xRatio : yRatio;
			rectTransform.localScale = new Vector3(scale, scale, 1);
			rectTransform.sizeDelta = spriteSize * scale;
		}
		*/
	}

	private void HideImage()
	{
		UnitImage.gameObject.SetActive(false);
		if (UnitImage != null)
		{
			UnitImage.sprite = null;
			UnitImage.material = null;
			UnitImage.rectTransform.localScale = Vector3.one;
			UnitImage.rectTransform.sizeDelta = Vector2.zero;
		}
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		ResetData();
		Deactivate();
	}

	public void ResetData()
	{
		data = null;
		HideImage();
	}

	public void Activate()
	{
		gameObject.SetActive(true);
	}

	public void Deactivate()
	{
		ResetData();
		gameObject.SetActive(false);
	}
}
