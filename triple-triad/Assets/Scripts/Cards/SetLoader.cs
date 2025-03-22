using cards;
using System;
using System.Collections.Generic;
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
		public List<Card> cards;
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
		cards = cardSet.cards;

		OnSetLoaded?.Invoke(this, cards);
	}

	// Update is called once per frame
	void Update()
	{

	}
}
