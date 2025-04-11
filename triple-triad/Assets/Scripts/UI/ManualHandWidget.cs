using cards;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManualHandWidget : MonoBehaviour
{
	[SerializeField]
	private CardLister CardLister;
	[SerializeField]
	private CardViewer CardViewer;
	[SerializeField]
	private GameObject CardViewerWidget;

	[SerializeField]
	private HandView HandView;

	[SerializeField]
	private Button StartButton;

	private List<Card> cards;
	private Action<List<Card>> onHandChosen;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		cards = new();
		SetLoader.OnSetLoaded += (sender, cardList) => SetLoaded(cardList);

		StartButton.enabled = false;
		StartButton.onClick.AddListener(StartButtonClick);

		DeactivateWidget();
	}

	public void ActivateWidget(Action<List<Card>> onHandChosen)
	{
		CardViewerWidget.SetActive(true);
		CardLister.gameObject.SetActive(true);
		HandView.gameObject.SetActive(true);

		ResetWidget();
		this.onHandChosen = onHandChosen;
	}

	public void ResetWidget()
	{
		cards.Clear();
		HandView.ResetWidget();
		CardLister.ResetList();
	}

	public void DeactivateWidget()
	{
		CardViewerWidget.SetActive(false);
		CardLister.gameObject.SetActive(false);
		HandView.gameObject.SetActive(false);
		this.onHandChosen = null;
	}

	private void StartButtonClick()
	{
		SoundManager.GetInstance().Click();

		onHandChosen?.Invoke(cards);
		DeactivateWidget();
	}

	private void SetLoaded(List<Card> cards)
	{
		var cardsClone = new List<Card>(cards);
		cardsClone.Reverse();

		CardLister.LoadSet(cardsClone, AddCard, RemoveCard);
		CardViewer.SetCallbacks(AddCard, RemoveCard);
	}

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

	// Update is called once per frame
	void Update()
	{

	}
}
