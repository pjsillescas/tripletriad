using cards;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnhancedCardItem : MonoBehaviour, IDropHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	[SerializeField]
	private TextMeshProUGUI NumberText;
	
	[SerializeField]
	private EnhancedCardView CardView;

	private CardDrag cardDrag;
	private Canvas canvas;

	private int numUnits;
	private int leftUnits;
	private Card card;
	private Vector3 defaultPosition;
	private bool isDragging;

	private void Start()
	{
		canvas = FindAnyObjectByType<Canvas>();
		isDragging = false;

		StartCoroutine(GetPositionCoroutine());
		ResetCard();
	}

	private IEnumerator GetPositionCoroutine()
	{
		yield return new WaitForSeconds(1.0f);
		defaultPosition = transform.position;

		yield return null;
	}


	public void AddCardData(Card card, int numUnits)
	{
		this.numUnits = numUnits;
		this.card = card;

		ResetCard();
		CardView.Load(card, Color.blue);
	}

	public void ResetCard()
	{
		leftUnits = numUnits;
		UpdateNumberText();
	}

	private void UpdateNumberText()
	{
		NumberText.text = $"x {leftUnits}";
	}

	public Card GetCard() => card;

	public int GetNumUnits() => numUnits;
	public int GetLeftUnits() => leftUnits;
	public void SetLeftUnits(int leftUnits)
	{
		this.leftUnits = leftUnits;
		UpdateNumberText();
	}

	/*
	public void AddButtonClick()
	{
		if (leftUnits <= 0)
		{
			return;
		}

		//if (addCard(card))
		{
			SoundManager.GetInstance().Click();

			leftUnits--;
			NumberText.text = leftUnits.ToString();

			// RemoveButton.enabled = leftUnits >= 0;
		}
	}
	
	public void RemoveButtonClick()
	{
		if (leftUnits >= numUnits)
		{
			return;
		}

		if (removeCard(card))
		{
			SoundManager.GetInstance().Click();

			leftUnits++;
			NumberText.text = leftUnits.ToString();

			// AddButton.enabled = leftUnits <= numUnits;
		}
	}
	*/

	public void Select()
	{
		NumberText.color = Color.red;
	}

	public void Deselect()
	{
		NumberText.color = Color.black;
	}
	
	private CardDrag GetCardDrag()
	{
		if (cardDrag == null)
		{
			var cardDrags = FindObjectsByType<CardDrag>(FindObjectsInactive.Include);
			cardDrag = new List<CardDrag>(cardDrags).FirstOrDefault();
			if (cardDrag == null)
			{
				throw new System.Exception("There is no UnitSetupDragPosition element in the scene");
			}
		}

		return cardDrag;
	}

	public void OnDrop(PointerEventData eventData)
	{
		if (eventData.pointerDrag == null)
		{
			return;
		}

		isDragging = false;
		if (eventData.pointerDrag.TryGetComponent<EnhancedHandItem>(out var handItem))
		{
			SetHandItem(handItem);
			return;
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (card == null || !isDragging)
		{
			return;
		}

		GetCardDrag().FollowMouse(eventData.position, canvas, eventData);
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (card == null || leftUnits <= 0)
		{
			isDragging = false;
			return;
		}

		isDragging = true;
		var cardDrag = GetCardDrag();
		var rectTransform = cardDrag.GetComponentInChildren<RectTransform>();
		float width = rectTransform.sizeDelta.x;
		float height = rectTransform.sizeDelta.y;

		var offset = new Vector2(width, -height);
		cardDrag.SetPosition(eventData.position + offset);
		cardDrag.Init(this, CardView);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (card == null)
		{
			return;
		}

		isDragging = false;
		GetCardDrag().Deactivate();
	}

	public void SetHandItem(EnhancedHandItem item)
	{
		Debug.Log($"hand item {item.name} dropped into card item {name}");

		//var card = item.GetCard();
		item.ResetCard();
		//OnHandItemDropCard?.Invoke(this, card);
	}

	public void	AddCard()
	{
		if (leftUnits < numUnits)
		{
			SetLeftUnits(leftUnits + 1);
		}
	}

	public void SubtractCard()
	{
		if (leftUnits > 0)
		{
			SetLeftUnits(leftUnits - 1);
		}
	}
}
