using cards;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SetLoader : MonoBehaviour
{
	public static EventHandler<List<Card>> OnSetLoaded;

	public string Set;
	private List<Card> cards;

	[Serializable]
	public class JsonSetObject
	{
		public List<Card> cards;
	}

	public List<Card> GetCards() => cards;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		var fileName = $"Sets/{Set}/cards.json";
		var jsonLoader = GetComponent<JsonLoader>();
		jsonLoader.Load(fileName, OnJsonLoad);
	}

	private void OnJsonLoad(string json)
	{
		var cardSet = JsonUtility.FromJson<JsonSetObject>(json);
		cards = cardSet.cards;

		OnSetLoaded?.Invoke(this, cards);
	}

	// Update is called once per frame
	void Update()
	{

	}
}
