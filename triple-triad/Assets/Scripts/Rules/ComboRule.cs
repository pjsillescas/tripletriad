using UnityEngine;

public class ComboRule : RuleVariation
{
	// Of the cards captured by the Same, Same Wall or Plus rule, if they are adjacent to another card whose rank is lower, it is captured as well. This is not a separate rule; any time Same or Plus is in effect, Combo is in effect as well.
}
