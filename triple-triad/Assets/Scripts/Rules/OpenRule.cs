using UnityEngine;

public class OpenRule : IRuleVariation
{
	// Enables the player to see which cards the opponent is using.
	public bool UseCardBack() => false;

}
