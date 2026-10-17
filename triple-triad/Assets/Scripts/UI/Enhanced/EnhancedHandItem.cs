using cards;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnhancedHandItem : MonoBehaviour, IDropHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	[SerializeField]
	private EnhancedCardView CardView;

	private Card card;
	private CardDrag cardDrag;
	private Canvas canvas;
	private Vector3 defaultPosition;

	private void Start()
	{
		canvas = FindAnyObjectByType<Canvas>();
		StartCoroutine(GetPositionCoroutine());
	}

	private IEnumerator GetPositionCoroutine()
	{
		yield return new WaitForSeconds(0.1f);
		defaultPosition = transform.position;
	}

	public void AddCardData(Card card, int numUnits)
	{
		this.card = card;

		ResetCard();
		CardView.Load(card, Color.blue);
	}

	public void ResetCard()
	{
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
		if (card == null)
		{
			return;
		}

		GetCardDrag().FollowMouse(eventData.position, canvas, eventData);
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (card == null)
		{
			return;
		}

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
		if (card == null)
		{
			return;
		}

		GetCardDrag().Deactivate();
	}

	public void SetCardItem(EnhancedCardItem cardItem)
	{
		Debug.Log($"card item {cardItem.GetCard().name} dropped into hand item {name}");
		// Implement bidirectional logic as needed
	}
}
