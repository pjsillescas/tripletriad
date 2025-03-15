using System.Collections.Generic;
using UnityEngine;
using Enums;
using cards;
using System.Collections;
using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

public class Hand : MonoBehaviour
{
	[SerializeField]
	private Team Team;
    [SerializeField]
    private List<PlayingCard> Cards;
    [SerializeField]
    private GameObject CardPrefab;
	[SerializeField]
	private Transform AnimationInitialPosition;

	private int numCardsToLoad;

	public EventHandler<Hand> OnHandLoaded;

	public List<PlayingCard> GetPlayingCards() => Cards;

	private void Awake()
	{
        Cards = new();
	}

	public void Initialize(List<Card> cards, bool useCardBack)
	{
		const float deltaHeight = 0.5f;
		const float offset = 1.25f;
		const float offsetTime = 0.1f;

		Unload();

		numCardsToLoad = cards.Count;
		var numCards = cards.Count;
		
		var i = 0;
		cards.ForEach(card => 
		{
			var cardObject = Instantiate(CardPrefab);
			var playingCard = cardObject.GetComponent<PlayingCard>();
			//playingCard.Load(card, Team, useCardBack);
			LoadWithDelay(playingCard, card, Team, useCardBack);

			//playingCard.transform.position += transform.position + new Vector3(0, (numCards - 1 - i) * deltaHeight, i * offset);

			if (useCardBack)
			{
				var rotation = playingCard.transform.rotation;
				playingCard.transform.rotation = new Quaternion(rotation.x, rotation.y, 180.0f, rotation.w);
			}
			var endPosition = playingCard.transform.position + transform.position + new Vector3(0, (numCards - 1 - i) * deltaHeight, i * offset);

			var cardAnimator = gameObject.AddComponent<HandCardAnimator>();
			if (cardAnimator != null)
			{
				cardAnimator.StartAnimation(playingCard, AnimationInitialPosition.position, endPosition, offsetTime * (numCards - i), FinishLoadCard);
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

	private void FinishLoadCard()
	{
		numCardsToLoad--;
		if (numCardsToLoad == 0)
		{
			OnHandLoaded?.Invoke(this, this);
		}
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
