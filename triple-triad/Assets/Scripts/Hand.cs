using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using Enums;

public class Hand : MonoBehaviour
{
	[SerializeField]
	private Team Team;
	[SerializeField]
    private int NumCards;
    [SerializeField]
    private List<PlayingCard> Cards;
	[SerializeField]
	private SetLoader Loader;
    [SerializeField]
    private GameObject CardPrefab;

	private void Awake()
	{
        Cards = new();
	}

	public void Initialize(bool useCardBack)
	{
        const float deltaHeight = 0.5f;
        const float offset = 1.25f;


		var cards = Loader.GetCards();
        for (int i = 0; i < NumCards; i++)
        {
            var cardObject = Instantiate(CardPrefab);
            var card = cardObject.GetComponent<PlayingCard>();
            card.Load(cards[Random.Range(0, cards.Count - 1)], Team, useCardBack);
            card.transform.position += transform.position + new Vector3(0, (NumCards - 1 - i) *deltaHeight, i * offset);

            if(useCardBack)
            {
                var rotation = card.transform.rotation;
                card.transform.rotation = new Quaternion(rotation.x, rotation.y, 180.0f, rotation.w);
            }

			Cards.Add(card);
        }
	}

    public void Drop(PlayingCard card)
    {
        Cards.Remove(card);
    }

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
