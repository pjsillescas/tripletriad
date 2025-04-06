using System.Data;
using System.Threading;
using Unity.Burst.Intrinsics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
public class SameWallRule : RuleVariationBase
{
	// An extension of the Same rule.The edges of the board are counted as A ranks for the purposes of the Same rule. Combo rule applies.If the Same rule is not present in a region that has Same Wall, Same Wall will not appear in the list of rules when starting a game because it can have no effect without Same but it will be carried with the player to other regions, and can therefore still be spread.

}
