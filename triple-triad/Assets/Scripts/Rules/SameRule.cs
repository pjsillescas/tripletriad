using System;
using UnityEngine;

public class SameRule : RuleVariationBase
{
	// When a card is placed touching two or more other cards (one or both of them have to be the opposite color), and the touching sides of each card is the same (8 touching 8 for example), then the other two cards are flipped. Combo rule applies.

	public override bool ImplementsWinsDirection()
	{
		return true;
	}

	public override bool WinsDirection(PlayingCard card1, PlayingCard card2, Board.Direction direction)
	{
		return direction switch
		{
			Board.Direction.North => card1.GetNorth() >= card2.GetSouth(),
			Board.Direction.South => card1.GetSouth() >= card2.GetNorth(),
			Board.Direction.West => card1.GetWest() >= card2.GetEast(),
			Board.Direction.East => card1.GetEast() >= card2.GetWest(),
			_ => throw new Exception($"Invalid direction to check '{direction}'"),
		};
	}
}
