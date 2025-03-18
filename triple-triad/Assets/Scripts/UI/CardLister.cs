using cards;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;

public class CardLister : MonoBehaviour
{
	[SerializeField]
	private int MaxCards = 2;

	[SerializeField]
	private Transform ItemPrefab;

	[SerializeField]
	private Transform Content;

	private List<CardItem> items;
	/*
	private void OnSetLoaded(object sender, List<Card> cardList)
	{
		LoadSet(cardList);
	}
	*/

	public void LoadSet(List<Card> cardList)
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
			cardItem.AddCardData(card, MaxCards);
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
		SetLoader.OnSetLoaded += (sender, cardList) => LoadSet(cardList);
	}

	// Update is called once per frame
	void Update()
	{

	}
}
