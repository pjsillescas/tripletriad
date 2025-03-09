using System;
using System.Collections;
using UnityEngine;

public class CardTravel : MonoBehaviour
{
	const float TRAVEL_TIME = 1.0f;
	const float TRAVEL_TIME_HALF = TRAVEL_TIME / 2.0f;

	public float flipSpeed = 1.0f;
	private bool isTravelling = false;

	private Action onEndTravel;
	private Vector3 endPosition;
	private Vector3 middlePosition;

	public void Travel(Vector3 endPosition, Vector3 middlePosition, Action onEndTravel)
	{
		if (!isTravelling)
		{
			this.endPosition = endPosition;
			this.middlePosition = middlePosition;
			this.onEndTravel = onEndTravel;
			StartCoroutine(TravelAnimation());
		}
	}

	private IEnumerator TravelAnimation()
	{
		isTravelling = true;
		float elapsedTime = 0f;

		var startPosition = transform.position;

		while (elapsedTime < TRAVEL_TIME)
		{
			elapsedTime += Time.deltaTime * flipSpeed;
			if (elapsedTime <= TRAVEL_TIME_HALF)
			{
				transform.position = Vector3.Lerp(startPosition, middlePosition, elapsedTime);
			}
			else
			{
				transform.position = Vector3.Lerp(middlePosition, endPosition, elapsedTime);
			}

			yield return null;
		}

		transform.position = endPosition;
		isTravelling = false;

		onEndTravel?.Invoke();
		onEndTravel = null;
	}
}
