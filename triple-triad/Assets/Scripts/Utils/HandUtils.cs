using cards;
using System.Collections.Generic;

public class HandUtils
{
	public static List<Card> GetRandomHand(SetLoader loader, int numCardsPerHand)
	{
		var allCards = loader.GetCards();

		var cards = new List<Card>();
		for (int i = 0; i < numCardsPerHand; i++)
		{
			cards.Add(allCards[UnityEngine.Random.Range(0, allCards.Count)]);
		}

		return cards;
	}
}
