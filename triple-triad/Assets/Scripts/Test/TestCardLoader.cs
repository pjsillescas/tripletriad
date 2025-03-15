using UnityEngine;

public class TestCardLoader : MonoBehaviour
{
	[SerializeField]
	private PlayingCard Card;
	[SerializeField]
	private SetLoader Loader;
	[SerializeField]
	private Hand PlayerHand;
	[SerializeField]
	private Hand OpponentHand;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		/*
		var card = Loader.GetCards()[Random.Range(0, Loader.GetCards().Count - 1)];
		Card.Load(card, "team");
		Debug.Log(Card);
		*/

		PlayerHand.Initialize(new(), false);
		OpponentHand.Initialize(new(), true);
	}

	// Update is called once per frame
	void Update()
	{

	}
}
