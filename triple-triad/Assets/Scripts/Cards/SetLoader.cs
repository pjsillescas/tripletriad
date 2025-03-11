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
		
		//var json = File.ReadAllText(fileName);
		// Debug.Log($"json {json}");
	}

	private void OnJsonLoad(string json)
	{
		var cardSet = JsonUtility.FromJson<JsonSetObject>(json);
		cards = cardSet.cards;

		OnSetLoaded?.Invoke(this, cards);
		//Debug.Log($"{cards.Count} cards");
		//Debug.Log(new List<int>() { 1, 2, 3 });
		//Debug.Log(cards.Select(c => c.name).ToList()[0]);
	}

	// Update is called once per frame
	void Update()
	{

	}
}
