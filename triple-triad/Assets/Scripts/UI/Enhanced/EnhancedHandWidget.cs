using cards;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnhancedHandWidget : MonoBehaviour
{
	public static event EventHandler<List<Card>> OnHandChosen;

	[SerializeField]
	private EnhancedCardLister CardLister;
	[SerializeField]
	private EnhancedHandView HandView;
	[SerializeField]
	private GameObject ButtonFiller;

	[SerializeField]
	private Button StartButton;
	[SerializeField]
	private Button RandomHandButton;
	[SerializeField]
	private Button ResetHandButton;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		SetLoader.OnSetLoaded += OnSetLoaded;

		StartButton.interactable = false;
		StartButton.onClick.RemoveAllListeners();
		StartButton.onClick.AddListener(StartButtonClick);

		RandomHandButton.onClick.RemoveAllListeners();
		RandomHandButton.onClick.AddListener(RandomHandButtonClick);

		ResetHandButton.onClick.RemoveAllListeners();
		ResetHandButton.onClick.AddListener(ResetHandButtonClick);

		EnhancedHandItem.OnDropCard += HandItemDropCard;
		EnhancedHandItem.OnAddCard += HandItemAddCard;

		DeactivateWidget();
	}

	private void OnDestroy()
	{
		SetLoader.OnSetLoaded -= OnSetLoaded;
		EnhancedHandItem.OnDropCard -= HandItemDropCard;
		EnhancedHandItem.OnAddCard -= HandItemAddCard;
	}

	private void OnSetLoaded(object sender, List<Card> cardList) => SetLoaded(cardList);

	private void HandItemDropCard(object sender, Card card)
	{
		if (card == null)
		{
			return;
		}

		CardLister.AddCard(card);
	}

	private void HandItemAddCard(object sender, Card card)
	{
		StartButton.interactable = HandView.IsHandComplete();
	}

	public void ActivateWidget()
	{
		CardLister.gameObject.SetActive(true);
		HandView.gameObject.SetActive(true);
		ButtonFiller.gameObject.SetActive(true);
		ResetWidget();
	}

	public void ResetWidget()
	{
		HandView.ResetWidget();
		CardLister.ResetList();
		StartButton.interactable = false;
	}

	public void DeactivateWidget()
	{
		CardLister.gameObject.SetActive(false);
		HandView.gameObject.SetActive(false);
		ButtonFiller.gameObject.SetActive(false);
	}

	private void StartButtonClick()
	{
		SoundManager.GetInstance().Click();

		OnHandChosen?.Invoke(this, HandView.GetHand());
		DeactivateWidget();
	}

	private void RandomHandButtonClick()
	{
		CompleteRandomHand();
	}

	private void ResetHandButtonClick()
	{
		ResetHand();
	}

	private void ResetHand()
	{
		var items = HandView.GetItems();

		foreach(var item in items)
		{
			var card = item.GetCard();
			if (card != null)
			{
				item.ResetCard();
				CardLister.AddCard(card);
			}
		}
	}

	private EnhancedCardItem SelectRandomAvailableCard()
	{
		var availableCardItems = CardLister.GetAvailableCardItems();
		var randomIndex = UnityEngine.Random.Range(0, availableCardItems.Count);
		return availableCardItems[randomIndex];
	}
	
	private void CompleteRandomHand()
	{
		var availableItems = HandView.GetAvailableItems();

		availableItems.ForEach(handItem => handItem.SetCardItem(SelectRandomAvailableCard()));
	}

	private void SetLoaded(List<Card> cards)
	{
		var cardsClone = new List<Card>(cards);
		cardsClone.Reverse();

		CardLister.LoadSet(cardsClone);
	}
}
