using UnityEngine;

public class OpenRule : RuleVariationBase
{
	// Enables the player to see which cards the opponent is using.
	public override bool UseCardBack()
	{
		return false;
	}
}
