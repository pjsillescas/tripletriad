using Enums;
using UnityEngine;

public class Score : MonoBehaviour
{
	public int blue;
	public int red;
	public Team localTeam;

	public bool IsLocalWinner()
	{
		return localTeam == Team.Blue && blue > red
			|| localTeam == Team.Red && red > blue;
	}

	public bool IsDraw()
	{
		return blue == red;
	}

	public bool IsLocalLoser()
	{
		return localTeam == Team.Blue && blue < red
			|| localTeam == Team.Red && red < blue;
	}
}
