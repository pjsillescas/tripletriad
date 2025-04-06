using UnityEngine;

public class RandomRule : IRuleVariation
{
	// Five cards are randomly chosen from the player's deck instead of the player being able to choose five cards themselves.
	public bool UseCardBack() => true;

}
