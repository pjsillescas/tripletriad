using Enums;
using System;
using UnityEngine;

public class TokenTravel : MonoBehaviour
{
	const float MOVEMENT_SPEED = 15f;

	private Action onFinishMovement;
	private bool isEnabled;
	Vector3 endPosition;
	Vector3 direction;

	public void StartMovement(Vector3 endPosition, Action onFinishMovement)
	{
		this.endPosition = endPosition;
		this.onFinishMovement = onFinishMovement;
		direction = (endPosition - transform.position).normalized;
		isEnabled = true;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		isEnabled = false;
	}

	// Update is called once per frame
	void Update()
	{
		if (!isEnabled)
		{
			return;
		}

		if ((transform.position - endPosition).sqrMagnitude <= 0.2f)
		{
			onFinishMovement();
			isEnabled = false;
			return;
		}

		transform.position += MOVEMENT_SPEED * Time.deltaTime * direction;
	}
}
