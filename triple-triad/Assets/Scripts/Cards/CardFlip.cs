using System;
using System.Collections;
using UnityEngine;

public class CardFlip : MonoBehaviour
{
	const float FLIP_TIME = 0.6f;
	const float FLIP_TIME_HALF = FLIP_TIME / 2.0f;

	const float flipHeight = 5f;

	public float flipSpeed = 2.0f * flipHeight / FLIP_TIME;
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
			var alpha = elapsedTime / FLIP_TIME;
			transform.rotation = Quaternion.Lerp(startRotation, endRotation, alpha);
			
			if(elapsedTime <= FLIP_TIME_HALF)
			{
				transform.position = Vector3.Lerp(startPosition, middlePosition, 2*alpha);
			}
			else
            {
				transform.position = Vector3.Lerp(middlePosition, endPosition, 2*alpha -1);
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
