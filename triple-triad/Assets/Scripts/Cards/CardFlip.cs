using System;
using System.Collections;
using UnityEngine;

public class CardFlip : MonoBehaviour
{
	const float FLIP_TIME = 1.0f;
	const float FLIP_TIME_HALF = FLIP_TIME / 2.0f;

	const float flipHeight = 10f;

	public float flipSpeed = 1.0f;
	private bool isFlipping = false;

	private Action onEndFlip;

	public void Flip(Action onEndFlip)
	{
		if (!isFlipping)
		{
			this.onEndFlip = onEndFlip;
			StartCoroutine(FlipAnimation());
		}
	}

	private IEnumerator FlipAnimation()
	{
		isFlipping = true;
		float elapsedTime = 0f;
		var startRotation = transform.rotation;
		var endRotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + 180);

		var startPosition = transform.position;
		var middlePosition = transform.position + new Vector3(0, flipHeight, 0);
		var endPosition = transform.position;

		while (elapsedTime < FLIP_TIME)
		{
			elapsedTime += Time.deltaTime * flipSpeed;
			transform.rotation = Quaternion.Lerp(startRotation, endRotation, elapsedTime);
			
			if(elapsedTime <= FLIP_TIME_HALF)
			{
				transform.position = Vector3.Lerp(startPosition, middlePosition, elapsedTime);
			}
            else
            {
				transform.position = Vector3.Lerp(middlePosition, endPosition, elapsedTime);
			}

			yield return null;
		}

		transform.rotation = endRotation;
		transform.position = endPosition;
		isFlipping = false;

		onEndFlip?.Invoke();
		onEndFlip = null;
	}
}
