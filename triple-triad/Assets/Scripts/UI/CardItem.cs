using cards;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class CardItem : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI TitleText;
	[SerializeField]
	private TextMeshProUGUI NumberText;
	[SerializeField]
    private Button AddButton;
	[SerializeField]
	private Button RemoveButton;

	private int numUnits;
    private int leftUnits;
    Card card;

    public void AddCardData(Card card, int numUnits)
    {
        this.numUnits = numUnits;
        this.card = card;
		
		ResetCard();
		TitleText.text = card.name;
		NumberText.text = leftUnits.ToString();
	}

	public void ResetCard()
	{
		leftUnits = numUnits;

		AddButton.enabled = true;
		RemoveButton.enabled = false;
	}

	private void Awake()
	{
        AddButton.onClick.AddListener(AddButtonClick);
		RemoveButton.onClick.AddListener(RemoveButtonClick);
        AddButton.enabled = false;
        RemoveButton.enabled = false;
	}

	private void AddButtonClick()
    {
        if(leftUnits <= 0)
        {
            return;
        }

        leftUnits--;
		NumberText.text = leftUnits.ToString();

		RemoveButton.enabled = leftUnits >= 0;
    }
	private void RemoveButtonClick()
	{
		if (leftUnits >= numUnits)
		{
			return;
		}
		
        leftUnits++;
		NumberText.text = leftUnits.ToString();

		AddButton.enabled = leftUnits <= numUnits;
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
