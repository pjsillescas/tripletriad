using cards;
using System.Collections.Generic;
using UnityEngine;

public class ElementalRule : RuleVariationBase
{
	const int MAX_TRIES = 4;

	// In the elemental rule, one or more of the spaces are randomly marked with an element. Some cards have elements in the upper-right corner. Ruby Dragon, for example, is fire-elemental, and Quezacotl is thunder-elemental. When an elemental card is placed on a corresponding element, each rank goes up a point. When any card is placed on a non-matching element, each rank goes down a point. This does not affect the Same, Plus and Same Wall rules, where the cards' original ranks apply.
	public override void Initialize()
	{
		var availableElements = new List<Card.Element>() {
			Card.Element.fire,
			Card.Element.wind,
			Card.Element.ice,
			Card.Element.earth,
			Card.Element.holy,
			Card.Element.poison,
			Card.Element.thunder,
			Card.Element.water,
		};

		var board = Board.GetInstance();
		var tiles = board.GetTiles();
		
		int numTries = Random.Range(0, MAX_TRIES);

		for (int i = 0;i < numTries; i++)
		{
			var tile = tiles[Random.Range(0, tiles.Count)];
			var element = availableElements[Random.Range(0, availableElements.Count)];
			Debug.Log($"element {element}");
			tile.SetElement(element);
		}
		/*
		tiles[0].SetElement(Card.Element.fire);
		tiles[1].SetElement(Card.Element.fire);
		tiles[2].SetElement(Card.Element.fire);
		tiles[3].SetElement(Card.Element.fire);
		tiles[4].SetElement(Card.Element.fire);
		tiles[5].SetElement(Card.Element.holy);
		tiles[6].SetElement(Card.Element.poison);
		tiles[7].SetElement(Card.Element.thunder);
		*/
	}
}
