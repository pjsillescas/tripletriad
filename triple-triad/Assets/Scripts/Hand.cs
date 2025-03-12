using System.Collections.Generic;
using UnityEngine;
using Enums;
using cards;
using System.Collections;

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

	public List<PlayingCard> GetPlayingCards() => Cards;

	private void Awake()
	{
        Cards = new();
	}

	public void Initialize(bool useCardBack)
	{
		var allCards = Loader.GetCards();

		var cards = new List<Card>();
		for (int i = 0; i < NumCards; i++)
        {
			cards.Add(allCards[Random.Range(0, allCards.Count - 1)]);
        }

		Initialize(cards, useCardBack);
	}

	public void Initialize(List<Card> cards, bool useCardBack)
	{
		const float deltaHeight = 0.5f;
		const float offset = 1.25f;

		Unload();

		int i = 0;
		cards.ForEach(card => 
		{
			var cardObject = Instantiate(CardPrefab);
			var playingCard = cardObject.GetComponent<PlayingCard>();
			//playingCard.Load(card, Team, useCardBack);
			LoadWithDelay(playingCard, card, Team, useCardBack);

			playingCard.transform.position += transform.position + new Vector3(0, (NumCards - 1 - i) * deltaHeight, i * offset);

			if (useCardBack)
			{
				var rotation = playingCard.transform.rotation;
				playingCard.transform.rotation = new Quaternion(rotation.x, rotation.y, 180.0f, rotation.w);
			}

			Cards.Add(playingCard);
			i++;
		});
	}

	private void LoadWithDelay(PlayingCard playingCard, Card card, Team team, bool useCardBack)
	{
		StartCoroutine(DoLoadWithDelay(playingCard, card, team, useCardBack));
	}

	private IEnumerator DoLoadWithDelay(PlayingCard playingCard, Card card, Team team, bool useCardBack)
	{
		yield return new WaitForSeconds(0.1f);
		playingCard.Load(card, Team, useCardBack);
	}

	public void Drop(PlayingCard card)
    {
        Cards.Remove(card);
    }

	public void Unload()
	{
		Cards.ForEach(card => Destroy(card.gameObject));
		Cards.Clear();
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
