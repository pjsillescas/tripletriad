using Enums;
using System;
using UnityEngine;

public class AdversaryController : Controller
{

	protected override void OnTurnStart(object sender, Team currentTeam)
	{
        ;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		Initialize();
	}

	// Update is called once per frame
	void Update()
    {
		if (Team.Blue.Equals(gameManager.GetCurrentTeamTurn()))
        {
            return;
        }

        Debug.Log("Adversary turn start");

        gameManager.StartNextTurn();

        Debug.Log("Passing turn for now");
    }
}
