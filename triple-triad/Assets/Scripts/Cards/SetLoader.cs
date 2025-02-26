using cards;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SetLoader : MonoBehaviour
{
    public string Set;
    private List<Card> cards;

    [Serializable]
    public class JsonSetObject
    {
        public List<Card> cards;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var fileName = $"Assets/Data/Sets/{Set}/cards.json";
        var json = File.ReadAllText(fileName);
        // Debug.Log($"json {json}");
		var cardSet = JsonUtility.FromJson<JsonSetObject>(json);
        cards = cardSet.cards;
        Debug.Log($"{cards.Count} cards");
        Debug.Log(new List<int>() { 1,2,3 });
        Debug.Log(cards.Select(c => c.name).ToList());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
