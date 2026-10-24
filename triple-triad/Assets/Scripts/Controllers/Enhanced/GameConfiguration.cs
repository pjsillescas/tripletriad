using Enums;
using UnityEngine;

public class GameConfiguration : MonoBehaviour
{
    [SerializeField]
    private Hand RedHand;
	[SerializeField]
	private Hand BlueHand;

	[SerializeField]
	private Transform RedMiddlePosition;
	[SerializeField]
	private Transform BlueMiddlePosition;

	public Hand GetHand(Team team)
	{
		return (team == Team.Red) ? RedHand : BlueHand;
	}

	public Transform GetMiddlePosition(Team team)
	{
		return (team == Team.Red) ? RedMiddlePosition : BlueMiddlePosition;
	}

}
