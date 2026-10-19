using cards;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnhancedHandItem : MonoBehaviour, IDropHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	public static event EventHandler<Card> OnDropCard;
	public static event EventHandler<Card> OnAddCard;

	[SerializeField]
	private EnhancedCardView CardView;

	private Card card;
	private CardDrag cardDrag;
	private Canvas canvas;
	private bool isDragging;

	private void Start()
	{
		canvas = FindAnyObjectByType<Canvas>();
		StartCoroutine(GetPositionCoroutine());
		isDragging = false;
	}

	private IEnumerator GetPositionCoroutine()
	{
		yield return new WaitForSeconds(0.1f);
	}

	public void AddCardData(Card card)
	{
		this.card = card;

		//ResetCard();
		CardView.Load(card, Color.blue);

		OnAddCard?.Invoke(this, card);
	}

	public void ResetCard()
	{
		OnDropCard?.Invoke(this, card);
		card = null;
		CardView.ResetCard();
	}

	public Card GetCard() => card;

	private CardDrag GetCardDrag()
	{
		if (cardDrag == null)
		{
			var cardDrags = FindObjectsByType<CardDrag>(FindObjectsInactive.Include);
			cardDrag = new List<CardDrag>(cardDrags).FirstOrDefault();
			if (cardDrag == null)
			{
				throw new System.Exception("There is no CardDrag element in the scene");
			}
		}

		return cardDrag;
	}

	public void OnDrop(PointerEventData eventData)
	{
		isDragging = false;
		if (eventData.pointerDrag == null)
		{
			return;
		}

		if (eventData.pointerDrag.TryGetComponent<EnhancedCardItem>(out var cardItem))
		{
			SetCardItem(cardItem);
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
		if (card == null)
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
		/*
		var rectTransform = cardDrag.GetComponentInChildren<RectTransform>();
		float width = rectTransform.sizeDelta.x;
		float height = rectTransform.sizeDelta.y;
		var offset = new Vector2(width, height);
		cardDrag.SetPosition(eventData.position - offset);
		*/
		cardDrag.Init(this, CardView);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		isDragging = false;
		
		if (card == null)
		{
			return;
		}

		GetCardDrag().Deactivate();
	}

	public void SetCardItem(EnhancedCardItem cardItem)
	{
		Debug.Log($"card item {cardItem.GetCard().name} dropped into hand item {name}");

		if (card != null || cardItem.GetLeftUnits() <= 0)
		{
			return;
		}

		cardItem.SubtractCard();
		AddCardData(cardItem.GetCard());
	}
}
