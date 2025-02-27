using UnityEngine;

public class TestCardLoader : MonoBehaviour
{
	[SerializeField]
	private PlayingCard Card;
	[SerializeField]
	private SetLoader Loader;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		var card = Loader.GetCards()[Random.Range(0, Loader.GetCards().Count - 1)];
		Card.Load(card);
		Debug.Log(Card);
	}

	// Update is called once per frame
	void Update()
	{

	}
}
