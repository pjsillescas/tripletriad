using UnityEngine;

public class SameRule : IRuleVariation
{
	// When a card is placed touching two or more other cards (one or both of them have to be the opposite color), and the touching sides of each card is the same (8 touching 8 for example), then the other two cards are flipped. Combo rule applies.
	public bool UseCardBack() => true;

}
