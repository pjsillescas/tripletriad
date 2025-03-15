using System;
using System.Collections;
using UnityEngine;

public class HandCardAnimator : MonoBehaviour
{
	const float ANIMATION_SPEED = 15f;

	private Action finishLoadCard;
	private bool isEnabled;
	PlayingCard card;
	Vector3 endPosition;
	Vector3 direction;
	
	public void StartAnimation(PlayingCard card, Vector3 startPosition, Vector3 endPosition, float offsetTime, Action finishLoadCard)
	{
		isEnabled = false;
		card.transform.position = startPosition;
		this.card = card;
		this.endPosition = endPosition;
		this.finishLoadCard = finishLoadCard;
		direction = (endPosition - startPosition).normalized;
		StartCoroutine(SleepCoroutine(card, endPosition, offsetTime));
	}
	private IEnumerator SleepCoroutine(PlayingCard card, Vector3 endPosition, float offsetTime)
	{
		yield return new WaitForSeconds(offsetTime);
		isEnabled = true;
		yield return null;
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

		if ((card.transform.position - endPosition).sqrMagnitude <= 0.2f)
		{
			finishLoadCard();
			Destroy(this);
			return;
		}

		card.transform.position += ANIMATION_SPEED * Time.deltaTime * direction;
	}
}
