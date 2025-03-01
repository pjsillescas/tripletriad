using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
	[SerializeField]
	private string Team;
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
        var cards = Loader.GetCards();
        for (int i = 0; i < NumCards; i++)
        {
            var cardObject = Instantiate(CardPrefab);
            var card = cardObject.GetComponent<PlayingCard>();
            card.Load(cards[Random.Range(0, cards.Count - 1)], Team, useCardBack);
            card.transform.position += transform.position /*+ new Vector3(8f, 0, -3f)*/ + new Vector3(0, (NumCards - i)*0.1f, i * 1.25f);

            if(useCardBack)
            {
                var rotation = card.transform.rotation;
                card.transform.rotation = new Quaternion(rotation.x, rotation.y, 180.0f, rotation.w);
            }

			Cards.Add(card);
        }
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
