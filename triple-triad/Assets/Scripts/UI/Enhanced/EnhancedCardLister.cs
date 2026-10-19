using cards;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnhancedCardLister : MonoBehaviour
{
	[SerializeField]
	private int MaxCards = 2;

	[SerializeField]
	private Transform ItemPrefab;

	[SerializeField]
	private Transform Content;

	private List<EnhancedCardItem> items;

	private void CleanComponent()
	{
		items?.ForEach(card =>
		{
			if (card != null)
			{
				Destroy(card.gameObject);
			}
		});
		Content.DetachChildren();
		items?.Clear();
	}

	public List<EnhancedCardItem> GetAvailableCardItems()
	{
		return items?.Where(item => item.GetLeftUnits() > 0).ToList();
	}

	public void AddCard(Card card)
	{
		var item = items.FirstOrDefault(it => it.GetCard().Equals(card));

		if (item == null)
		{
			return;
		}

		//item.SetLeftUnits(item.GetLeftUnits() + 1);
		item.AddCard();
	}

	public void LoadSet(List<Card> cardList)
	{
		CleanComponent();
		gameObject.SetActive(true);

		items = cardList.Select(card =>
		{
			var item = Instantiate(ItemPrefab, Content);
			var cardItem = item.GetComponent<EnhancedCardItem>();
			cardItem.AddCardData(card, MaxCards);
			return cardItem;
		}).ToList();

		// Empty card to see the last card in the scroll widget
		//var item = Instantiate(ItemPrefab, Content);
		//var cardItem = item.GetComponent<EnhancedCardItem>();
		//items.Add(cardItem);

		/*
		Debug.Log("loadset cardlister");
		var card = cardList[0];
		var item = Instantiate(ItemPrefab, Content);
		var cardItem = item.GetComponent<EnhancedCardItem>();

		Debug.Log($"active cardlister {gameObject.activeInHierarchy}");
		//StartCoroutine(DelayReset(card, cardItem));
		cardItem.AddCardData(card, MaxCards);
		items.Add(cardItem);
		*/
		/*
		items = new List<Card>() { cardList[0], cardList[1] }.Select(card =>
		{
			var item = Instantiate(ItemPrefab, Content);
			var cardItem = item.GetComponent<EnhancedCardItem>();
			cardItem.AddCardData(card, MaxCards);
			return cardItem;
		}).ToList();
		*/
	}

	private IEnumerator DelayReset(Card card, EnhancedCardItem cardItem)
	{
		yield return new WaitForSeconds(3f);
		cardItem.AddCardData(card, MaxCards);
		items.Add(cardItem);
	}
	public void ResetList()
	{
		items?.ForEach(cardItem => cardItem.ResetCard());
	}

	void Awake()
	{
		items = new();
	}
}
