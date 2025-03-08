using Enums;
using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event EventHandler<Team> OnNewTurn;

    private static GameManager Instance = null;

    private Team currentTeamTurn;

	private void Awake()
	{
        if (Instance != null)
        {
            Debug.LogError("GameManager instance duplicated");
        }

		Instance = this;
	}

    public static GameManager GetInstance() => Instance;

    public Team GetCurrentTeamTurn() => currentTeamTurn;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        currentTeamTurn = (UnityEngine.Random.Range(0, 2) == 0) ? Team.Blue : Team.Red;
    }

    public void StartNextTurn()
    {
        currentTeamTurn = Team.Red.Equals(currentTeamTurn) ? Team.Blue : Team.Red;
        OnNewTurn?.Invoke(this, currentTeamTurn);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
