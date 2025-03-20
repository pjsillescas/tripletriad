using cards;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;
using System;

public class CardLister : MonoBehaviour
{
	[SerializeField]
	private int MaxCards = 2;

	[SerializeField]
	private Transform ItemPrefab;

	[SerializeField]
	private Transform Content;

	private List<CardItem> items;

	public void LoadSet(List<Card> cardList, Func<Card, bool> addCard, Func<Card, bool> removeCard)
	{
		items.ForEach(card => {
			if (card != null)
			{
				Destroy(card.gameObject);
			}
		});

		items = cardList.Select(card => {
			var item = Instantiate(ItemPrefab, Content);
			var cardItem = item.GetComponentInChildren<CardItem>();
			cardItem.AddCardData(card, MaxCards, addCard, removeCard);
			return cardItem;
		}).ToList();
		
		// Empty card to see the last card in the scroll widget
		var item = Instantiate(ItemPrefab, Content);
		var cardItem = item.GetComponentInChildren<CardItem>();
		items.Add(cardItem);
	}

	public void ResetList()
	{
		items.ForEach(cardItem => cardItem.ResetCard());
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		items = new();
		
	}

	// Update is called once per frame
	void Update()
	{

	}
}
