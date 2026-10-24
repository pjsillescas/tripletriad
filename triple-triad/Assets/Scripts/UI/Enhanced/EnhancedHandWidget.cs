using cards;
using Enums;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnhancedHandWidget : MonoBehaviour
{
	public enum HandWidgetMode
	{
		SOLO, PLAYER_1, PLAYER_2,
	}

	public class HandConfigurationData
	{
		public List<Card> cards;
		public Team team;
		public ControllerType type;
	}

	public static event EventHandler<HandConfigurationData> OnHandChosen;

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

	private List<Card> cards;
	private Team team;
	private HandWidgetMode mode;

	//private List<Card> cards;
	//private Image backgroundImage;

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
		//backgroundImage = GetComponent<Image>();

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

	public void ActivateWidget(Team team, HandWidgetMode mode)
	{
		this.mode = mode;
		this.team = team;
		CardLister.gameObject.SetActive(true);
		HandView.gameObject.SetActive(true);
		ButtonFiller.gameObject.SetActive(true);
		//backgroundImage.gameObject.SetActive(true);
		ResetWidget();
		//StartCoroutine(DelayReset());
	}

	public void ResetWidget()
	{
		/*
		if (cards == null)
		{
			cards = new();
		}

		Debug.Log("reset");
		cards.Clear();
		*/
		HandView.ResetWidget();
		CardLister.ResetList();
	}

	public void DeactivateWidget()
	{
		CardLister.gameObject.SetActive(false);
		HandView.gameObject.SetActive(false);
		ButtonFiller.gameObject.SetActive(false);
		//backgroundImage.gameObject.SetActive(false);
	}

	private void StartButtonClick()
	{
		SoundManager.GetInstance().Click();

		Debug.Log("display hands");
		OnHandChosen?.Invoke(this, new HandConfigurationData() { cards = HandView.GetHand(), team = team, type = ControllerType.PLAYER });

		if (mode == HandWidgetMode.SOLO)
		{
			Debug.Log("display hands");
			OnHandChosen?.Invoke(this, new HandConfigurationData() { cards = GetRandomHand(), team = GetOppositeTeam(team), type = ControllerType.IA });
		}

		DeactivateWidget();
	}

	private List<Card> GetRandomHand()
	{
		return HandUtils.GetRandomHand(cards, 5);
	}

	private Team GetOppositeTeam(Team team)
	{
		return team switch
		{
			Team.Blue => Team.Red,
			Team.Red => Team.Blue,
			_ => Team.None,
		};
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
		this.cards = cards;
		var cardsClone = new List<Card>(cards);
		cardsClone.Reverse();

		CardLister.LoadSet(cardsClone);
	}
	/*
	private bool RemoveCard(Card card)
	{
		cards.Remove(card);
		HandView.Refresh(cards);

		StartButton.enabled = false;

		return true;
	}

	private bool AddCard(Card card)
	{
		if (cards.Count != 5)
		{
			cards.Add(card);
			HandView.Refresh(cards);

			StartButton.enabled = cards.Count == 5;

			return true;
		}

		return false;
	}
	*/
}
