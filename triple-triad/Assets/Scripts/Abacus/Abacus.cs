using System;
using System.Collections.Generic;
using UnityEngine;

public class Abacus : MonoBehaviour
{
	[SerializeField]
	private GameObject AbacusPointPrefab;
	[SerializeField]
	private List<Transform> places;

	private List<AbacusPoint> points;
	private int lastRedIndex;
	private bool isGameStarted;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		isGameStarted = false;

		points = new();
		for (int k = 0; k < 10; k++)
		{
			var point = Instantiate(AbacusPointPrefab, Vector3.zero, Quaternion.identity).GetComponent<AbacusPoint>();
			points.Add(point);
		}

		lastRedIndex = 0;

		GameManager.GetInstance().OnStartGame += OnStartGame;
		GameManager.GetInstance().OnFinishGame += OnFinishGame;
		GameManager.GetInstance().OnScoreChange += OnScoreChange;
	}

	private void OnDisable()
	{
		GameManager.GetInstance().OnStartGame -= OnStartGame;
		GameManager.GetInstance().OnFinishGame -= OnFinishGame;
		GameManager.GetInstance().OnScoreChange -= OnScoreChange;
	}

	private void SwapRed()
	{
		if (points == null || points.Count == 0)
		{
			return;
		}

		lastRedIndex++;

		var point = points[lastRedIndex];

		var targetIndex = points[lastRedIndex - 1].GetAbacusIndex() + 1;

		point.SetAbacusIndex(targetIndex, places[targetIndex].position);
		point.SetColor(AbacusPoint.PointColor.RED);
	}

	private void SwapBlue()
	{
		if (points == null || points.Count == 0)
		{
			return;
		}

		lastRedIndex--;

		var point = points[lastRedIndex + 1];

		var currentIndex = point.GetAbacusIndex();
		var targetIndex = points[lastRedIndex + 2].GetAbacusIndex() - 1;
		
		point.SetAbacusIndex(targetIndex, places[targetIndex].position);
		point.SetColor(AbacusPoint.PointColor.BLUE);
	}

	private void OnStartGame(object sender, EventArgs args)
	{
		for (int k = 0; k < 5; k++)
		{
			points[k].SetAbacusIndexInstant(k, places[k].position);
			points[k].SetColor(AbacusPoint.PointColor.RED);

			var kBlue = places.Count - 1 - k;
			points[9 - k].SetAbacusIndexInstant(kBlue, places[kBlue].position);
			points[9 - k].SetColor(AbacusPoint.PointColor.BLUE);
		}

		lastRedIndex = 4;

		isGameStarted = true;
	}

	private void OnFinishGame(object sender, EventArgs args)
	{
		isGameStarted = false;
	}

	private void OnScoreChange(object sender, GameManager.Score score)
	{
		if (!isGameStarted)
		{
			return;
		}

		var newRedScore = score.adversary;
		var lastRedScore = lastRedIndex + 1;

		var newPoints = newRedScore - lastRedScore;

		if (newPoints == 0)
		{
			return;
		}

		if (newPoints > 0)
		{
			for (var k = 0; k < newPoints; k++)
			{
				SwapRed();
			}
		}
		else
		{
			for (var k = 0; k < -newPoints; k++)
			{
				SwapBlue();
			}
		}
	}
}
