using cards;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SetLoader : MonoBehaviour
{
	public static EventHandler<List<Card>> OnSetLoaded;

	private static SetLoader instance = null;

	public string Set;
	private List<Card> cards;

	[Serializable]
	public class JsonSetObject
	{
		public List<CardRaw> cards;
	}

	public List<Card> GetCards() => cards;
	public static SetLoader GetInstance() => instance;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		if (instance != null)
		{
			Debug.LogError("SetLoader duplicated");
		}

		instance = this;
	}

	public void LoadSet(string set)
	{
		if (!set.Equals(Set) || cards == null)
		{
			Set = set;
			var fileName = $"Sets/{set}/cards.json";
			var jsonLoader = GetComponent<JsonLoader>();
			jsonLoader.Load(fileName, OnJsonLoad);
		}
		else
		{
			OnSetLoaded?.Invoke(this, cards);
		}
	}

	private void OnJsonLoad(string json)
	{
		var cardSet = JsonUtility.FromJson<JsonSetObject>(json);
		cards = cardSet.cards.Select(cardRaw => 
			new Card
			{
				name = cardRaw.name,
				set = cardRaw.set,
				nameFormat = cardRaw.nameFormat,
				values = cardRaw.values,
				level = cardRaw.level,
				elemental = (cardRaw.elemental == "") ? Card.Element.none : (Card.Element)System.Enum.Parse(typeof(Card.Element), cardRaw.elemental),
			}).ToList();
		cards.ForEach(card =>
		{
			card.set = Set;
			card.nameFormat = (Set.Equals("FFVIII")) ? "TT{0}.jpg" : "{0}.png";
		});
		OnSetLoaded?.Invoke(this, cards);
	}

	// Update is called once per frame
	void Update()
	{

	}
}
