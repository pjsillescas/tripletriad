using cards;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnhancedHandView : MonoBehaviour
{
	[SerializeField]
	private List<EnhancedHandItem> items;

	public void ResetWidget()
	{
		items?.ForEach(item => item.ResetCard());
	}

	public void Refresh(List<Card> cards)
	{
		ResetWidget();
	}

	public List<Card> GetHand()
	{
		return items.Select(item => item.GetCard()).Where(card => card != null).ToList();
	}

	public List<EnhancedHandItem> GetAvailableItems()
	{
		return items.Where(item => item.GetCard() == null).ToList();
	}

	public bool IsHandComplete()
	{
		return items.All(item => item.GetCard() != null);
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		ResetWidget();
	}
}
