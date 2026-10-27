using Enums;
using UnityEngine;

public class GameConfiguration : MonoBehaviour
{
	public enum ControllerType { LOCAL_PLAYER, REMOTE_PLAYER, IA }

	public class TeamConfiguration
	{
		public ControllerType controllerType;
		public Transform middlePosition;
		public Hand hand;
	}

	[SerializeField]
	private Hand BlueHand;
	[SerializeField]
	private Hand RedHand;

	private TeamConfiguration redConfiguration;
	private TeamConfiguration blueConfiguration;

	private void Awake()
	{
		redConfiguration = new TeamConfiguration();
		blueConfiguration = new TeamConfiguration();
	}

	public void SetConfiguration(Team team, ControllerType controllerType, Transform middlePosition)
	{
		if (team == Team.Blue)
		{
			blueConfiguration = new TeamConfiguration() { controllerType = controllerType, middlePosition = middlePosition, hand = BlueHand };
		}
		else if (team == Team.Red)
		{
			redConfiguration = new TeamConfiguration() { controllerType = controllerType, middlePosition = middlePosition, hand = RedHand };
		}
		else
		{
			Debug.LogError("Invalid team to assign configuration to");
		}
	}

	public TeamConfiguration GetRedConfiguration() => redConfiguration;
	public TeamConfiguration GetBlueConfiguration() => blueConfiguration;
}
