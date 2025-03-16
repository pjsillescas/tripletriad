using Enums;
using System;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField]
    private Transform PlayerPosition;
	[SerializeField]
	private Transform AdversaryPosition;
    [SerializeField]
    private TokenTravel TokenTravel;

    private Team currentTeam;

	private void Awake()
	{
        ResetTurn();
	}

    public Team GetCurrentTeam() => currentTeam;

	public Team ResetTurn()
	{
        currentTeam = Team.None;
        return currentTeam;
	}

    void DoStartMovement(Action<Team> OnSelectedTeam)
    {
		var targetPosition = currentTeam switch
		{
			Team.Red => AdversaryPosition.position,
			Team.Blue => PlayerPosition.position,
			_ => Vector3.zero,
		};

		TokenTravel.StartMovement(targetPosition, () => OnSelectedTeam(currentTeam));

	}

	public void ChooseRandomTeam(Action<Team> OnSelectedTeam)
    {
        currentTeam = (UnityEngine.Random.Range(0, 2) == 0) ? Team.Blue : Team.Red;
        DoStartMovement(OnSelectedTeam);
	}

    public void SetNextTurn(Action<Team> OnSelectedTeam)
    {
		currentTeam = Team.Red.Equals(currentTeam) ? Team.Blue : Team.Red;
		DoStartMovement(OnSelectedTeam);
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
