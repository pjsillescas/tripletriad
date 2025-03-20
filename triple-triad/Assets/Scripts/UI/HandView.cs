using cards;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HandView : MonoBehaviour
{
    [SerializeField]
    private List<TextMeshProUGUI> Texts;

    private int numCards;

    public void ResetWidget()
    {
        Texts.ForEach(text => text.text = "");
    }

    public void Refresh(List<Card> cards)
    {
        ResetWidget();

        int i = 0;
        Texts.ForEach(text => {
            if (i < cards.Count)
            {
                text.text = cards[i].name;
                i++;
            }
        });
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		if (Texts == null || Texts.Count == 0)
		{
			Debug.LogError("There are no texts in the hand widget");
		}
		
        numCards = Texts.Count;
		ResetWidget();
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
